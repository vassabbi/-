using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace NewChat
{
    static class NetInterfaces
    {
        public static string Search()
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
                            return el.Address.ToString();
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
    }
}
