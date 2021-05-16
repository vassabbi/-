using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Linq;

namespace NewChat
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Введите имя пользователя");
			var user = new User(Console.ReadLine());
		}
	}
}