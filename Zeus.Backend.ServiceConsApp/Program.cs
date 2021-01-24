using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Zeus.Backend.ServiceConsApp
{
    class Program
    {
        static int Main(string[] args)
        {
            //AsynchronousSocketListener.StartListening();
            new MyTcpListener();
            return 0;
        }
    }
}
