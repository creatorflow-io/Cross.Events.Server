using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cross.MongoDb.Test
{
	internal class SharedService
	{
		public HashSet<string> EventHandlers { get; } = new HashSet<string>();
	}
}
