using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Lab3
{
    class Program
    {
        const int maxSize = 1024 * 20;
        readonly static int port = 8888;
        static BlackList blackList = new BlackList();
        private static byte[] ReadFromStream(NetworkStream stream)
        {
            byte[] data = new byte[maxSize];
            byte[] builder = new byte[maxSize];
            int builderBytes = 0;
            do
            {
                int bytes = stream.Read(data, 0, data.Length);
                Array.Copy(data, 0, builder, builderBytes, bytes);
                builderBytes += bytes;
            } while (stream.DataAvailable && builderBytes < maxSize);

            return builder;
        }
        private static void ExecuteRequest(object arg)
        {
            using Socket requestSocket = (Socket)arg;
            NetworkStream streamReq = new NetworkStream(requestSocket);
            if (requestSocket.Connected)
            {
 
                byte[] httpRequest = ReadFromStream(streamReq);
                HttpHeader http = new HttpHeader(httpRequest);
                //Console.WriteLine(http.Source);
                try
                {
                    IPHostEntry ipHostEntry = Dns.GetHostEntry(http.Host);
                    if (blackList.isForbitten(http.Path))
                    {
                        Console.WriteLine("{0} находится в чёрном списке", http.Host);
                        string errorMessage = "HTTP/1.1 403 Forbidden\nContent-Type: text/html\r\nContent-Length: 100\n\n" + http.Host + " is in black list";
                        streamReq.Write(Encoding.ASCII.GetBytes(errorMessage));
                    }
                    else
                    {
                        IPEndPoint endPoint = new IPEndPoint(ipHostEntry.AddressList[0], http.Port);
                        using (Socket receiver = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
                        {                            
                            receiver.Connect(endPoint);
                            NetworkStream streamRec = new NetworkStream(receiver);
                            streamRec.Write(httpRequest, 0, httpRequest.Length);
                            byte[] httpResponse = ReadFromStream(streamRec);
                            streamReq.Write(httpResponse, 0, httpResponse.Length);
                            HttpHeader Response = new HttpHeader(httpResponse);
                            Console.WriteLine($"{http.Path} {Response.StatusCode} {Response.StatusMessage}");
                            streamRec.CopyTo(streamReq);
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("{0} не является известным хостом {1}", http.Host, e.Message);
                }
            }

        }
        static void Main()
        {
            IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
            Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            { 
                listener.Bind(ipPoint);
                listener.Listen(30);
                while (true)
                {
                    Thread newReq = new Thread(ExecuteRequest);
                    newReq.IsBackground = true;
                    newReq.Start(listener.Accept());
                }
            }
            catch
            {
                listener.Close();
                Console.WriteLine("Соединение с клиентом было разорвано");
            }
        }
    }
}
