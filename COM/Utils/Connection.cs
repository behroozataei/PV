using System;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Text;

namespace COM
{
    public static class  Connection
    {
        public static bool PingHost(string hostUri, int portNumber)
        {
            try
            {
                using (var client = new TcpClient(hostUri, portNumber))
                    return true;
            }
            catch (SocketException ex)
            {
                return false;
            }
        }
    }
}
