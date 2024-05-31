namespace PCLParaphernalia
{
    /// <summary>
    ///
    /// <para>Class handles a PJL 'status readback' Variable object.</para>
    /// <para>© Chris Hutchinson 2010</para>
    ///
    /// </summary>
    internal class PJLVariable
    {
        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Class variables.                                                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        private readonly PJLVariables.VarType _varType;
        private readonly string _varName;

        //--------------------------------------------------------------------//
        //                                              C o n s t r u c t o r //
        // P J L V a r i a b l e                                              //
        //                                                                    //
        //--------------------------------------------------------------------//

        public PJLVariable(PJLVariables.VarType type, string name)
        {
            _varType = type;
            _varName = name;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t N a m e                                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the variable name.                                          //
        //                                                                    //
        //--------------------------------------------------------------------//

        public string GetName()
        {
            return _varName;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t T y p e                                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the variable type.                                          //
        //                                                                    //
        //--------------------------------------------------------------------//

        public PJLVariables.VarType GetVariableType()
        {
            return _varType;
        }
    }
}