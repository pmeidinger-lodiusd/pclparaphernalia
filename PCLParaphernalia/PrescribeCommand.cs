using System;

namespace PCLParaphernalia
{
    /// <summary>
    /// 
    /// Class handles Kyocera Prescribe command object.
    /// 
    /// © Chris Hutchinson 2017
    /// 
    /// </summary>

    [System.Reflection.ObfuscationAttribute(Feature = "properties renaming")]

    class PrescribeCommand
    {
        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Class variables.                                                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        private string _cmdName;
        private string _cmdDesc;
        private bool _flagCmdIntro;
        private bool _flagCmdExit;
        private bool _flagCmdSetCRC;

        private int _statsCtParent;
        private int _statsCtChild;

        //--------------------------------------------------------------------//
        //                                              C o n s t r u c t o r //
        // P r e s c r i b e C o m m a n d                                    //
        //                                                                    //
        //--------------------------------------------------------------------//

        public PrescribeCommand(string name,
                                 string desc,
                                 bool flagCmdIntro,
                                 bool flagCmdExit,
                                 bool flagCmdSetCRC)
        {
            _cmdName = name;
            _cmdDesc = desc;
            _flagCmdIntro = flagCmdIntro;
            _flagCmdExit = flagCmdExit;
            _flagCmdSetCRC = flagCmdSetCRC;

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
            get { return _cmdDesc; }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // i n c r e m e n t S t a t i s t i c s C o u n t                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Increment 'statistics' count.                                      //
        //                                                                    //
        //--------------------------------------------------------------------//

        public void incrementStatisticsCount(int level)
        {
            if (level == 0)
                _statsCtParent++;
            else
                _statsCtChild++;
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // I s C m d E x i t                                                  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Returns the 'is exit command' flag.                                //
        //                                                                    //
        //--------------------------------------------------------------------//

        public bool IsCmdExit
        {
            get { return _flagCmdExit; }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // I s C m d I n t r o                                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Returns the 'is introduction command' flag.                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        public bool IsCmdIntro
        {
            get { return _flagCmdIntro; }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // I s C m d S e t C R C                                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Returns the 'is set CRC command' flag.                             //
        //                                                                    //
        //--------------------------------------------------------------------//

        public bool IsCmdSetCRC
        {
            get { return _flagCmdSetCRC; }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // N a m e                                                            //
        //                                                                    //
        //--------------------------------------------------------------------//

        public string Name
        {
            get { return _cmdName; }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r e s e t S t a t i s t i c s                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Reset 'statistics' counts.                                         //
        //                                                                    //
        //--------------------------------------------------------------------//

        public void resetStatistics()
        {
            _statsCtParent = 0;
            _statsCtChild = 0;
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
    }
}