using System;

namespace PCLParaphernalia
{
    /// <summary>
    ///
    /// <para>Class defines a PCL XL Embedded Data Length Definer tag.</para>
    /// <para>© Chris Hutchinson 2010</para>
    ///
    /// </summary>
    // [System.Reflection.ObfuscationAttribute(Feature = "properties renaming")]
    [System.Reflection.Obfuscation(
        Feature = "renaming",
        ApplyToMembers = true)]

    class PCLXLEmbedDataDef
    {
        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Class variables.                                                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        private readonly byte _tag;

        //--------------------------------------------------------------------//
        //                                              C o n s t r u c t o r //
        // P C L X L E m b e d D a t a D e f                                  //
        //                                                                    //
        //--------------------------------------------------------------------//

	    public PCLXLEmbedDataDef(byte tag,
                                    bool flagReserved,
                                    string description)
	    {
            _tag          = tag;
            FlagReserved = flagReserved;
            Description = description;
	    }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // D e s c r i p t i o n                                              //
        //                                                                    //
        //--------------------------------------------------------------------//

        public string Description { get; }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // F l a g R e s e r v e d                                            //
        //                                                                    //
        //--------------------------------------------------------------------//

        public bool FlagReserved { get; }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // T a g                                                              //
        //                                                                    //
        //--------------------------------------------------------------------//

        public string Tag
        {
            get
            {
                string tag = "0x" + _tag.ToString("x2");

                return tag;
            }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // T y p e                                                            //
        //                                                                    //
        //--------------------------------------------------------------------//

        public string Type
        {
            get
            {
                return "Embed Data Definer";
            }
        }
    }
}