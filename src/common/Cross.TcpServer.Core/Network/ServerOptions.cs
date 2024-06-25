using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cross.TcpServer.Core.Network
{
	public class ServerOptions
	{
		public int BufferSize { get; set; } = 4096;
		public int Port { get; set; } = 5000;
		public int MaxConnections { get; set; } = 100;
	}
}
