using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cross.Events.Api.Authorization
{
	public static class Policies
	{
		public const string EventReadPolicy = "EventRead";
		public const string EventContributePolicy = "EventContribute";
		public const string EventAdminPolicy = "EventAdmin";
		public const string ClientContributePolicy = "ClientContribute";
	}
}
