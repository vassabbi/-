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
	class Server
	{
		User myUser;
		List<ConnectedUser> users = new List<ConnectedUser>();
		UdpClient nameReceiver = new UdpClient();
		int udpPort = 8754;
		int tcpPort = 8755;

		public Server(User user)
		{
			this.myUser = user;
			Thread listenThread = new Thread(new ThreadStart(Listen));
			listenThread.Start();
			Thread nameReciveThread = new Thread(new ThreadStart(ReceiveNewUsers));
			nameReciveThread.Start();
		}

		void ReceiveNewUsers()
		{
			nameReceiver = new UdpClient(udpPort);
			nameReceiver.EnableBroadcast = true;
			IPEndPoint remoteIp = null;
			try
			{
				while (true)
				{
					byte[] data = nameReceiver.Receive(ref remoteIp);
					string name = Encoding.UTF8.GetString(data);
					if (!IsIpAdressMy(remoteIp.Address))
					{
						users.Add(new ConnectedUser(remoteIp.Address, name, myUser));
						Console.WriteLine($"{DateTime.Now} {name}({remoteIp.Address}) подключился", name);
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			finally
			{
				nameReceiver.Close();
			}
		}

		bool IsIpAdressMy(IPAddress iPAddress)
		{
			foreach (var myIPAdress in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
			{
				if (iPAddress.Equals(myIPAdress))
				{
					return true;
				}
			}

			return false;
		}

		void Listen()
		{
			TcpListener listener = null;
			try
			{
				listener = new TcpListener(IPAddress.Any, tcpPort);
				listener.Start();
				while (true)
				{
					var tcpClient = listener.AcceptTcpClient();
					users.Add(new ConnectedUser(tcpClient, myUser));
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
			}
			finally
			{
				if (listener != null)
				{
					listener.Stop();
				}
			}
		}

		public void Disconnect(ConnectedUser connectedUser)
		{
			if (users.Contains(connectedUser))
			{
				Console.WriteLine($"{DateTime.Now} {connectedUser.Name}({connectedUser.Ip}) покинул чат");
				users.Remove(connectedUser);
			}
		}

		public void SendAllUsers(byte[] data)
		{
			foreach (var user in users)
			{
				user.WriteToStream(data);
			}
		}
	}
}
