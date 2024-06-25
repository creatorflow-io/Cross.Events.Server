using System.Diagnostics.Metrics;
using System.Net;

namespace Cross.TcpServer.Core.Metrics
{
	public class TcpServerMetrics
	{
		private Counter<int> _totalMessagesClientCounter;
		private Counter<int> _totalMessagesCounter;
		private Counter<int> _totalErrorsCounter;
		private Counter<int> _totalUniqueClientsCounter;
		private UpDownCounter<int> _handlingRequestsCounter;

		private HashSet<string> _uniqueClients = new HashSet<string>();

		public TcpServerMetrics(IMeterFactory meterFactory)
		{
			var meter = meterFactory.Create("Cross.Tcpserver");
			_totalMessagesClientCounter = meter.CreateCounter<int>("tcpserver.messages.client_total", "Total messages received by client");
			_totalMessagesCounter = meter.CreateCounter<int>("tcpserver.messages.total", "Total messages received");
			_totalErrorsCounter = meter.CreateCounter<int>("tcpserver.errors.total", "Total errors occurred");
			_totalUniqueClientsCounter = meter.CreateCounter<int>("tcpserver.clients.unique_total", "Total clients connected");
			_handlingRequestsCounter = meter.CreateUpDownCounter<int>("tcpserver.messages.requests", "Requests handling");
		}

		public void IncrementTotalMessages(IPEndPoint? endPoint)
		{
			_totalMessagesCounter.Add(1);
			if (endPoint != null)
			{
				_totalMessagesClientCounter.Add(1, new KeyValuePair<string, object?>("client.ipaddress", endPoint.Address));
			}
		}

		public void IncrementTotalErrors()
		{
			_totalErrorsCounter.Add(1);
		}

		public void IncrementTotalClients(IPEndPoint? endPoint)
		{
			if (endPoint != null && _uniqueClients.Add(endPoint.Address.ToString()))
			{
				_totalUniqueClientsCounter.Add(1);
			}
		}

		public void IncrementHandlingRequests()
		{
			_handlingRequestsCounter.Add(1);
		}

		public void DecrementHandlingRequests()
		{
			_handlingRequestsCounter.Add(-1);
		}

	}
}
