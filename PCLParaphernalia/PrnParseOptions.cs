using System.Reflection;
using System.Windows.Media;

namespace PCLParaphernalia
{
    /// <summary>
    /// 
    /// Class manages options for the AnalysePRN tool.
    /// 
    /// © Chris Hutchinson 2010
    /// 
    /// </summary>

    public class PrnParseOptions
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

        private string _prnFilename = string.Empty; // TEMPORARY ????????????????????????????????

        private PropertyInfo[] _stdClrsPropertyInfo;

        private PrnParseConstants.eOptCharSets _indxCharSetName;
        private PrnParseConstants.eOptCharSetSubActs _indxCharSetSubAct;
        private int _valCharSetSubCode;

        private PrnParseConstants.eOptOffsetFormats _indxGenOffsetFormat;

        private PrnParseConstants.eOptOffsetFormats _indxCurFOffsetFormat;

        private PrnParseConstants.eOptStatsLevel _indxStatsLevel;

        private ToolCommonData.ePrintLang _indxCurFInitLang;

        private PrnParseConstants.ePCLXLBinding _indxCurFXLBinding;

        private int _ctClrMapStdClrs;

        private int[] _indxClrMapBack;
        private int[] _indxClrMapFore;

        private int _valCurFOffsetStart;
        private int _valCurFOffsetEnd;
        private int _valCurFOffsetMax;

        private int _valPCLFontDrawHeight;
        private int _valPCLFontDrawWidth;

        private int _valPCLXLFontDrawHeight;
        private int _valPCLXLFontDrawWidth;

        private bool _flagClrMapUseClr;

        private bool _flagGenMiscAutoAnalyse;
        private bool _flagGenDiagFileAccess;

        private bool _flagHPGL2MiscBinData;

        private bool _flagPCLFontHddr;
        private bool _flagPCLFontChar;
        private bool _flagPCLFontDraw;
        private bool _flagPCLMacroDisplay;
        private bool _flagPCLMiscStyleData;
        private bool _flagPCLMiscBinData;

        private bool _flagPCLTransAlphaNumId;
        private bool _flagPCLTransColourLookup;
        private bool _flagPCLTransConfIO;
        private bool _flagPCLTransConfImageData;
        private bool _flagPCLTransConfRasterData;
        private bool _flagPCLTransDefLogPage;
        private bool _flagPCLTransDefSymSet;
        private bool _flagPCLTransDitherMatrix;
        private bool _flagPCLTransDriverConf;
        private bool _flagPCLTransEscEncText;
        private bool _flagPCLTransPaletteConf;
        private bool _flagPCLTransUserPattern;
        private bool _flagPCLTransViewIlluminant;

        private bool _flagPCLXLFontHddr;
        private bool _flagPCLXLFontChar;
        private bool _flagPCLXLFontDraw;
        private bool _flagPCLXLEncPCLFontSelect;
        private bool _flagPCLXLEncPCLPassThrough;
        private bool _flagPCLXLEncUserStream;
        private bool _flagPCLXLMiscBinData;
        private bool _flagPCLXLMiscOperPos;
        private bool _flagPCLXLMiscVerbose;

        private bool _flagPMLMiscVerbose;
        private bool _flagPMLWithinPCL;
        private bool _flagPMLWithinPJL;

        private bool _flagStatsExcUnusedPCLObs;
        private bool _flagStatsExcUnusedPCLXLRes;

        //--------------------------------------------------------------------//
        //                                              C o n s t r u c t o r //
        // P r n P a r s e O p t i o n s                                      //
        //                                                                    //
        //--------------------------------------------------------------------//

        public PrnParseOptions()
        {
            int ctRowTypes = PrnParseRowTypes.GetCount();

            _indxClrMapBack = new int[ctRowTypes];
            _indxClrMapFore = new int[ctRowTypes];

            MetricsLoad();
        }

        //--------------------------------------------------------------------//
        //                                              C o n s t r u c t o r //
        // P r n P a r s e O p t i o n s                                      //
        //                                                                    //
        //--------------------------------------------------------------------//

        public PrnParseOptions(PrnParseOptions parent)
        {
            int ctRowTypes = PrnParseRowTypes.GetCount();

            _indxClrMapBack = new int[ctRowTypes];
            _indxClrMapFore = new int[ctRowTypes];

            parent.GetOptCharSet(ref _indxCharSetName,
                                  ref _indxCharSetSubAct,
                                  ref _valCharSetSubCode);

            parent.GetOptClrMap(ref _flagClrMapUseClr,
                                 ref _indxClrMapBack,
                                 ref _indxClrMapFore);

            parent.GetOptClrMapStdClrs(ref _ctClrMapStdClrs,
                                        ref _stdClrsPropertyInfo);

            parent.GetOptCurF(ref _indxCurFInitLang,
                               ref _indxCurFXLBinding,
                               ref _indxCurFOffsetFormat,
                               ref _valCurFOffsetStart,
                               ref _valCurFOffsetEnd,
                               ref _valCurFOffsetMax);

            parent.GetOptGeneral(ref _indxGenOffsetFormat,
                                  ref _flagGenMiscAutoAnalyse,
                                  ref _flagGenDiagFileAccess);

            parent.GetOptHPGL2(ref _flagHPGL2MiscBinData);

            parent.GetOptPCL(ref _flagPCLFontHddr,
                              ref _flagPCLFontChar,
                              ref _flagPCLFontDraw,
                              ref _valPCLFontDrawHeight,
                              ref _valPCLFontDrawWidth,
                              ref _flagPCLMacroDisplay,
                              ref _flagPCLMiscStyleData,
                              ref _flagPCLMiscBinData,
                              ref _flagPCLTransAlphaNumId,
                              ref _flagPCLTransColourLookup,
                              ref _flagPCLTransConfIO,
                              ref _flagPCLTransConfImageData,
                              ref _flagPCLTransConfRasterData,
                              ref _flagPCLTransDefLogPage,
                              ref _flagPCLTransDefSymSet,
                              ref _flagPCLTransDitherMatrix,
                              ref _flagPCLTransDriverConf,
                              ref _flagPCLTransEscEncText,
                              ref _flagPCLTransPaletteConf,
                              ref _flagPCLTransUserPattern,
                              ref _flagPCLTransViewIlluminant);

            parent.GetOptPCLXL(ref _flagPCLXLFontHddr,
                                ref _flagPCLXLFontChar,
                                ref _flagPCLXLFontDraw,
                                ref _valPCLXLFontDrawHeight,
                                ref _valPCLXLFontDrawWidth,
                                ref _flagPCLXLEncUserStream,
                                ref _flagPCLXLEncPCLPassThrough,
                                ref _flagPCLXLEncPCLFontSelect,
                                ref _flagPCLXLMiscOperPos,
                                ref _flagPCLXLMiscBinData,
                                ref _flagPCLXLMiscVerbose);

            parent.GetOptPML(ref _flagPMLWithinPCL,
                              ref _flagPMLWithinPJL,
                              ref _flagPMLMiscVerbose);

            parent.GetOptStats(ref _indxStatsLevel,
                                ref _flagStatsExcUnusedPCLObs,
                                ref _flagStatsExcUnusedPCLXLRes);
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // F l a g C l r M a p U s e C l r                                    //
        //                                                                    //
        //--------------------------------------------------------------------//

        public bool FlagClrMapUseClr
        {
            get { return _flagClrMapUseClr; }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // F l a g G e n D i a g F i l e A c c e s s                          //
        //                                                                    //
        //--------------------------------------------------------------------//

        public bool FlagGenDiagFileAccess
        {
            get { return _flagGenDiagFileAccess; }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // F l a g G e n M i s c A u t o A n a l y s e                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        public bool FlagGenMiscAutoAnalyse
        {
            get { return _flagGenMiscAutoAnalyse; }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // F l a g P C L M i s c B i n D a t a                                //
        //                                                                    //
        //--------------------------------------------------------------------//

        public bool FlagPCLMiscBinData
        {
            get { return _flagPCLMiscBinData; }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // F l a g P C L X L M i s c B i n D a t a                            //
        //                                                                    //
        //--------------------------------------------------------------------//

        public bool FlagPCLXLMiscBinData
        {
            get { return _flagPCLXLMiscBinData; }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // F l a g P M L W i t h i n P C L                                    //
        //                                                                    //
        //--------------------------------------------------------------------//

        public bool FlagPMLWithinPCL
        {
            get { return _flagPMLWithinPCL; }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // F l a g P M L W i t h i n P J L                                    //
        //                                                                    //
        //--------------------------------------------------------------------//

        public bool FlagPMLWithinPJL
        {
            get { return _flagPMLWithinPJL; }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // F l a g P M L V e r b o s e                                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        public bool FlagPMLVerbose
        {
            get { return _flagPMLMiscVerbose; }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t O p t C h a r S e t                                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return current 'Character Set' options.                            //
        //                                                                    //
        //--------------------------------------------------------------------//

        public void GetOptCharSet(
            ref PrnParseConstants.eOptCharSets indxName,
            ref PrnParseConstants.eOptCharSetSubActs indxSubAct,
            ref int valSubCode)
        {
            indxName = _indxCharSetName;
            indxSubAct = _indxCharSetSubAct;
            valSubCode = _valCharSetSubCode;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t O p t C l r M a p                                            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return current 'Colour Map' options.                               //
        //                                                                    //
        //--------------------------------------------------------------------//

        public void GetOptClrMap(ref bool flagClrMapUseClr,
                                  ref int[] indxClrMapBack,
                                  ref int[] indxClrMapFore)
        {
            int ctRowTypes = PrnParseRowTypes.GetCount();

            flagClrMapUseClr = _flagClrMapUseClr;

            for (int i = 0; i < ctRowTypes; i++)
            {
                indxClrMapBack[i] = _indxClrMapBack[i];
                indxClrMapFore[i] = _indxClrMapFore[i];
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t O p t C l r M a p                                            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return current 'Colour Map' options.                               //
        //                                                                    //
        //--------------------------------------------------------------------//

        public void GetOptClrMapStdClrs(
            ref int ctClrMapStdClrs,
            ref PropertyInfo[] stdClrsPropertyInfo)
        {
            ctClrMapStdClrs = _ctClrMapStdClrs;
            stdClrsPropertyInfo = _stdClrsPropertyInfo;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t O p t C u r F                                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return current 'Current File' options.                             //
        //                                                                    //
        //--------------------------------------------------------------------//

        public void GetOptCurF(
            ref ToolCommonData.ePrintLang indxInitLang,
            ref PrnParseConstants.ePCLXLBinding indxXLBinding,
            ref PrnParseConstants.eOptOffsetFormats indxOffsetFormat,
            ref int valOffsetStart,
            ref int valOffsetEnd,
            ref int valOffsetMax)
        {
            indxInitLang = _indxCurFInitLang;
            indxXLBinding = _indxCurFXLBinding;
            indxOffsetFormat = _indxCurFOffsetFormat;
            valOffsetStart = _valCurFOffsetStart;
            valOffsetEnd = _valCurFOffsetEnd;
            valOffsetMax = _valCurFOffsetMax;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t O p t C u r F B a s i c                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return basic 'Current File' options.                               //
        //                                                                    //
        //--------------------------------------------------------------------//

        public void GetOptCurFBasic(
            ref ToolCommonData.ePrintLang indxInitLang,
            ref PrnParseConstants.ePCLXLBinding indxXLBinding,
            ref int valOffsetStart,
            ref int valOffsetEnd)
        {
            indxInitLang = _indxCurFInitLang;
            indxXLBinding = _indxCurFXLBinding;
            valOffsetStart = _valCurFOffsetStart;
            valOffsetEnd = _valCurFOffsetEnd;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t O p t C u r F O f f s e t s                                  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return 'Current File' offset options.                              //
        //                                                                    //
        //--------------------------------------------------------------------//

        public void GetOptCurFOffsets(
            ref int valOffsetStart,
            ref int valOffsetEnd)
        {
            valOffsetStart = _valCurFOffsetStart;
            valOffsetEnd = _valCurFOffsetEnd;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t O p t G e n e r a l                                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return current 'General' options.                                  //
        //                                                                    //
        //--------------------------------------------------------------------//

        public void GetOptGeneral(
            ref PrnParseConstants.eOptOffsetFormats indxOffsetFormat,
            ref bool flagAutoAnalyse,
            ref bool flagDiagFileAccess)
        {
            indxOffsetFormat = _indxGenOffsetFormat;
            flagAutoAnalyse = _flagGenMiscAutoAnalyse;
            flagDiagFileAccess = _flagGenDiagFileAccess;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t O p t H P G L 2                                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return current 'HP-GL/2' options.                                  //
        //                                                                    //
        //--------------------------------------------------------------------//

        public void GetOptHPGL2(ref bool flagMiscBinData)
        {
            flagMiscBinData = _flagHPGL2MiscBinData;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t O p t P C L                                                  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return current 'PCL' options.                                      //
        //                                                                    //
        //--------------------------------------------------------------------//

        public void GetOptPCL(ref bool flagFontHddr,
                              ref bool flagFontChar,
                              ref bool flagFontDraw,
                              ref int valFontDrawHeight,
                              ref int valFontDrawWidth,
                              ref bool flagMacroDisplay,
                              ref bool flagMiscStyleData,
                              ref bool flagMiscBinData,
                              ref bool flagTransAlphaNumId,
                              ref bool flagTransColourLookup,
                              ref bool flagTransConfIO,
                              ref bool flagTransConfImageData,
                              ref bool flagTransConfRasterData,
                              ref bool flagTransDefLogPage,
                              ref bool flagTransDefSymSet,
                              ref bool flagTransDitherMatrix,
                              ref bool flagTransDriverConf,
                              ref bool flagTransEscEncText,
                              ref bool flagTransPaletteConf,
                              ref bool flagTransUserPattern,
                              ref bool flagTransViewIlluminant)
        {
            flagFontHddr = _flagPCLFontHddr;
            flagFontChar = _flagPCLFontChar;
            flagFontDraw = _flagPCLFontDraw;

            valFontDrawHeight = _valPCLFontDrawHeight;
            valFontDrawWidth = _valPCLFontDrawWidth;

            flagMacroDisplay = _flagPCLMacroDisplay;

            flagMiscStyleData = _flagPCLMiscStyleData;
            flagMiscBinData = _flagPCLMiscBinData;

            flagTransAlphaNumId = _flagPCLTransAlphaNumId;
            flagTransColourLookup = _flagPCLTransColourLookup;
            flagTransConfIO = _flagPCLTransConfIO;
            flagTransConfImageData = _flagPCLTransConfImageData;
            flagTransConfRasterData = _flagPCLTransConfRasterData;
            flagTransDefLogPage = _flagPCLTransDefLogPage;
            flagTransDefSymSet = _flagPCLTransDefSymSet;
            flagTransDitherMatrix = _flagPCLTransDitherMatrix;
            flagTransDriverConf = _flagPCLTransDriverConf;
            flagTransEscEncText = _flagPCLTransEscEncText;
            flagTransPaletteConf = _flagPCLTransPaletteConf;
            flagTransUserPattern = _flagPCLTransUserPattern;
            flagTransViewIlluminant = _flagPCLTransViewIlluminant;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t O p t P C L B a s i c                                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return current 'PCL' basic options.                                //
        //                                                                    //
        //--------------------------------------------------------------------//

        public void GetOptPCLBasic(ref bool flagFontHddr,
                                   ref bool flagFontChar,
                                   ref bool flagMacroDisplay,
                                   ref bool flagMiscStyleData,
                                   ref bool flagMiscBinData,
                                   ref bool flagTransAlphaNumId,
                                   ref bool flagTransColourLookup,
                                   ref bool flagTransConfIO,
                                   ref bool flagTransConfImageData,
                                   ref bool flagTransConfRasterData,
                                   ref bool flagTransDefLogPage,
                                   ref bool flagTransDefSymSet,
                                   ref bool flagTransDitherMatrix,
                                   ref bool flagTransDriverConf,
                                   ref bool flagTransEscEncText,
                                   ref bool flagTransPaletteConf,
                                   ref bool flagTransUserPattern,
                                   ref bool flagTransViewIlluminant)
        {
            flagFontHddr = _flagPCLFontHddr;
            flagFontChar = _flagPCLFontChar;

            flagMacroDisplay = _flagPCLMacroDisplay;

            flagMiscStyleData = _flagPCLMiscStyleData;
            flagMiscBinData = _flagPCLMiscBinData;

            flagTransAlphaNumId = _flagPCLTransAlphaNumId;
            flagTransColourLookup = _flagPCLTransColourLookup;
            flagTransConfIO = _flagPCLTransConfIO;
            flagTransConfImageData = _flagPCLTransConfImageData;
            flagTransConfRasterData = _flagPCLTransConfRasterData;
            flagTransDefLogPage = _flagPCLTransDefLogPage;
            flagTransDefSymSet = _flagPCLTransDefSymSet;
            flagTransDitherMatrix = _flagPCLTransDitherMatrix;
            flagTransDriverConf = _flagPCLTransDriverConf;
            flagTransEscEncText = _flagPCLTransEscEncText;
            flagTransPaletteConf = _flagPCLTransPaletteConf;
            flagTransUserPattern = _flagPCLTransUserPattern;
            flagTransViewIlluminant = _flagPCLTransViewIlluminant;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t O p t P C L D r a w                                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return current 'PCL' font character draw options.                  //
        //                                                                    //
        //--------------------------------------------------------------------//

        public void GetOptPCLDraw(ref bool flagFontDraw,
                                  ref int valFontDrawHeight,
                                  ref int valFontDrawWidth)
        {
            flagFontDraw = _flagPCLFontDraw;

            valFontDrawHeight = _valPCLFontDrawHeight;
            valFontDrawWidth = _valPCLFontDrawWidth;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t O p t P C L X L                                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return current 'PCL XL' options.                                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        public void GetOptPCLXL(ref bool flagFontHddr,
                                ref bool flagFontChar,
                                ref bool flagFontDraw,
                                ref int valFontDrawHeight,
                                ref int valFontDrawWidth,
                                ref bool flagEncUserStream,
                                ref bool flagEncPCLPassThrough,
                                ref bool flagEncPCLFontSelect,
                                ref bool flagMiscOperPos,
                                ref bool flagMiscBinData,
                                ref bool flagMiscVerbose)
        {
            flagFontHddr = _flagPCLXLFontHddr;
            flagFontChar = _flagPCLXLFontChar;
            flagFontDraw = _flagPCLXLFontDraw;

            valFontDrawHeight = _valPCLXLFontDrawHeight;
            valFontDrawWidth = _valPCLXLFontDrawWidth;

            flagEncUserStream = _flagPCLXLEncUserStream;
            flagEncPCLPassThrough = _flagPCLXLEncPCLPassThrough;
            flagEncPCLFontSelect = _flagPCLXLEncPCLFontSelect;

            flagMiscOperPos = _flagPCLXLMiscOperPos;
            flagMiscBinData = _flagPCLXLMiscBinData;
            flagMiscVerbose = _flagPCLXLMiscVerbose;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t O p t P C L X L B a s i c                                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return current 'PCL XL' basic options.                             //
        //                                                                    //
        //--------------------------------------------------------------------//

        public void GetOptPCLXLBasic(ref bool flagFontHddr,
                                     ref bool flagFontChar,
                                     ref bool flagEncUserStream,
                                     ref bool flagEncPCLPassThrough,
                                     ref bool flagEncPCLFontSelect,
                                     ref bool flagMiscOperPos,
                                     ref bool flagMiscBinData,
                                     ref bool flagMiscVerbose)
        {
            flagFontHddr = _flagPCLXLFontHddr;
            flagFontChar = _flagPCLXLFontChar;

            flagEncUserStream = _flagPCLXLEncUserStream;
            flagEncPCLPassThrough = _flagPCLXLEncPCLPassThrough;
            flagEncPCLFontSelect = _flagPCLXLEncPCLFontSelect;

            flagMiscOperPos = _flagPCLXLMiscOperPos;
            flagMiscBinData = _flagPCLXLMiscBinData;
            flagMiscVerbose = _flagPCLXLMiscVerbose;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t O p t P C L X L D r a w                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return current 'PCL XL' font character draw options.               //
        //                                                                    //
        //--------------------------------------------------------------------//

        public void GetOptPCLXLDraw(ref bool flagFontDraw,
                                     ref int valFontDrawHeight,
                                     ref int valFontDrawWidth)
        {
            flagFontDraw = _flagPCLXLFontDraw;

            valFontDrawHeight = _valPCLXLFontDrawHeight;
            valFontDrawWidth = _valPCLXLFontDrawWidth;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t O p t P M L                                                  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return current 'PML' options.                                      //
        //                                                                    //
        //--------------------------------------------------------------------//

        public void GetOptPML(ref bool flagWithinPCL,
                              ref bool flagWithinPJL,
                              ref bool flagMiscVerbose)
        {
            flagWithinPCL = _flagPMLWithinPCL;
            flagWithinPJL = _flagPMLWithinPJL;
            flagMiscVerbose = _flagPMLMiscVerbose;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t O p t S t a t s                                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return current 'Statistics' options.                               //
        //                                                                    //
        //--------------------------------------------------------------------//

        public void GetOptStats(
            ref PrnParseConstants.eOptStatsLevel indxLevel,
            ref bool flagExcUnusedPCLObs,
            ref bool flagExcUnusedPCLXLRes)
        {
            indxLevel = _indxStatsLevel;
            flagExcUnusedPCLObs = _flagStatsExcUnusedPCLObs;
            flagExcUnusedPCLXLRes = _flagStatsExcUnusedPCLXLRes;
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // I n d x G e n O f f s e t F o r m a t                              //
        //                                                                    //
        //--------------------------------------------------------------------//

        public PrnParseConstants.eOptOffsetFormats IndxGenOffsetFormat
        {
            get { return _indxGenOffsetFormat; }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // I n d x C u r F X L B i n d i n g                                  //
        //                                                                    //
        //--------------------------------------------------------------------//

        public PrnParseConstants.ePCLXLBinding IndxCurFXLBinding
        {
            get { return _indxCurFXLBinding; }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // I n d x S t a t s L e v e l                                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        public PrnParseConstants.eOptStatsLevel IndxStatsLevel
        {
            get { return _indxStatsLevel; }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // m e t r i c s L o a d                                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Load metrics from persistent storage.                              //
        //                                                                    //
        //--------------------------------------------------------------------//

        public void MetricsLoad()
        {
            ToolPrnAnalysePersist.LoadData(ref _prnFilename);

            MetricsLoadCharSet();
            MetricsLoadClrMap();
            MetricsLoadGen();
            MetricsLoadHPGL2();
            MetricsLoadPCL();
            MetricsLoadPCLXL();
            MetricsLoadPML();
            MetricsLoadStats();

            MetricsLoadCurF();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // m e t r i c s L o a d C h a r S e t                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Load 'Character Set' option metrics from persistent storage.       //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void MetricsLoadCharSet()
        {
            int i1 = 0,
                  i2 = 0,
                  i3 = 0,
                  max;

            //----------------------------------------------------------------//

            ToolPrnAnalysePersist.LoadOptCharSet(ref i1,
                                                 ref i2,
                                                 ref i3);

            max = (int)PrnParseConstants.eOptCharSets.Max;

            if ((i1 < 0) || (i1 >= max))
                i1 = (int)PrnParseConstants.eOptCharSets.ISO_8859_1;

            _indxCharSetName = (PrnParseConstants.eOptCharSets)i1;

            max = (int)PrnParseConstants.eOptCharSetSubActs.Max;

            if ((i2 < 0) || (i2 >= max))
                i2 = (int)PrnParseConstants.eOptCharSetSubActs.Hex;

            _indxCharSetSubAct = (PrnParseConstants.eOptCharSetSubActs)i2;

            if ((i3 < PrnParseConstants.asciiSpace) ||
                (i3 == PrnParseConstants.asciiDEL) ||
                (i3 > PrnParseConstants.asciiMax8bit))
                i3 = PrnParseConstants.asciiSubDefault;

            _valCharSetSubCode = i3;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // m e t r i c s L o a d C l r M a p                                  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Load 'colour map' option metrics from persistent storage.          //
        //                                                                    //
        // Set the arrays to the application default values first, so that    //
        // these values (which are passed by reference to the subsequent      //
        // 'load from registry' function) are used as defaults if the         //
        // registry items do not yet exist (i.e. on first run of the version  //
        // which introduced the colour coding feature).                       // 
        //                                                                    //
        //--------------------------------------------------------------------//

        private void MetricsLoadClrMap()
        {
            PrnParseRowTypes.SetDefaultClrs(ref _indxClrMapBack,
                                             ref _indxClrMapFore);

            ToolPrnAnalysePersist.LoadOptClrMap(ref _flagClrMapUseClr);

            ToolPrnAnalysePersist.LoadOptClrMapCrnt(ref _indxClrMapBack,
                                                     ref _indxClrMapFore);

            //----------------------------------------------------------------//
            //                                                                //
            // Get the properties of the standard 'Colors' class object.      //
            //                                                                //
            // We should be provided with a list of the 140 standard colours  //
            // (as defined in .net, Unix X11, etc.) plus the 'transparent'    //
            // colour.                                                        //
            // This list is unlikely to change, so we can store the indices   //
            // of the selected colours and be relatively confident that these //
            // will always refer to the same actual colours.                  //
            //                                                                //
            //----------------------------------------------------------------//

            _stdClrsPropertyInfo = typeof(Colors).GetProperties();

            _ctClrMapStdClrs = _stdClrsPropertyInfo.Length;   // length = 141 //
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // m e t r i c s L o a d C u r F                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Load 'Current file' option metrics (default values).               //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void MetricsLoadCurF()
        {
            ResetOptCurF(-1);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // m e t r i c s L o a d G e n                                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Load 'General' option metrics from persistent storage.             //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void MetricsLoadGen()
        {
            int i1 = 0,
                  max;

            //----------------------------------------------------------------//

            ToolPrnAnalysePersist.LoadOptGeneral(ref i1,
                                                  ref _flagGenMiscAutoAnalyse,
                                                  ref _flagGenDiagFileAccess);

            max = (int)PrnParseConstants.eOptOffsetFormats.Max;

            if ((i1 < 0) || (i1 >= max))
                i1 = (int)PrnParseConstants.eOptOffsetFormats.Decimal;

            _indxGenOffsetFormat = (PrnParseConstants.eOptOffsetFormats)i1;

        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // m e t r i c s L o a d H P G L 2                                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Load 'HP-GL/2' option metrics from persistent storage.             //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void MetricsLoadHPGL2()
        {
            ToolPrnAnalysePersist.LoadOptHPGL2(ref _flagHPGL2MiscBinData);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // m e t r i c s L o a d P C L                                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Load metrics from persistent storage.                              //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void MetricsLoadPCL()
        {
            ToolPrnAnalysePersist.LoadOptPCL(ref _flagPCLFontHddr,
                                              ref _flagPCLFontChar,
                                              ref _flagPCLFontDraw,
                                              ref _valPCLFontDrawHeight,
                                              ref _valPCLFontDrawWidth,
                                              ref _flagPCLMacroDisplay,
                                              ref _flagPCLMiscStyleData,
                                              ref _flagPCLMiscBinData,
                                              ref _flagPCLTransAlphaNumId,
                                              ref _flagPCLTransColourLookup,
                                              ref _flagPCLTransConfIO,
                                              ref _flagPCLTransConfImageData,
                                              ref _flagPCLTransConfRasterData,
                                              ref _flagPCLTransDefLogPage,
                                              ref _flagPCLTransDefSymSet,
                                              ref _flagPCLTransDitherMatrix,
                                              ref _flagPCLTransDriverConf,
                                              ref _flagPCLTransEscEncText,
                                              ref _flagPCLTransPaletteConf,
                                              ref _flagPCLTransUserPattern,
                                              ref _flagPCLTransViewIlluminant);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // m e t r i c s L o a d P C L X L                                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Load 'PCL XL' option metrics from persistent storage.              //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void MetricsLoadPCLXL()
        {
            ToolPrnAnalysePersist.LoadOptPCLXL(ref _flagPCLXLFontHddr,
                                                ref _flagPCLXLFontChar,
                                                ref _flagPCLXLFontDraw,
                                                ref _valPCLXLFontDrawHeight,
                                                ref _valPCLXLFontDrawWidth,
                                                ref _flagPCLXLEncUserStream,
                                                ref _flagPCLXLEncPCLPassThrough,
                                                ref _flagPCLXLEncPCLFontSelect,
                                                ref _flagPCLXLMiscOperPos,
                                                ref _flagPCLXLMiscBinData,
                                                ref _flagPCLXLMiscVerbose);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // m e t r i c s L o a d P M L                                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Load 'PML' option metrics from persistent storage.                 //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void MetricsLoadPML()
        {
            ToolPrnAnalysePersist.LoadOptPML(ref _flagPMLWithinPCL,
                                              ref _flagPMLWithinPJL,
                                              ref _flagPMLMiscVerbose);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // m e t r i c s L o a d S t a t s                                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Load 'General' option metrics from persistent storage.             //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void MetricsLoadStats()
        {
            int i1 = 0,
                  max;

            //----------------------------------------------------------------//

            ToolPrnAnalysePersist.LoadOptStats(
                ref i1,
                ref _flagStatsExcUnusedPCLObs,
                ref _flagStatsExcUnusedPCLXLRes);

            max = (int)PrnParseConstants.eOptStatsLevel.Max;

            if ((i1 < 0) || (i1 >= max))
                i1 = (int)PrnParseConstants.eOptStatsLevel.ReferencedOnly;

            _indxStatsLevel = (PrnParseConstants.eOptStatsLevel)i1;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // m e t r i c s S a v e                                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Save metrics to persistent storage.                                //
        //                                                                    //
        //--------------------------------------------------------------------//

        public void MetricsSave()
        {
            ToolPrnAnalysePersist.SaveOptCharSet(
                (int)_indxCharSetName,
                (int)_indxCharSetSubAct,
                        _valCharSetSubCode);

            //----------------------------------------------------------------//

            ToolPrnAnalysePersist.SaveOptClrMap(_flagClrMapUseClr);

            ToolPrnAnalysePersist.SaveOptClrMapCrnt(_indxClrMapBack,
                                                     _indxClrMapFore);

            //----------------------------------------------------------------//

            ToolPrnAnalysePersist.SaveOptGeneral(
                (int)_indxGenOffsetFormat,
                        _flagGenMiscAutoAnalyse,
                        _flagGenDiagFileAccess);

            //----------------------------------------------------------------//

            ToolPrnAnalysePersist.SaveOptHPGL2(_flagHPGL2MiscBinData);

            //----------------------------------------------------------------//

            ToolPrnAnalysePersist.SaveOptPCL(_flagPCLFontHddr,
                                              _flagPCLFontChar,
                                              _flagPCLFontDraw,
                                              _valPCLFontDrawHeight,
                                              _valPCLFontDrawWidth,
                                              _flagPCLMacroDisplay,
                                              _flagPCLMiscStyleData,
                                              _flagPCLMiscBinData,
                                              _flagPCLTransAlphaNumId,
                                              _flagPCLTransColourLookup,
                                              _flagPCLTransConfIO,
                                              _flagPCLTransConfImageData,
                                              _flagPCLTransConfRasterData,
                                              _flagPCLTransDefLogPage,
                                              _flagPCLTransDefSymSet,
                                              _flagPCLTransDitherMatrix,
                                              _flagPCLTransDriverConf,
                                              _flagPCLTransEscEncText,
                                              _flagPCLTransPaletteConf,
                                              _flagPCLTransUserPattern,
                                              _flagPCLTransViewIlluminant);

            //----------------------------------------------------------------//

            ToolPrnAnalysePersist.SaveOptPCLXL(_flagPCLXLFontHddr,
                                                _flagPCLXLFontChar,
                                                _flagPCLXLFontDraw,
                                                _valPCLXLFontDrawHeight,
                                                _valPCLXLFontDrawWidth,
                                                _flagPCLXLEncUserStream,
                                                _flagPCLXLEncPCLPassThrough,
                                                _flagPCLXLEncPCLFontSelect,
                                                _flagPCLXLMiscOperPos,
                                                _flagPCLXLMiscBinData,
                                                _flagPCLXLMiscVerbose);

            //----------------------------------------------------------------//

            ToolPrnAnalysePersist.SaveOptPML(_flagPMLWithinPCL,
                                              _flagPMLWithinPJL,
                                              _flagPMLMiscVerbose);

            //----------------------------------------------------------------//

            ToolPrnAnalysePersist.SaveOptStats((int)_indxStatsLevel,
                                                _flagStatsExcUnusedPCLObs,
                                                _flagStatsExcUnusedPCLXLRes);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r e s e t O p t C u r F                                            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Reset 'Current File' options to defaults.                          //
        //                                                                    //
        //--------------------------------------------------------------------//

        public void ResetOptCurF(long fileSize)
        {
            _indxCurFInitLang = ToolCommonData.ePrintLang.PCL;
            _indxCurFXLBinding = PrnParseConstants.ePCLXLBinding.Unknown;
            _indxCurFOffsetFormat = _indxGenOffsetFormat;

            _valCurFOffsetMax = (int)fileSize;

            _valCurFOffsetStart = 0;
            _valCurFOffsetEnd = -1;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s e t O p t C h a r S e t                                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Set current 'Character Set' options.                               //
        //                                                                    //
        //--------------------------------------------------------------------//

        public void SetOptCharSet(
            PrnParseConstants.eOptCharSets indxName,
            PrnParseConstants.eOptCharSetSubActs indxSubAct,
            int valSubCode)
        {
            _indxCharSetName = indxName;
            _indxCharSetSubAct = indxSubAct;
            _valCharSetSubCode = valSubCode;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s e t O p t C l r M a p                                            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return current 'Colour Map' options.                               //
        //                                                                    //
        //--------------------------------------------------------------------//

        public void SetOptClrMap(bool flagClrMapUseClr,
                                  int[] indxClrMapBack,
                                  int[] indxClrMapFore)
        {
            _flagClrMapUseClr = flagClrMapUseClr;
            _indxClrMapBack = indxClrMapBack;
            _indxClrMapFore = indxClrMapFore;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s e t O p t C u r F                                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Set current 'Current File' options.                                //
        //                                                                    //
        //--------------------------------------------------------------------//

        public void SetOptCurF(
            ToolCommonData.ePrintLang indxInitLang,
            PrnParseConstants.ePCLXLBinding indxXLBinding,
            PrnParseConstants.eOptOffsetFormats indxOffsetFormat,
            int valOffsetStart,
            int valOffsetEnd)
        {
            _indxCurFInitLang = indxInitLang;
            _indxCurFXLBinding = indxXLBinding;
            _indxCurFOffsetFormat = indxOffsetFormat;
            _valCurFOffsetStart = valOffsetStart;
            _valCurFOffsetEnd = valOffsetEnd;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s e t O p t G e n e r a l                                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Set current 'General' options.                                     //
        //                                                                    //
        //--------------------------------------------------------------------//

        public void SetOptGeneral(
            PrnParseConstants.eOptOffsetFormats indxOffsetFormat,
            bool flagMiscAutoAnalyse,
            bool flagDiagFileAccess)
        {
            _indxGenOffsetFormat = indxOffsetFormat;
            _flagGenMiscAutoAnalyse = flagMiscAutoAnalyse;
            _flagGenDiagFileAccess = flagDiagFileAccess;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s e t O p t H P G L 2                                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Set current 'HP-GL/2' options.                                     //
        //                                                                    //
        //--------------------------------------------------------------------//

        public void SetOptHPGL2(bool flagMiscBinData)
        {
            _flagHPGL2MiscBinData = flagMiscBinData;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s e t O p t P C L                                                  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Set current 'PCL' options.                                         //
        //                                                                    //
        //--------------------------------------------------------------------//

        public void SetOptPCL(bool flagFontHddr,
                              bool flagFontChar,
                              bool flagFontDraw,
                              int valFontDrawHeight,
                              int valFontDrawWidth,
                              bool flagMacroDisplay,
                              bool flagMiscStyleData,
                              bool flagMiscBinData,
                              bool flagTransAlphaNumId,
                              bool flagTransColourLookup,
                              bool flagTransConfIO,
                              bool flagTransConfImageData,
                              bool flagTransConfRasterData,
                              bool flagTransDefLogPage,
                              bool flagTransDefSymSet,
                              bool flagTransDitherMatrix,
                              bool flagTransDriverConf,
                              bool flagTransEscEncText,
                              bool flagTransPaletteConf,
                              bool flagTransUserPattern,
                              bool flagTransViewIlluminant)
        {
            _flagPCLFontHddr = flagFontHddr;
            _flagPCLFontChar = flagFontChar;
            _flagPCLFontDraw = flagFontDraw;

            _valPCLFontDrawHeight = valFontDrawHeight;
            _valPCLFontDrawWidth = valFontDrawWidth;

            _flagPCLMacroDisplay = flagMacroDisplay;

            _flagPCLMiscStyleData = flagMiscStyleData;
            _flagPCLMiscBinData = flagMiscBinData;

            _flagPCLTransAlphaNumId = flagTransAlphaNumId;
            _flagPCLTransColourLookup = flagTransColourLookup;
            _flagPCLTransConfIO = flagTransConfIO;
            _flagPCLTransConfImageData = flagTransConfImageData;
            _flagPCLTransConfRasterData = flagTransConfRasterData;
            _flagPCLTransDefLogPage = flagTransDefLogPage;
            _flagPCLTransDefSymSet = flagTransDefSymSet;
            _flagPCLTransDitherMatrix = flagTransDitherMatrix;
            _flagPCLTransDriverConf = flagTransDriverConf;
            _flagPCLTransEscEncText = flagTransEscEncText;
            _flagPCLTransPaletteConf = flagTransPaletteConf;
            _flagPCLTransUserPattern = flagTransUserPattern;
            _flagPCLTransViewIlluminant = flagTransViewIlluminant;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s e t O p t P C L X L                                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Set current 'PCL XL' options.                                      //
        //                                                                    //
        //--------------------------------------------------------------------//

        public void SetOptPCLXL(bool flagFontHddr,
                                bool flagFontChar,
                                bool flagFontDraw,
                                int valFontDrawHeight,
                                int valFontDrawWidth,
                                bool flagEncUserStream,
                                bool flagEncPCLPassThrough,
                                bool flagEncPCLFontSelect,
                                bool flagMiscOperPos,
                                bool flagMiscBinData,
                                bool flagMiscVerbose)
        {
            _flagPCLXLFontHddr = flagFontHddr;
            _flagPCLXLFontChar = flagFontChar;
            _flagPCLXLFontDraw = flagFontDraw;

            _valPCLXLFontDrawHeight = valFontDrawHeight;
            _valPCLXLFontDrawWidth = valFontDrawWidth;

            _flagPCLXLEncUserStream = flagEncUserStream;
            _flagPCLXLEncPCLPassThrough = flagEncPCLPassThrough;
            _flagPCLXLEncPCLFontSelect = flagEncPCLFontSelect;

            _flagPCLXLMiscOperPos = flagMiscOperPos;
            _flagPCLXLMiscBinData = flagMiscBinData;
            _flagPCLXLMiscVerbose = flagMiscVerbose;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s e t O p t P M L                                                  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Set current 'PML' options.                                         //
        //                                                                    //
        //--------------------------------------------------------------------//

        public void SetOptPML(bool flagWithinPCL,
                              bool flagWithinPJL,
                              bool flagMiscVerbose)
        {
            _flagPMLWithinPCL = flagWithinPCL;
            _flagPMLWithinPJL = flagWithinPJL;
            _flagPMLMiscVerbose = flagMiscVerbose;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s e t O p t S t a t s                                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Set current 'Statistics' options.                                  //
        //                                                                    //
        //--------------------------------------------------------------------//

        public void SetOptStats(
            PrnParseConstants.eOptStatsLevel indxLevel,
            bool flagExcUnusedPCLObs,
            bool flagExcUnusedPCLXLRes)
        {
            _indxStatsLevel = indxLevel;
            _flagStatsExcUnusedPCLObs = flagExcUnusedPCLObs;
            _flagStatsExcUnusedPCLXLRes = flagExcUnusedPCLXLRes;
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // V a l C u r F O f f s e t E n d                                    //
        //                                                                    //
        //--------------------------------------------------------------------//

        public int ValCurFOffsetEnd
        {
            get { return _valCurFOffsetEnd; }
        }
    }
}
