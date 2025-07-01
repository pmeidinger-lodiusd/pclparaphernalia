using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Windows;

namespace PCLParaphernalia
{
    /// <summary>
    /// 
    /// Class provides the network printer Target functions.
    /// 
    /// © Chris Hutchinson 2010
    /// 
    /// </summary>

    static class TargetNetPrint
    {
        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Fields (class variables).                                          //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static Socket _socket;

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c h e c k I P A d d r e s s                                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Check that the provided string represents a valid (ipv4 or ipv6)   //
        // address.                                                           //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static bool checkIPAddress(string ipString,
                                             ref IPAddress ipAddress)
        {
            bool OK;

            OK = IPAddress.TryParse(ipString, out ipAddress);

            return OK;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c l o s e R e s p o n s e C o n n e c t i o n                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Close connection (after having read response block(s)).            //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void closeResponseConnection()
        {
            _socket.Close();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // r e a d R e s p o n s e B l o c k                                  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Read response block into supplied buffer.                          //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static bool readResponseBlock(int offset,
                                                 int bufRem,
                                                 ref byte[] replyData,
                                                 ref int blockLen)
        {
            int readLen = 0;

            bool OK = true;

            try
            {
                readLen = _socket.Receive(replyData,
                                           offset,
                                           bufRem,
                                           SocketFlags.None);

                blockLen = readLen;
            }

            //----------------------------------------------------------------//

            catch (SocketException e)
            {
                MessageBox.Show("SocketException" +
                                 "Message: " + e.Message + "\n\n" +
                                 "ErrorCode: " + e.ErrorCode + "\n\n" +
                                 "SocketErrorCode: " + e.SocketErrorCode,
                                 "Printer output",
                                 MessageBoxButton.OK,
                                 MessageBoxImage.Exclamation);

                OK = false;
            }

            catch (Exception e)
            {
                MessageBox.Show("Exception: " + e.ToString(),
                                "Printer output",
                                MessageBoxButton.OK,
                                MessageBoxImage.Exclamation);

                OK = false;
            }

            return OK;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s e n d D a t a                                                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Send data in provided stream to specified printer port.            //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static int sendData(BinaryReader prnReader,
                                     string ipString,
                                     int port,
                                     int timeoutSend,
                                     int timeoutReceive,
                                     bool keepConnect)
        {
            int result = 0;

            IPAddress ipAddress = new IPAddress(0x00);

            if (checkIPAddress(ipString, ref ipAddress))
            {
                //------------------------------------------------------------//
                //                                                            //
                // Address format is valid.                                   //
                //                                                            //
                //------------------------------------------------------------//

                try
                {
                    //--------------------------------------------------------//
                    //                                                        //
                    // Open and connect socket.                               //
                    //                                                        //
                    //--------------------------------------------------------//

                    IPEndPoint ipEndPoint;

                    int readLen;
                    int sockRes;

                    ipEndPoint = new IPEndPoint(ipAddress,
                                                port);

                    _socket = new Socket(ipAddress.AddressFamily,
                                          SocketType.Stream,
                                          ProtocolType.Tcp);

                    _socket.SendTimeout = timeoutSend;
                    _socket.ReceiveTimeout = timeoutReceive;

                    // or   _socket.SetSocketOption (SocketOptionLevel.Socket,
                    //                               SocketOptionName.SendTimeout, 1000);

                    _socket.Connect(ipEndPoint);

                    //--------------------------------------------------------//
                    //                                                        //
                    // Read contents of supplied stream and send via socket.  //
                    //                                                        //
                    //--------------------------------------------------------//

                    const int bufLen = 512;

                    byte[] prnData = new byte[bufLen];

                    prnReader.BaseStream.Position = 0;
                    prnData = prnReader.ReadBytes(bufLen);

                    while ((readLen = prnData.Length) != 0)
                    {
                        sockRes = _socket.Send(prnData, readLen, 0);

                        prnData = prnReader.ReadBytes(bufLen);
                    }

                    //--------------------------------------------------------//
                    //                                                        //
                    // Terminate connection, unless it is to be kept open     //
                    // (for subsequent read response action.                  //
                    //                                                        //
                    //--------------------------------------------------------//

                    if (!keepConnect)
                        _socket.Close();
                }

                //------------------------------------------------------------//

                catch (SocketException e)
                {
                    MessageBox.Show("SocketException:\n\n" +
                                     "Message: " + e.Message + "\n\n" +
                                     "ErrorCode: " + e.ErrorCode + "\n\n" +
                                     "SocketErrorCode: " + e.SocketErrorCode,
                                     "Printer output",
                                     MessageBoxButton.OK,
                                     MessageBoxImage.Exclamation);
                }

                catch (Exception e)
                {
                    MessageBox.Show("Exception:\n\n" + e.ToString(),
                                    "Printer output",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Exclamation);
                }
            }

            return result;
        }
    }
}
