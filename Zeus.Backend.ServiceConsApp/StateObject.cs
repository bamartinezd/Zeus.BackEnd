using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
namespace Zeus.Backend.ServiceConsApp{
    public class StateObject{
        public const int BufferSize = 1024;
        public byte[] buffer = new byte[BufferSize];
        public StringBuilder stringBuilder=new StringBuilder();
        public Socket workSocket = null;
    }
}