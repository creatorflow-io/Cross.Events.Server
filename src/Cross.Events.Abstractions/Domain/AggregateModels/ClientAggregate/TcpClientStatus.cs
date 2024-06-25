using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cross.Events.Domain.AggregateModels.ClientAggregate
{
	public enum TcpClientStatus
	{
		New,
		Accepted,
		Banned
	}
}
