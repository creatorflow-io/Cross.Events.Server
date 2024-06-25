using Juice;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cross.Events.Domain.Commands.Events
{
    /// <summary>
    /// Create TcpEvent Command
    /// </summary>
    public class CreateTcpEventCommand : IRequest<OperationResult<string>>
    {
        public DateTimeOffset Timestamp { get; private set; }
        public string Data { get; private set; }

        public CreateTcpEventCommand(DateTimeOffset timestamp, string data)
        {
            Timestamp = timestamp;
            Data = data;
        }
    }
}
