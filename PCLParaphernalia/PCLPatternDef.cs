using System;

namespace PCLParaphernalia
{
    /// <summary>
    /// 
    /// Class handles a PCL Pattern Def object.
    /// 
    /// © Chris Hutchinson 2016
    /// 
    /// </summary>

    class PCLPatternDef
    {
        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Class variables.                                                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        private string _desc;
        private PCLPatternDefs.eType _type;

        private ushort _id;
        private ushort _idSec;
        private ushort _height;
        private ushort _width;

        private byte[] _pattern;

        //--------------------------------------------------------------------//
        //                                              C o n s t r u c t o r //
        // P C L P a t t e r n D e f                                          //
        //                                                                    //
        //--------------------------------------------------------------------//

        public PCLPatternDef(string desc,
                              PCLPatternDefs.eType type,
                              ushort id,
                              ushort idSec,
                              ushort height,
                              ushort width,
                              byte[] pattern)
        {
            _desc = desc;
            _type = type;
            _id = id;
            _idSec = idSec;
            _height = height;
            _width = width;
            _pattern = pattern;
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // D e s c                                                            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the pattern decription.                                     //
        //                                                                    //
        //--------------------------------------------------------------------//

        public string Desc
        {
            get { return _desc; }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // H e i g h t                                                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the Height of the pattern definition.                       //
        //                                                                    //
        //--------------------------------------------------------------------//

        public ushort Height
        {
            get { return _height; }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // I d                                                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the PCL identifier value for the pattern.                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        public ushort Id
        {
            get { return _id; }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // I d S e c                                                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the secondary 'identifier' value for the pattern.           //
        //                                                                    //
        //--------------------------------------------------------------------//

        public ushort IdSec
        {
            get { return _idSec; }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // P a t t e r n                                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the bytes which define the pattern.                         //
        //                                                                    //
        //--------------------------------------------------------------------//

        public byte[] Pattern
        {
            get { return _pattern; }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // W i d t h                                                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the Width of the pattern definition.                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        public ushort Width
        {
            get { return _width; }
        }
    }
}