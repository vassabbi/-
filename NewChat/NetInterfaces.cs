using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace NewChat
{
    class NetInterfaces
    {
        public UnicastIPAddressInformation information;
        public NetInterfaces()
        {
            int i = 0;
            var listIP = new List<UnicastIPAddressInformation>();
            foreach (var NetIn in NetworkInterface.GetAllNetworkInterfaces())
            {
                foreach (UnicastIPAddressInformation ip in NetIn.GetIPProperties().UnicastAddresses)
                {
                    if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork) {
                        i++;
                        Console.WriteLine("{0}. {1}", i, ip.Address.ToString());
                        listIP.Add(ip);
                    }
                }
            }
            while (true) {
                try
                {
                    Console.WriteLine("Введите номер ip, который вы хотите использовать");
                    int num = int.Parse(Console.ReadLine());
                    i = 0;
                    foreach (var el in listIP)
                    {
                        i++;
                        if (i == num)
                        {
                            Console.WriteLine("Выбран {0}", el.Address.ToString());
                            information = el;
                            return;
                        }
                    }
                    throw new Exception("Not Found");
                }
                catch
                {
                    Console.WriteLine("Вы непрвильно выбрали, номер ip, начните заново");
                }
            }
        }
        public IPAddress BroadCastIP()
        {
            string Address = information.Address.ToString();
            string Mask = information.IPv4Mask.ToString();
            string[] pAddress = Address.Split('.');
            string[] pMask = Mask.Split('.');
            string newAddress = "";
            for (int i = 0; i < 4; i++)
            {
                if (pMask[i] == "0")
                {
                    newAddress += "255";
                }
                else
                {
                    newAddress += pAddress[i];
                }
                if (i != 3)
                {
                    newAddress += ".";
                }
            }
            return IPAddress.Parse(newAddress);
        }
    }
}
