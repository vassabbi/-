using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Tracert
{
    class Program
    {
        static void Main(string[] args)
        {
            int recv = 0;
            byte[] data = new byte[1024];
            Console.WriteLine("Введите ip для доставки");
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Raw, ProtocolType.Icmp);
            string ipAddress = Console.ReadLine();
            try
            {
                IPHostEntry ipHost = Dns.GetHostEntry(ipAddress);
                IPAddress ipAddr = ipHost.AddressList[0];
                IPEndPoint ipPoint = new IPEndPoint(ipAddr, 0);
                EndPoint eipPoint = ipPoint;
                Console.WriteLine("Трассировка маршрута к [" + ipAddress + "]");
                Console.WriteLine("с максимальным числом прыжков 30");
                int seqNumber = 1;

                ICMP packet = new ICMP();
                packet.Type = 8;
                packet.Code = 0;
                packet.CheckSum = 0;
                Buffer.BlockCopy(BitConverter.GetBytes(1), 0, packet.Message, 0, 2);
                Buffer.BlockCopy(BitConverter.GetBytes(seqNumber << 8), 0, packet.Message, 2, 2);
                data = Encoding.ASCII.GetBytes("test packet");
                Buffer.BlockCopy(data, 0, packet.Message, 4, data.Length);
                packet.MessageSize = data.Length + 4;
                int packetsize = packet.MessageSize + 4;
                packet.CheckSum = packet.getChecksum();

                bool finishTracing = false;
                socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 3000);
                for (int i = 1; i <= 30; i++)
                {
                    int badConnect = 0;
                    EndPoint ipNowAddress = eipPoint;
                    Console.Write("{0, 4}", i);
                    for (int j = 0; j < 3; j++)
                    {
                        Buffer.BlockCopy(BitConverter.GetBytes(seqNumber << 8), 0, packet.Message, 2, 2);
                        packet.CheckSum = 0;
                        packet.CheckSum = packet.getChecksum();
                        socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.IpTimeToLive, i);
                        socket.SendTo(packet.getBytes(), packetsize, SocketFlags.None, ipPoint);

                        try
                        {
                            data = new byte[1024];
                            DateTime timestart = DateTime.Now;
                            recv = socket.ReceiveFrom(data, ref eipPoint);
                            ipNowAddress = eipPoint;
                            TimeSpan timestop = DateTime.Now - timestart;
                            ICMP response = new ICMP(data, recv);
                            if (response.Type == 11 || response.Type == 0)
                            {
                                Console.Write("{0, 9}", timestop.Milliseconds.ToString() + " ms");
                                if (response.Type == 0)
                                {
                                    finishTracing = true;
                                }
                            }
                        }
                        catch (SocketException)
                        {
                            Console.Write("{0, 9}", "*   ");
                            badConnect++;
                        }
                        seqNumber++;
                    }
                    if (badConnect != 3)
                    {
                        string onlyipNowAddress = ipNowAddress.ToString();
                        onlyipNowAddress = onlyipNowAddress.Split(":")[0];
                        try
                        {
                            IPAddress addr = IPAddress.Parse(onlyipNowAddress);
                            IPHostEntry entry = Dns.GetHostEntry(addr);
                            Console.WriteLine("  " + entry.HostName + " [" + onlyipNowAddress + "]");
                        }
                        catch
                        {
                            Console.WriteLine("  [" + onlyipNowAddress + "]");
                        }
                    }
                    else
                    {
                        Console.WriteLine("  Превышен интервал ожидания для запроса.");
                    }
                    if (finishTracing)
                    {
                        Console.WriteLine();
                        Console.WriteLine("Трассировка завершена.");
                        Console.WriteLine();
                        break;
                    }
                }
                if (!finishTracing)
                {
                    Console.WriteLine();
                    Console.WriteLine("Соединение установить не удалось");
                    Console.WriteLine();
                }
                socket.Close();
            }
            catch
            {
                socket.Close();
                Console.WriteLine("Неверно указан ip для доставки");
            }
        }
    }

}
