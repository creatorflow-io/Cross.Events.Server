using System;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Net;
using System.Text;

namespace Cross.Events.Api.Mertics
{
    internal class EventMetrics
    {
        private Counter<int> _eventsCounter;
        private Counter<int> _eventsLimited;

        public EventMetrics(IMeterFactory meterFactory)
        {
            var meter = meterFactory.Create("Cross.Events");
            _eventsCounter = meter.CreateCounter<int>("events.total", "Total events received");
            _eventsLimited = meter.CreateCounter<int>("events.limited", "Total events limited");
        }

        public void IncrementEvents(int count, EndPoint? endpoint)
        {
            if (endpoint != null && endpoint is IPEndPoint ipEndPoint)
            {
                _eventsCounter.Add(count, new KeyValuePair<string, object?>("client.ipaddress", ipEndPoint.Address));
            }
        }

        public void IncrementEventsLimited(int count, EndPoint? endpoint)
        {
            if (endpoint != null && endpoint is IPEndPoint ipEndPoint)
            {
                _eventsLimited.Add(count, new KeyValuePair<string, object?>("client.ipaddress", ipEndPoint.Address));
            }
        }
    }
}
