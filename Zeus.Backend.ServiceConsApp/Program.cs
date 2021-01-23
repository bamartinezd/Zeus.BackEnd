using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Zeus.Backend.ServiceConsApp
{
    class Program
    {
        static Encoding enc = Encoding.UTF8;
        static void Main(string[] args)
        {

            Console.WriteLine("Escuchando...");

            IPAddress ip = Dns.GetHostEntry("192.168.0.2").AddressList[0];
            TcpListener listener = new TcpListener(ip, 5055);
            listener.Start();

            while (true)
            {
                TcpClient client = listener.AcceptTcpClient();
                Console.WriteLine("Peticion entrante...");

                NetworkStream stream = client.GetStream();
                
                //Conversion de la peticion a texto
                MemoryStream memoryStream = new MemoryStream();
                byte[] data = new byte[256];
                int size;
                do
                {
                    size = stream.Read(data, 0, data.Length);
                    if (size == 0)
                    {
                        Console.WriteLine("client disconnected...");
                    }
                    memoryStream.Write(data, 0, size);
                } while (stream.DataAvailable);
                string request = enc.GetString(memoryStream.ToArray());

                Console.WriteLine("req -> " + request);

                string response = "4c4f4144";
                Console.WriteLine("res -> " + response);

                byte[] sendBytes = enc.GetBytes(response);
                stream.Write(sendBytes, 0, sendBytes.Length);

                stream.Close();
                client.Close();
            }
        }
    }
}
