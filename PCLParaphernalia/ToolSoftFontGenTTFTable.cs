namespace PCLParaphernalia
{
    /// <summary>
    /// 
    /// Class provides TTF Table handling for the Soft Font Generate tool.
    /// 
    /// © Chris Hutchinson 2012
    /// 
    /// </summary>

    class ToolSoftFontGenTTFTable
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

        private readonly uint _tag;
        private uint _checksum;
        private uint _offset;
        private uint _length;
        private int _padBytes;

        //--------------------------------------------------------------------//
        //                                              C o n s t r u c t o r //
        // T o o l S o f t G e n T T F T a b l e                              //
        //                                                                    //
        //--------------------------------------------------------------------//

        public ToolSoftFontGenTTFTable(uint tag)
        {
            _tag = tag;
            _checksum = 0;
            _offset = 0;
            _length = 0;
            _padBytes = 0;
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // T a b l e C h e c k s u m                                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the checksum of the table in the TTF file.                  //
        //                                                                    //
        //--------------------------------------------------------------------//

        public uint TableChecksum
        {
            get { return _checksum; }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // T a b l e L e n g t h                                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the (unpadded) length of the table in the TTF file.         //
        //                                                                    //
        //--------------------------------------------------------------------//

        public uint TableLength
        {
            get { return _length; }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // T a b l e O f f s e t                                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the offset (start position) of the table in the TTF file.   //
        //                                                                    //
        //--------------------------------------------------------------------//

        public uint TableOffset
        {
            get { return _offset; }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // T a b l e P a d B y t e s                                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the number (0->3) of pad bytes required for the table from  //
        // the TTF file.                                                      //
        //                                                                    //
        //--------------------------------------------------------------------//

        public int TablePadBytes
        {
            get { return _padBytes; }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // T a b l e P a d L e n                                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the (padded) length of the table in the TTF file.           //
        //                                                                    //
        //--------------------------------------------------------------------//

        public uint TablePadLen
        {
            get { return (uint)(_length + _padBytes); }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // T a b l e T a g                                                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the tag of the table in the TTF file.                       //
        //                                                                    //
        //--------------------------------------------------------------------//

        public uint TableTag
        {
            get { return _tag; }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t M e t r i c s                                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return metrics (size and position) of the table.                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        public void GetByteRange(ref uint offset,
                                 ref uint length)
        {
            offset = _offset;
            length = _length;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // i n i t i a l i s e                                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Reset tables details (except tag).                                 //
        //                                                                    //
        //--------------------------------------------------------------------//

        public void Initialise()
        {
            _checksum = 0;
            _offset = 0;
            _length = 0;
            _padBytes = 0;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s e t D e t a i l s                                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Set details (except tag) of the table.                             //
        //                                                                    //
        //--------------------------------------------------------------------//

        public void SetMetrics(uint checksum,
                               uint offset,
                               uint length,
                               int padBytes)
        {
            _checksum = checksum;
            _offset = offset;
            _length = length;
            _padBytes = padBytes;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // z e r o L e n g t h                                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return value indicating whether table is present (non-zero length) //
        // or not.                                                            //
        //                                                                    //
        //--------------------------------------------------------------------//

        public bool ZeroLength()
        {
            return _length == 0;
        }
    }
}
