namespace PCLParaphernalia
{
    /// <summary>
    ///
    /// <para>Class handles 'link' data for 'parsing' of print file.</para>
    /// <para>© Chris Hutchinson 2010</para>
    ///
    /// </summary>
    class PrnParseLinkData
    {
        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Constants and enumerations.                                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Class variables.                                                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        private PrnParseConstants.ContType _contType;

        private long _pclComboStart;

        private int _prefixLen;
        private int _downloadRem;

        private bool _backTrack;
        private bool _eof;

        private byte _prefixA;
        private byte _prefixB;

        //--------------------------------------------------------------------//
        //                                              C o n s t r u c t o r //
        // P r n P a r s e L i n k D a t a                                    //
        //                                                                    //
        //--------------------------------------------------------------------//

        public PrnParseLinkData(
            PrnParse analysisOwner,
            int analysisLevel,
            int macroLevel,
            PCLXLOperators.EmbedDataType pclxlEmbedType)
        {
            AnalysisOwner = analysisOwner;
            AnalysisLevel = analysisLevel;
            MacroLevel = macroLevel;
            PclxlEmbedType = pclxlEmbedType;

            _contType = PrnParseConstants.ContType.None;
            _prefixLen = 0;
            DataLen = 0;
            _downloadRem = 0;

            EntryCt = 0;
            EntryNo = 0;
            EntryRem = 0;
            EntrySz1 = 0;
            EntrySz2 = 0;

            _backTrack = false;
            _prefixA = 0x00;
            _prefixB = 0x00;

            _eof = false;

            FileSize = 0;

            MakeOvlOffset = 0;
            MakeOvlSkipBegin = -1;
            MakeOvlSkipEnd = -1;
            MakeOvlAct = PrnParseConstants.OvlAct.None;
            MakeOvlPos = PrnParseConstants.OvlPos.BeforeFirstPage;
            MakeOvlShow = PrnParseConstants.OvlShow.None;
            MakeOvlMacroId = -1;
            MakeOvlStreamName = string.Empty;
            MakeOvlPageMark = false;
            MakeOvlXL = false;
            MakeOvlEncapsulate = false;

            _pclComboStart = -1;
            PclComboSeq = false;
            PclComboFirst = false;
            PclComboLast = false;
            PclComboModified = false;

            PrescribeSCRC = PrnParseConstants.prescribeSCRCDefault;
            PrescribeIntroRead = false;
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // A n a l y s i s L e v e l                                          //
        //                                                                    //
        //--------------------------------------------------------------------//

        public int AnalysisLevel { get; set; }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // A n a l y s i s O w n e r                                          //
        //                                                                    //
        //--------------------------------------------------------------------//

        public PrnParse AnalysisOwner { get; }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // B a c k T r a c k                                                  //
        //                                                                    //
        //--------------------------------------------------------------------//

        public bool BackTrack => _backTrack;

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // D a t a L e n                                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return or set continuation data length.                            //
        //                                                                    //
        //--------------------------------------------------------------------//

        public int DataLen { get; set; }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // E n t r y C t                                                      //
        //                                                                    //
        //--------------------------------------------------------------------//

        public int EntryCt { get; set; }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // E n t r y N o                                                      //
        //                                                                    //
        //--------------------------------------------------------------------//

        public int EntryNo { get; set; }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // E n t r y S z 1                                                    //
        //                                                                    //
        //--------------------------------------------------------------------//

        public int EntrySz1 { get; set; }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // E n t r y R e m                                                    //
        //                                                                    //
        //--------------------------------------------------------------------//

        public int EntryRem { get; set; }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // E n t r y S z 2                                                    //
        //                                                                    //
        //--------------------------------------------------------------------//

        public int EntrySz2 { get; set; }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // F i l e S i z e                                                    //
        //                                                                    //
        //--------------------------------------------------------------------//

        public long FileSize { get; set; }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t C o n t D a t a                                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return continuation data flags and values.                         //
        //                                                                    //
        //--------------------------------------------------------------------//

        public void GetContData(ref PrnParseConstants.ContType contType,
                                ref int prefixLen,
                                ref int dataLen,
                                ref int downloadRem,
                                ref bool backTrack,
                                ref byte prefixA,
                                ref byte prefixB)
        {
            contType = _contType;
            prefixLen = _prefixLen;
            dataLen = DataLen;
            downloadRem = _downloadRem;
            backTrack = _backTrack;
            prefixA = _prefixA;
            prefixB = _prefixB;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t C o n t T y p e                                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return stored continuation type identifier.                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        public PrnParseConstants.ContType GetContType()
        {
            return _contType;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t P C L C o m b o D a t a                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return 'PCL combination sequence' data flags and values.           //
        //                                                                    //
        //--------------------------------------------------------------------//

        public void GetPCLComboData(ref bool pclComboSeq,
                                    ref bool pclComboFirst,
                                    ref bool pclComboLast,
                                    ref bool pclComboModified,
                                    ref long pclComboStart)
        {
            pclComboSeq = PclComboSeq;
            pclComboFirst = PclComboFirst;
            pclComboLast = PclComboLast;
            pclComboModified = PclComboModified;
            pclComboStart = _pclComboStart;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t P r e f i x D a t a                                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return prefix information.                                         //
        //                                                                    //
        //--------------------------------------------------------------------//

        public void GetPrefixData(ref int prefixLen,
                                  ref byte prefixA,
                                  ref byte prefixB)
        {
            prefixLen = _prefixLen;
            prefixA = _prefixA;
            prefixB = _prefixB;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // i s C o n t i n u a t i o n                                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return value indicating continuation state.                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        public bool IsContinuation()
        {
            return _contType != PrnParseConstants.ContType.None;
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // I s E o f S e t                                                    //
        //                                                                    //
        //--------------------------------------------------------------------//

        public bool IsEofSet => _eof;

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // M a c r o L e v e l                                                //
        //                                                                    //
        //--------------------------------------------------------------------//

        public int MacroLevel { get; set; }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // m a c r o L e v e l A d j u s t                                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Increment or decrement Macro Level.                                //
        //                                                                    //
        //--------------------------------------------------------------------//

        public void MacroLevelAdjust(bool increment)
        {
            if (increment)
                MacroLevel++;
            else
                MacroLevel--;
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // M a k e O v l A c t                                                //
        //                                                                    //
        //--------------------------------------------------------------------//

        public PrnParseConstants.OvlAct MakeOvlAct { get; set; }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // M a k e O v l E n c a p s u l a t e                                //
        //                                                                    //
        //--------------------------------------------------------------------//

        public bool MakeOvlEncapsulate { get; set; }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // M a k e O v l M a c r o I d                                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        public int MakeOvlMacroId { get; set; }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // M a k e O v l O f f s e t                                          //
        //                                                                    //
        //--------------------------------------------------------------------//

        public long MakeOvlOffset { get; set; }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // M a k e O v l P a g e M a r k                                      //
        //                                                                    //
        //--------------------------------------------------------------------//

        public bool MakeOvlPageMark { get; set; }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // M a k e O v l P o s                                                //
        //                                                                    //
        //--------------------------------------------------------------------//

        public PrnParseConstants.OvlPos MakeOvlPos { get; set; }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // M a k e O v l R e s t o r e S t a t e X L                          //
        //                                                                    //
        //--------------------------------------------------------------------//

        public bool MakeOvlRestoreStateXL { get; set; }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // M a k e O v l S h o w                                              //
        //                                                                    //
        //--------------------------------------------------------------------//

        public PrnParseConstants.OvlShow MakeOvlShow { get; set; }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // M a k e O v l S k i p B e g i n                                    //
        //                                                                    //
        //--------------------------------------------------------------------//

        public long MakeOvlSkipBegin { get; set; }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // M a k e O v l S k i p E n d                                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        public long MakeOvlSkipEnd { get; set; }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // M a k e O v l S t r e a m N a m e                                  //
        //                                                                    //
        //--------------------------------------------------------------------//

        public string MakeOvlStreamName { get; set; }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // M a k e O v l X L                                                  //
        //                                                                    //
        //--------------------------------------------------------------------//

        public bool MakeOvlXL { get; set; }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // P c l C o m b o F i r s t                                          //
        //                                                                    //
        //--------------------------------------------------------------------//

        public bool PclComboFirst { get; set; }

        //--------------------------------------------------------------------//
        //                  `                                  P r o p e r t y //
        // P c l C o m b o L a s t                                            //
        //                                                                    //
        //--------------------------------------------------------------------//

        public bool PclComboLast { get; set; }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // P c l C o m b o M o d i f i e d                                    //
        //                                                                    //
        //--------------------------------------------------------------------//

        public bool PclComboModified { get; set; }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // P c l C o m b o S e q                                              //
        //                                                                    //
        //--------------------------------------------------------------------//

        public bool PclComboSeq { get; set; }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // P c l x l E m b e d T y p e                                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        public PCLXLOperators.EmbedDataType PclxlEmbedType { get; set; }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // P r e s c r i b e C a l l e r P D L                                //
        //                                                                    //
        //--------------------------------------------------------------------//

        public ToolCommonData.PrintLang PrescribeCallerPDL { get; set; }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // P r e s c r i b e I n t r o R e a d                                //
        //                                                                    //
        //--------------------------------------------------------------------//

        public bool PrescribeIntroRead { get; set; }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // P r e s c r i b e S C R C                                          //
        //                                                                    //
        //--------------------------------------------------------------------//

        public byte PrescribeSCRC { get; set; }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r e s e t C o n t D a t a                                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Reset 'link' continuation data flags and values.                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        public void ResetContData()
        {
            _contType = PrnParseConstants.ContType.None;
            _prefixLen = 0;
            DataLen = 0;
            _downloadRem = 0;
            _backTrack = false;
            _prefixA = 0x00;
            _prefixB = 0x00;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r e s e t P C L C o m b o D a t a                                  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Reset 'PCL combination sequence' data flags and values.            //
        //                                                                    //
        //--------------------------------------------------------------------//

        public void ResetPCLComboData()
        {
            PclComboSeq = false;
            PclComboFirst = false;
            PclComboLast = false;
            PclComboModified = false;
            _pclComboStart = -1;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s e t B a c k t r a c k                                            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Set (backtracking) continuation data flags and values.             //
        //                                                                    //
        //--------------------------------------------------------------------//

        public void SetBacktrack(PrnParseConstants.ContType contType,
                                  int dataLen)
        {
            _contType = contType;
            _prefixLen = 0;
            DataLen = dataLen;
            _downloadRem = 0;
            _backTrack = true;
            _prefixA = 0x20;
            _prefixB = 0x20;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s e t C o n t D a t a                                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Set continuation data flags and values.                            //
        //                                                                    //
        //--------------------------------------------------------------------//

        public void SetContData(PrnParseConstants.ContType contType,
                                int prefixLen,
                                int dataLen,
                                int downloadRem,
                                bool backTrack,
                                byte prefixA,
                                byte prefixB)
        {
            _contType = contType;
            _prefixLen = prefixLen;
            DataLen = dataLen;
            _downloadRem = downloadRem;
            _backTrack = backTrack;
            _prefixA = prefixA;
            _prefixB = prefixB;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s e t C o n t i n u a t i o n                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Set (non-backtracking) continuation data flags and values.         //
        //                                                                    //
        //--------------------------------------------------------------------//

        public void SetContinuation(PrnParseConstants.ContType contType)
        {
            _contType = contType;
            _prefixLen = 0;
            DataLen = 0;
            _downloadRem = 0;
            _backTrack = false;
            _prefixA = 0x20;
            _prefixB = 0x20;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s e t E o f                                                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Sets the end-of-file flag in the 'link' data.                      //
        //                                                                    //
        //--------------------------------------------------------------------//

        public void SetEof(bool eofSet)
        {
            _eof = eofSet;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s e t P C L C o m b o D a t a                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Set 'PCL combination sequence' data flags and values.              //
        //                                                                    //
        //--------------------------------------------------------------------//

        public void SetPCLComboData(bool pclComboSeq,
                                    bool pclComboFirst,
                                    bool pclComboLast,
                                    bool pclComboModified,
                                    long pclComboStart)
        {
            PclComboSeq = pclComboSeq;
            PclComboFirst = pclComboFirst;
            PclComboLast = pclComboLast;
            PclComboModified = pclComboModified;
            _pclComboStart = pclComboStart;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s e t P r e f i x D a t a                                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Set prefix information.                                            //
        //                                                                    //
        //--------------------------------------------------------------------//

        public void SetPrefixData(int prefixLen,
                                  byte prefixA,
                                  byte prefixB)
        {
            _prefixLen = prefixLen;
            _prefixA = prefixA;
            _prefixB = prefixB;
        }
    }
}