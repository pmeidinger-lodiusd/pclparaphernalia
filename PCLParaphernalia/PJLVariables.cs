using System;

namespace PCLParaphernalia
{
    /// <summary>
    ///
    /// <para>Class defines a set of PJL 'status readback' Variable objects.</para>
    /// <para>© Chris Hutchinson 2010</para>
    ///
    /// </summary>
    static class PJLVariables
    {
        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Class variables.                                                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        public enum VarType
        {
            Custom,
            General,
            PCL,
            PDF,
            PS
        }

        private static readonly PJLVariable[] _vars =
        {
            new PJLVariable(VarType.Custom, "<specify value>"),
            new PJLVariable(VarType.General, "AUTOCONT"),
            new PJLVariable(VarType.General, "AUTOSELECT"),
            new PJLVariable(VarType.General, "BINDING"),
            new PJLVariable(VarType.General, "BITSPERPIXEL"),
            new PJLVariable(VarType.General, "CLEARABLEWARNINGS"),
            new PJLVariable(VarType.General, "CONTEXTSWITCH"),
            new PJLVariable(VarType.General, "COPIES"),
            new PJLVariable(VarType.General, "COURIER"),
            new PJLVariable(VarType.General, "CPLOCK"),
            new PJLVariable(VarType.General, "DENSITY"),
            new PJLVariable(VarType.General, "DISKLOCK"),
            new PJLVariable(VarType.General, "DUPLEX"),
            new PJLVariable(VarType.General, "ECONOMODE"),
            new PJLVariable(VarType.General, "EDGETOEDGE"),
            new PJLVariable(VarType.General, "FIH"),
            new PJLVariable(VarType.General, "FINISH"),
            new PJLVariable(VarType.General, "FINISHEROPTION"),
            new PJLVariable(VarType.General, "FINISHERTYPE"),
            new PJLVariable(VarType.General, "FORMLINES"),
            new PJLVariable(VarType.General, "HELDJOBTIMEOUT"),
            new PJLVariable(VarType.General, "HITRANSFER"),
            new PJLVariable(VarType.General, "HOLD"),
            new PJLVariable(VarType.General, "HOLDKEY"),
            new PJLVariable(VarType.General, "HOLDTYPE"),
            new PJLVariable(VarType.General, "HOSTCLEANINGPAGE"),
            new PJLVariable(VarType.General, "IMAGEADAPT"),
            new PJLVariable(VarType.General, "INTRAY1"),
            new PJLVariable(VarType.General, "INTRAY2"),
            new PJLVariable(VarType.General, "INTRAY3"),
            new PJLVariable(VarType.General, "INTRAY1SIZE"),
            new PJLVariable(VarType.General, "INTRAY2SIZE"),
            new PJLVariable(VarType.General, "INTRAY3SIZE"),
            new PJLVariable(VarType.General, "INTRAY4SIZE"),
            new PJLVariable(VarType.General, "INTRAY5SIZE"),
            new PJLVariable(VarType.General, "INTRAY6SIZE"),
            new PJLVariable(VarType.General, "INTRAY7SIZE"),
            new PJLVariable(VarType.General, "INTRAY8SIZE"),
            new PJLVariable(VarType.General, "IOBUFFER"),
            new PJLVariable(VarType.General, "IOSIZE"),
            new PJLVariable(VarType.General, "JOBATTR"),
            new PJLVariable(VarType.General, "JOBID"),
            new PJLVariable(VarType.General, "JOBIDVALUE"),
            new PJLVariable(VarType.General, "JOBMFQBEGIN"),
            new PJLVariable(VarType.General, "JOBMFQEND"),
            new PJLVariable(VarType.General, "JOBNAME"),
            new PJLVariable(VarType.General, "JOBOFFSET"),
            new PJLVariable(VarType.General, "JOBSOURCE"),
            new PJLVariable(VarType.General, "LANG"),
            new PJLVariable(VarType.General, "LANGPROMPT"),
            new PJLVariable(VarType.General, "LOWCARTRIDGE"),
            new PJLVariable(VarType.General, "LOWSUPPLIES"),
            new PJLVariable(VarType.General, "LOWTONER"),
            new PJLVariable(VarType.General, "MAINTINTERVAL"),
            new PJLVariable(VarType.General, "MANUALDUPLEX"),
            new PJLVariable(VarType.General, "MANUALFEED"),
            new PJLVariable(VarType.General, "MEDIASOURCE"),
            new PJLVariable(VarType.General, "MEDIATYPE"),
            new PJLVariable(VarType.General, "MFQBEGIN"),
            new PJLVariable(VarType.General, "MFQEND"),
            new PJLVariable(VarType.General, "MPTRAY"),
            new PJLVariable(VarType.General, "ORIENTATION"),
            new PJLVariable(VarType.General, "OUTBIN"),
            new PJLVariable(VarType.General, "OUTBINPROCESS"),
            new PJLVariable(VarType.General, "OUTLINEPOINTSIZE"),
            new PJLVariable(VarType.General, "OUTTONER"),
            new PJLVariable(VarType.General, "PAGEPROTECT"),
            new PJLVariable(VarType.General, "PAGES"),
            new PJLVariable(VarType.General, "PAPER"),
            new PJLVariable(VarType.General, "PARALLEL"),
            new PJLVariable(VarType.General, "PASSWORD"),
            new PJLVariable(VarType.General, "PERSONALITY"),
            new PJLVariable(VarType.General, "PLANESINUSE"),
            new PJLVariable(VarType.General, "POWERSAVE"),
            new PJLVariable(VarType.General, "POWERSAVEMODE"),
            new PJLVariable(VarType.General, "POWERSAVETIME"),
            new PJLVariable(VarType.General, "PR1200SPEED"),
            new PJLVariable(VarType.General, "PRINTONBACKSIDE"),
            new PJLVariable(VarType.General, "PRINTQUALITY"),
            new PJLVariable(VarType.General, "PROCESSINGBOUNDARY"),
            new PJLVariable(VarType.General, "PROCESSINGOPTION"),
            new PJLVariable(VarType.General, "PROCESSINGTYPE"),
            new PJLVariable(VarType.General, "QTY"),
            new PJLVariable(VarType.General, "RENDERMODE"),
            new PJLVariable(VarType.General, "REPRINT"),
            new PJLVariable(VarType.General, "RESOLUTION"),
            new PJLVariable(VarType.General, "RESOURCESAVE"),
            new PJLVariable(VarType.General, "RESOURCESAVESIZE"),
            new PJLVariable(VarType.General, "RET"),
            new PJLVariable(VarType.General, "SCAN"),
            new PJLVariable(VarType.General, "STAPLEOPTION"),
            new PJLVariable(VarType.General, "STRINGCODESET"),
            new PJLVariable(VarType.General, "TESTPAGE"),
            new PJLVariable(VarType.General, "TIMEOUT"),
            new PJLVariable(VarType.General, "TRAY1TEMP"),
            new PJLVariable(VarType.General, "TRAY2TEMP"),
            new PJLVariable(VarType.General, "TRAY3TEMP"),
            new PJLVariable(VarType.General, "USERNAME"),
            new PJLVariable(VarType.General, "WIDEA4"),
            //----------------------------------------------------------------//
            new PJLVariable(VarType.PCL, "FONTNUMBER"),
            new PJLVariable(VarType.PCL, "FONTSOURCE"),
            new PJLVariable(VarType.PCL, "LINETERMINATION"),
            new PJLVariable(VarType.PCL, "PITCH"),
            new PJLVariable(VarType.PCL, "PTSIZE"),
            new PJLVariable(VarType.PCL, "RESOURCESAVESIZE"),
            new PJLVariable(VarType.PCL, "SYMSET"),
            //----------------------------------------------------------------//
            new PJLVariable(VarType.PDF, "OWNERPASSWORD"),
            new PJLVariable(VarType.PDF, "USERPASSWORD"),
            //----------------------------------------------------------------//
            new PJLVariable(VarType.PS, "ADOBEMBT"),
            new PJLVariable(VarType.PS, "JAMRECOVERY"),
            new PJLVariable(VarType.PS, "PRTPSERRS"),
            new PJLVariable(VarType.PS, "RESOURCESAVESIZE")
        };

        private static readonly int _varCount = _vars.GetUpperBound(0) + 1;

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t C o u n t                                                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return count of Variable definitions.                              //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static int GetCount()
        {
            return _varCount;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t N a m e                                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return name associated with specified variable.                    //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static string GetName(int selection)
        {
            return _vars[selection].getName();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t T y p e                                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return type of variable.                                           //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static VarType GetType(int selection)
        {
            return _vars[selection].getType();
        }
    }
}