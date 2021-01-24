using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Zeus.Backend.ServiceConsApp
{
    public class AsynchronousSocketListener
    {
        public static ManualResetEvent allDone =  new ManualResetEvent(false);
        public AsynchronousSocketListener(){

        }

        public static void StartListening(){
            IPHostEntry iPHost= Dns.GetHostEntry(Dns.GetHostName());
            IPAddress iPAddress = iPHost.AddressList[0];
            IPEndPoint localEndpoint = new IPEndPoint(iPAddress,5055);

            Socket listener = new Socket(iPAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                listener.Bind(localEndpoint);
                listener.Listen(100);
                int a=0;
                while (true)
                {
                    allDone.Reset();
                    Console.WriteLine($"{a} Escuchando...");
                    listener.BeginAccept(
                        new AsyncCallback(AcceptCallBack),
                        listener
                    );
                    a++;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
            }

        }

        private static void AcceptCallBack(IAsyncResult ar)
        {
            //Señala el hilo principal para continuar
            allDone.Set();

            //Obtiene el socket que maneja la petición del cliente
            Socket listener = (Socket) ar.AsyncState;
            Socket handler = listener.EndAccept(ar);

            //Crea el objeto de estado
            StateObject state = new StateObject();
            state.workSocket = handler;
            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
        }

        private static void ReadCallback(IAsyncResult ar)
        {
            String content = String.Empty;

            //Recupera el objeto de estado y el socket del controlador
            //del objeto de estado asincrono
            StateObject state = (StateObject) ar.AsyncState;
            Socket handler = state.workSocket;

            //Lee los datos del socket cliente
            int bytesRead = handler.EndReceive(ar);

            if (bytesRead > 0)
            {
                //Es posible que haya mas datos, asi que guarde los datos recibidos
                state.stringBuilder.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));

                //Verifica el tag <EOF> si no está lee mas datos
                content = state.stringBuilder.ToString();
                if (content.IndexOf("<EOF>") > -1)
                {
                    //Imprimir la data capturada del cliente
                    Console.WriteLine("Leídos {0} bytes desde: {1}", content.Length, content);

                    //Devuelvo los datos al cliente
                    Send(handler, content);    
                }else
                {
                    //No todos los datos recibidos. Obtener más.
                    handler.BeginReceive(state.buffer, 0, StateObject.BufferSize,0, new AsyncCallback(ReadCallback), state);
                }
            }
        }

        private static void Send(Socket handler, string content)
        {
            //Se convierte la data a Bytes usando codificación ASCII.
            byte[] byteData = Encoding.ASCII.GetBytes(content);

            //Se envia la data al cliente
            handler.BeginSend(byteData, 0,byteData.Length,0, new AsyncCallback(SendCallBack), handler);
        }

        private static void SendCallBack(IAsyncResult ar)
        {
            try
            {
                //Recuperando el socket del objeto de estado
                Socket handler = (Socket) ar.AsyncState;

                //Se completa el envio de datos al cliente
                int bytesSent = handler.EndSend(ar);
                Console.WriteLine("{0} Bytes enviados al cliente.", bytesSent);

                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
            }
        }
    }
}