using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace InterComm.Helpers
{
    public static class NetHelper
    {
        public static bool IsConnectionAlive()
        {
            try
            {
                TcpClient clnt=new TcpClient("www.google.com",80);
                clnt.Close();
                return true;
            }
            catch(System.Exception ex)
            {
                Logger.LogException(ex);
                return false;
            }
        }
    }
}
