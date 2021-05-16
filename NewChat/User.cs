using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NewChat
{
	class User
	{
		public string Name { get; set; }
		public IPAddress Ip { get; set; }

		IPAddress remoteAddress;// = IPAddress.Parse("255.255.255.255");
		int remotePort = 8754;
		UdpClient udpClient;

		public Server Server;

		public User(string name)
		{
			Name = name;
			var host = Dns.GetHostEntry(Dns.GetHostName());
			Ip = host.AddressList[0];
			Server = new Server(this);
			SendName();
			Thread sendMessageTread = new Thread(new ThreadStart(SendMessage));
			sendMessageTread.Start();
		}

		void SendMessage()
		{
			try
			{
				while (true)
				{
					string message = Console.ReadLine();
					
					byte[] data = (new MessagePacket(1, message)).GetBytes();
					Server.SendAllUsers(data);
					Console.SetCursorPosition(0, Console.CursorTop - 1);
					Console.WriteLine($"{DateTime.Now} Вы: {message}");
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
			}
		}

		void SendName()
		{
			udpClient = new UdpClient();
			remoteAddress = IPAddress.Parse(NetInterfaces.Search());
			IPEndPoint endPoint = new IPEndPoint(remoteAddress, remotePort);
			udpClient.EnableBroadcast = true;
			var udpMessage = Encoding.UTF8.GetBytes(Name);
			udpClient.Send(udpMessage, udpMessage.Length, endPoint);
			udpClient.Close();
		}
	}
}
