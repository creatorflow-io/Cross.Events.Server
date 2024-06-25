using Cross.TcpServer.Core.Network;
using Juice.BgService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cross.TcpServer.Core
{
	public class TcpServiceModel : IServiceModel
	{
		public Guid? Id { get; set; }

		public string Name { get; set; }

		public Dictionary<string, object?> Options { get; set; } = [];

		public string AssemblyQualifiedName { get; set; }

		public ServerOptions? ServerOptions { get; set; }
	}
}
