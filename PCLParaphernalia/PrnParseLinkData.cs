namespace PCLParaphernalia
{
    /// <summary>
    /// 
    /// Class handles 'link' data for 'parsing' of print file.
    /// 
    /// © Chris Hutchinson 2010
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

        private PrnParseConstants.eContType _contType;
        private PrnParseConstants.eOvlAct _makeOvlAct;
        private PrnParseConstants.eOvlPos _makeOvlPos;
        private PrnParseConstants.eOvlShow _makeOvlShow;

        private readonly PrnParse _analysisOwner;

        private int _analysisLevel;
        private int _macroLevel;

        private long _fileSize;
        private long _makeOvlOffset;
        private long _makeOvlSkipBegin;
        private long _makeOvlSkipEnd;

        private long _pclComboStart;

        private int _makeOvlMacroId;

        private string _makeOvlStreamName;

        private PCLXLOperators.eEmbedDataType _pclxlEmbedType;

        private int _prefixLen;
        private int _dataLen;
        private int _downloadRem;

        private int _entryCt;
        private int _entryNo;
        private int _entryRem;
        private int _entrySz1;
        private int _entrySz2;

        private bool _pclComboSeq;
        private bool _pclComboFirst;
        private bool _pclComboLast;
        private bool _pclComboModified;

        private bool _makeOvlPageMark;
        private bool _makeOvlXL;
        private bool _makeOvlEncapsulate;
        private bool _makeOvlRestoreStateXL;

        private bool _backTrack;
        private bool _eof;

        private byte _prefixA;
        private byte _prefixB;

        private byte _prescribeSCRC;
        private bool _prescribeIntroRead;
        private ToolCommonData.ePrintLang _prescribeCallerPDL;

        //--------------------------------------------------------------------//
        //                                              C o n s t r u c t o r //
        // P r n P a r s e L i n k D a t a                                    //
        //                                                                    //
        //--------------------------------------------------------------------//

        public PrnParseLinkData(
            PrnParse analysisOwner,
            int analysisLevel,
            int macroLevel,
            PCLXLOperators.eEmbedDataType pclxlEmbedType)
        {
            _analysisOwner = analysisOwner;
            _analysisLevel = analysisLevel;
            _macroLevel = macroLevel;
            _pclxlEmbedType = pclxlEmbedType;

            _contType = PrnParseConstants.eContType.None;
            _prefixLen = 0;
            _dataLen = 0;
            _downloadRem = 0;

            _entryCt = 0;
            _entryNo = 0;
            _entryRem = 0;
            _entrySz1 = 0;
            _entrySz2 = 0;

            _backTrack = false;
            _prefixA = 0x00;
            _prefixB = 0x00;

            _eof = false;

            _fileSize = 0;

            _makeOvlOffset = 0;
            _makeOvlSkipBegin = -1;
            _makeOvlSkipEnd = -1;
            _makeOvlAct = PrnParseConstants.eOvlAct.None;
            _makeOvlPos = PrnParseConstants.eOvlPos.BeforeFirstPage;
            _makeOvlShow = PrnParseConstants.eOvlShow.None;
            _makeOvlMacroId = -1;
            _makeOvlStreamName = string.Empty;
            _makeOvlPageMark = false;
            _makeOvlXL = false;
            _makeOvlEncapsulate = false;

            _pclComboStart = -1;
            _pclComboSeq = false;
            _pclComboFirst = false;
            _pclComboLast = false;
            _pclComboModified = false;

            _prescribeSCRC = PrnParseConstants.prescribeSCRCDefault;
            _prescribeIntroRead = false;
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // A n a l y s i s L e v e l                                          //
        //                                                                    //
        //--------------------------------------------------------------------//

        public int AnalysisLevel
        {
            get { return _analysisLevel; }
            set { _analysisLevel = value; }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // A n a l y s i s O w n e r                                          //
        //                                                                    //
        //--------------------------------------------------------------------//

        public PrnParse AnalysisOwner
        {
            get { return _analysisOwner; }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // B a c k T r a c k                                                  //
        //                                                                    //
        //--------------------------------------------------------------------//

        public bool BackTrack
        {
            get { return _backTrack; }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // D a t a L e n                                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return or set continuation data length.                            //
        //                                                                    //
        //--------------------------------------------------------------------//

        public int DataLen
        {
            get { return _dataLen; }
            set { _dataLen = value; }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // E n t r y C t                                                      //
        //                                                                    //
        //--------------------------------------------------------------------//

        public int EntryCt
        {
            get { return _entryCt; }
            set { _entryCt = value; }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // E n t r y N o                                                      //
        //                                                                    //
        //--------------------------------------------------------------------//

        public int EntryNo
        {
            get { return _entryNo; }
            set { _entryNo = value; }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // E n t r y S z 1                                                    //
        //                                                                    //
        //--------------------------------------------------------------------//

        public int EntrySz1
        {
            get { return _entrySz1; }
            set { _entrySz1 = value; }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // E n t r y R e m                                                    //
        //                                                                    //
        //--------------------------------------------------------------------//

        public int EntryRem
        {
            get { return _entryRem; }
            set { _entryRem = value; }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // E n t r y S z 2                                                    //
        //                                                                    //
        //--------------------------------------------------------------------//

        public int EntrySz2
        {
            get { return _entrySz2; }
            set { _entrySz2 = value; }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // F i l e S i z e                                                    //
        //                                                                    //
        //--------------------------------------------------------------------//

        public long FileSize
        {
            get { return _fileSize; }
            set { _fileSize = value; }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t C o n t D a t a                                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return continuation data flags and values.                         //
        //                                                                    //
        //--------------------------------------------------------------------//

        public void getContData(ref PrnParseConstants.eContType contType,
                                ref int prefixLen,
                                ref int dataLen,
                                ref int downloadRem,
                                ref bool backTrack,
                                ref byte prefixA,
                                ref byte prefixB)
        {
            contType = _contType;
            prefixLen = _prefixLen;
            dataLen = _dataLen;
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

        public PrnParseConstants.eContType getContType()
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

        public void getPCLComboData(ref bool pclComboSeq,
                                    ref bool pclComboFirst,
                                    ref bool pclComboLast,
                                    ref bool pclComboModified,
                                    ref long pclComboStart)
        {
            pclComboSeq = _pclComboSeq;
            pclComboFirst = _pclComboFirst;
            pclComboLast = _pclComboLast;
            pclComboModified = _pclComboModified;
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

        public void getPrefixData(ref int prefixLen,
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

        public bool isContinuation()
        {
            return _contType != PrnParseConstants.eContType.None;
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // I s E o f S e t                                                    //
        //                                                                    //
        //--------------------------------------------------------------------//

        public bool IsEofSet
        {
            get { return _eof; }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // M a c r o L e v e l                                                //
        //                                                                    //
        //--------------------------------------------------------------------//

        public int MacroLevel
        {
            get { return _macroLevel; }
            set { _macroLevel = value; }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // m a c r o L e v e l A d j u s t                                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Increment or decrement Macro Level.                                //
        //                                                                    //
        //--------------------------------------------------------------------//

        public void macroLevelAdjust(bool increment)
        {
            if (increment)
                _macroLevel++;
            else
                _macroLevel--;
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // M a k e O v l A c t                                                //
        //                                                                    //
        //--------------------------------------------------------------------//

        public PrnParseConstants.eOvlAct MakeOvlAct
        {
            get { return _makeOvlAct; }
            set { _makeOvlAct = value; }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // M a k e O v l E n c a p s u l a t e                                //
        //                                                                    //
        //--------------------------------------------------------------------//

        public bool MakeOvlEncapsulate
        {
            get { return _makeOvlEncapsulate; }
            set { _makeOvlEncapsulate = value; }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // M a k e O v l M a c r o I d                                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        public int MakeOvlMacroId
        {
            get { return _makeOvlMacroId; }
            set { _makeOvlMacroId = value; }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // M a k e O v l O f f s e t                                          //
        //                                                                    //
        //--------------------------------------------------------------------//

        public long MakeOvlOffset
        {
            get { return _makeOvlOffset; }
            set { _makeOvlOffset = value; }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // M a k e O v l P a g e M a r k                                      //
        //                                                                    //
        //--------------------------------------------------------------------//

        public bool MakeOvlPageMark
        {
            get { return _makeOvlPageMark; }
            set { _makeOvlPageMark = value; }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // M a k e O v l P o s                                                //
        //                                                                    //
        //--------------------------------------------------------------------//

        public PrnParseConstants.eOvlPos MakeOvlPos
        {
            get { return _makeOvlPos; }
            set { _makeOvlPos = value; }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // M a k e O v l R e s t o r e S t a t e X L                          //
        //                                                                    //
        //--------------------------------------------------------------------//

        public bool MakeOvlRestoreStateXL
        {
            get { return _makeOvlRestoreStateXL; }
            set { _makeOvlRestoreStateXL = value; }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // M a k e O v l S h o w                                              //
        //                                                                    //
        //--------------------------------------------------------------------//

        public PrnParseConstants.eOvlShow MakeOvlShow
        {
            get { return _makeOvlShow; }
            set { _makeOvlShow = value; }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // M a k e O v l S k i p B e g i n                                    //
        //                                                                    //
        //--------------------------------------------------------------------//

        public long MakeOvlSkipBegin
        {
            get { return _makeOvlSkipBegin; }
            set { _makeOvlSkipBegin = value; }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // M a k e O v l S k i p E n d                                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        public long MakeOvlSkipEnd
        {
            get { return _makeOvlSkipEnd; }
            set { _makeOvlSkipEnd = value; }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // M a k e O v l S t r e a m N a m e                                  //
        //                                                                    //
        //--------------------------------------------------------------------//

        public string MakeOvlStreamName
        {
            get { return _makeOvlStreamName; }
            set { _makeOvlStreamName = value; }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // M a k e O v l X L                                                  //
        //                                                                    //
        //--------------------------------------------------------------------//

        public bool MakeOvlXL
        {
            get { return _makeOvlXL; }
            set { _makeOvlXL = value; }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // P c l C o m b o F i r s t                                          //
        //                                                                    //
        //--------------------------------------------------------------------//

        public bool PclComboFirst
        {
            get { return _pclComboFirst; }
            set { _pclComboFirst = value; }
        }

        //--------------------------------------------------------------------//
        //                  `                                  P r o p e r t y //
        // P c l C o m b o L a s t                                            //
        //                                                                    //
        //--------------------------------------------------------------------//

        public bool PclComboLast
        {
            get { return _pclComboLast; }
            set { _pclComboLast = value; }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // P c l C o m b o M o d i f i e d                                    //
        //                                                                    //
        //--------------------------------------------------------------------//

        public bool PclComboModified
        {
            get { return _pclComboModified; }
            set { _pclComboModified = value; }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // P c l C o m b o S e q                                              //
        //                                                                    //
        //--------------------------------------------------------------------//

        public bool PclComboSeq
        {
            get { return _pclComboSeq; }
            set { _pclComboSeq = value; }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // P c l x l E m b e d T y p e                                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        public PCLXLOperators.eEmbedDataType PclxlEmbedType
        {
            get { return _pclxlEmbedType; }
            set { _pclxlEmbedType = value; }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // P r e s c r i b e C a l l e r P D L                                //
        //                                                                    //
        //--------------------------------------------------------------------//

        public ToolCommonData.ePrintLang PrescribeCallerPDL
        {
            get { return _prescribeCallerPDL; }
            set { _prescribeCallerPDL = value; }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // P r e s c r i b e I n t r o R e a d                                //
        //                                                                    //
        //--------------------------------------------------------------------//

        public bool PrescribeIntroRead
        {
            get { return _prescribeIntroRead; }
            set { _prescribeIntroRead = value; }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // P r e s c r i b e S C R C                                          //
        //                                                                    //
        //--------------------------------------------------------------------//

        public byte PrescribeSCRC
        {
            get { return _prescribeSCRC; }
            set { _prescribeSCRC = value; }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r e s e t C o n t D a t a                                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Reset 'link' continuation data flags and values.                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        public void resetContData()
        {
            _contType = PrnParseConstants.eContType.None;
            _prefixLen = 0;
            _dataLen = 0;
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

        public void resetPCLComboData()
        {
            _pclComboSeq = false;
            _pclComboFirst = false;
            _pclComboLast = false;
            _pclComboModified = false;
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

        public void setBacktrack(PrnParseConstants.eContType contType,
                                  int dataLen)
        {
            _contType = contType;
            _prefixLen = 0;
            _dataLen = dataLen;
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

        public void setContData(PrnParseConstants.eContType contType,
                                int prefixLen,
                                int dataLen,
                                int downloadRem,
                                bool backTrack,
                                byte prefixA,
                                byte prefixB)
        {
            _contType = contType;
            _prefixLen = prefixLen;
            _dataLen = dataLen;
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

        public void setContinuation(PrnParseConstants.eContType contType)
        {
            _contType = contType;
            _prefixLen = 0;
            _dataLen = 0;
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

        public void setEof(bool eofSet)
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

        public void setPCLComboData(bool pclComboSeq,
                                    bool pclComboFirst,
                                    bool pclComboLast,
                                    bool pclComboModified,
                                    long pclComboStart)
        {
            _pclComboSeq = pclComboSeq;
            _pclComboFirst = pclComboFirst;
            _pclComboLast = pclComboLast;
            _pclComboModified = pclComboModified;
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

        public void setPrefixData(int prefixLen,
                                  byte prefixA,
                                  byte prefixB)
        {
            _prefixLen = prefixLen;
            _prefixA = prefixA;
            _prefixB = prefixB;
        }
    }
}