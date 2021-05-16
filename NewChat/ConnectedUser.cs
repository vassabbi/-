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
	class ConnectedUser
	{
		User user;
		public IPAddress Ip { get; private set; }
		public string Name { get; private set; }

		TcpClient tcpClient;
		NetworkStream stream = null;
		int tcpPort = 8755;

		public ConnectedUser(IPAddress ip, string name, User user)
		{
			Ip = ip;
			Name = name;
			this.user = user;
			var iPEndPoint = new IPEndPoint(Ip, tcpPort);
			tcpClient = new TcpClient();
			tcpClient.Connect(iPEndPoint);
			Thread userThread = new Thread(new ThreadStart(Listen));
			userThread.Start();
		}

		public ConnectedUser(TcpClient tcpClient, User user)
		{
			this.tcpClient = tcpClient;
			this.user = user;
			Ip = IPAddress.Parse(tcpClient.Client.RemoteEndPoint.ToString().Split(':')[0]);
			Name = "undef";
			Thread userThread = new Thread(new ThreadStart(Listen));
			userThread.Start();
		}

		void Listen()
		{
			try
			{
				stream = tcpClient.GetStream();
				var name = (new MessagePacket(0, user.Name)).GetBytes();
				stream.Write(name, 0, name.Length);
				while (true)
				{
					var packet = new MessagePacket(GetMessage());
					if (packet.Type == 0)
					{
						Name = packet.Content;
					}
					else
					{
						Console.WriteLine($"{Name}({Ip}) : {packet.Content}");
					}
				}
			}
			catch (Exception e)
			{
			}
			finally
			{
				if (stream != null)
				{
					stream.Close();
				}

				tcpClient.Close();
				user.Server.Disconnect(this);
			}
		}

		byte[] GetMessage()
		{
			byte[] data = new byte[64];
			var packet = new List<byte>();
			StringBuilder builder = new StringBuilder();
			int bytes = 0;
			do
			{
				bytes = stream.Read(data, 0, data.Length);
				packet.AddRange(data.Take(bytes));
			}
			while (stream.DataAvailable);

			return packet.ToArray();
		}

		public void WriteToStream(byte[] data)
		{
			if (stream != null)
			{
				stream.Write(data, 0, data.Length);
			}
		}
	}
}
