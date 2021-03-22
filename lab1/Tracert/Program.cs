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
            string ipAddress = Console.ReadLine();
            //ipAddress = "142.250.186.46";
            int port = 8765;
            try
            {
                IPHostEntry ipHost = Dns.GetHostEntry(ipAddress);
                IPAddress ipAddr = ipHost.AddressList[0];
                IPEndPoint ipPoint = new IPEndPoint(ipAddr, port);
                EndPoint eipPoint = ipPoint;
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Raw, ProtocolType.Icmp);
                Console.WriteLine("Трассировка маршрута к [" + ipAddress + "]");
                Console.WriteLine("с максимальным числом прыжков 256");

                ICMP packet = new ICMP();
                packet.Type = 8;
                packet.Code = 0;
                packet.CheckSum = 0;
                Buffer.BlockCopy(BitConverter.GetBytes(1), 0, packet.Message, 0, 2);
                Buffer.BlockCopy(BitConverter.GetBytes(1), 0, packet.Message, 2, 2);
                data = Encoding.ASCII.GetBytes("test packet");
                Buffer.BlockCopy(data, 0, packet.Message, 4, data.Length);
                packet.MessageSize = data.Length + 4;
                int packetsize = packet.MessageSize + 4;
                UInt16 chcksum = packet.getChecksum();
                packet.CheckSum = chcksum;

                socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 3000);

                int badcount = 0;
                for (int i = 1; i < 256; i++)
                {
                    bool finishTracing = false;
                    int badConnect = 0;
                    EndPoint ipNowAddress = eipPoint;
                    Console.Write("{0, 4}", i);
                    for (int j = 0; j < 3; j++)
                    {
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
                        badcount = 0;
                    }
                    else
                    {
                        Console.WriteLine("  Превышен интервал ожидания для запроса.");
                        badcount++;
                    }
                    if (finishTracing)
                    {
                        Console.WriteLine();
                        Console.WriteLine("Трассировка завершена.");
                        Console.WriteLine();
                        break;
                    }
                    if (badcount == 5)
                    {
                        Console.WriteLine();
                        Console.WriteLine("Не удалось установить соединение");
                        Console.WriteLine();
                    }
                }
                socket.Close();
            }
            catch
            {
                Console.WriteLine("Неверно указан ip для доставки");
            }
        }
    }

}
