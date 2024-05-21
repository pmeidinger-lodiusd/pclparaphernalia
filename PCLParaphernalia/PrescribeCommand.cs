using System;

namespace PCLParaphernalia
{
    /// <summary>
    ///
    /// <para>Class handles Kyocera Prescribe command object.</para>
    /// <para>© Chris Hutchinson 2017</para>
    ///
    /// </summary>
    [System.Reflection.Obfuscation(Feature = "properties renaming")]

    class PrescribeCommand
    {

        private int _statsCtParent;
        private int _statsCtChild;

        //--------------------------------------------------------------------//
        //                                              C o n s t r u c t o r //
        // P r e s c r i b e C o m m a n d                                    //
        //                                                                    //
        //--------------------------------------------------------------------//

        public PrescribeCommand (string name,
                                 string desc,
                                 bool flagCmdIntro,
                                 bool flagCmdExit,
                                 bool flagCmdSetCRC)
        {
            Name = name;
            Description = desc;
            IsCmdIntro = flagCmdIntro;
            IsCmdExit = flagCmdExit;
            IsCmdSetCRC = flagCmdSetCRC;

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

        public bool IsCmdExit { get; }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // I s C m d I n t r o                                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Returns the 'is introduction command' flag.                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        public bool IsCmdIntro { get; }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // I s C m d S e t C R C                                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Returns the 'is set CRC command' flag.                             //
        //                                                                    //
        //--------------------------------------------------------------------//

        public bool IsCmdSetCRC { get; }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // N a m e                                                            //
        //                                                                    //
        //--------------------------------------------------------------------//

        public string Name { get; }

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
            get { return _statsCtParent + _statsCtChild; }
        }
    }
}