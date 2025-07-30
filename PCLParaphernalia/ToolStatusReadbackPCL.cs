using System.IO;

namespace PCLParaphernalia;

/// <summary>
/// 
/// Class provides PCL support for the PrinterInfo tool.
/// 
/// © Chris Hutchinson 2010
/// 
/// </summary>

static class ToolStatusReadbackPCL
{
    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // g e n e r a t e R e q u e s t                                      //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Generate status readback request data.                             //
    //                                                                    //
    //--------------------------------------------------------------------//

    public static void GenerateRequest(BinaryWriter prnWriter,
                                       int indexEntity,
                                       int indexLocType)
    {
        string seq;

        PCLEntityTypes.eType entityType;

        string entityIdPCL;
        string locTypeIdPCL;

        if (indexEntity < PCLEntityTypes.GetCount())
        {
            entityType = PCLEntityTypes.GetType(indexEntity);
            entityIdPCL = PCLEntityTypes.GetIdPCL(indexEntity);
            locTypeIdPCL = PCLLocationTypes.GetIdPCL(indexLocType);

            if (entityType == PCLEntityTypes.eType.Memory)
            {
                seq = "\x1b" + "*s" +
                               entityIdPCL +
                               "M";         // entity = memory
            }
            else
            {
                seq = "\x1b" + "*s" +
                               locTypeIdPCL +
                               "t" +       // loc. type 
                               "0u" +       // loc. unit = all
                               entityIdPCL +
                               "I";         // entity
            }

            prnWriter.Write(seq.ToCharArray(), 0, seq.Length);
        }
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // r e a d R e s p o n s e                                            //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Read response from target.                                         //
    //                                                                    //
    //--------------------------------------------------------------------//

    public static string ReadResponse()
    {
        const int replyBufLen = 32768;

        byte[] replyData = new byte[replyBufLen];

        int replyLen = 0;

        //  Boolean readFF_A = true;            // only one <FF> expected //
        bool OK = false;
        bool replyComplete = false;

        int offset = 0;
        int endOffset = 0;
        int bufRem = replyBufLen;
        int blockLen = 0;

        while (!replyComplete)
        {
            OK = TargetCore.ResponseReadBlock(offset,
                                               bufRem,
                                               ref replyData,
                                               ref blockLen);

            endOffset = offset + blockLen;

            if (!OK)
            {
                replyComplete = true;
            }
            /*else if (!readFF_A)
            {
                //--------------------------------------------------------//
                //                                                        //
                // Not yet found any <FF> bytes.                          //
                // Search buffer to see if first, or both first and       //
                // second <FF> (as applicable) are present.               //
                //                                                        //
                // This branch will never be entered unless we include a  //
                // PJL ECHO commmand in the job header; included in case  //
                // we ever do this.                                       //
                //                                                        //
                //--------------------------------------------------------//

                for (Int32 i = offset; i < endOffset; i++)
                {
                    if (replyData[i] == 0x0c)
                    {
                        if ((readFF_A) && (replyData[endOffset - 1] == 0x0c))
                            replyComplete = true;
                        else
                            readFF_A = true;
                    }
                }
            }*/
            else
            {
                //--------------------------------------------------------//
                //                                                        //
                // <FF> at end of ECHO text is either not expected, or    //
                // has been read in a previous read action.               // 
                //                                                        //
                //--------------------------------------------------------//

                if (replyData[endOffset - 1] == 0x0c)
                {
                    //----------------------------------------------------//
                    //                                                    //
                    // Terminating <FF> found (as last byte of data       //
                    // returned by current read action).                  // 
                    //                                                    //
                    //----------------------------------------------------//

                    replyComplete = true;
                }
            }

            offset += blockLen;
            bufRem -= blockLen;

            if (bufRem <= 0)
                replyComplete = true;
        }

        replyLen = endOffset;

        TargetCore.ResponseCloseConnection();

        return System.Text.Encoding.ASCII.GetString(replyData,
                                                     0,
                                                     replyLen);
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // s e n d R e q u e s t                                              //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Send generated status readback request data to target.             //
    //                                                                    //
    //--------------------------------------------------------------------//

    public static void SendRequest()
    {
        TargetCore.RequestStreamWrite(true);
    }
}
