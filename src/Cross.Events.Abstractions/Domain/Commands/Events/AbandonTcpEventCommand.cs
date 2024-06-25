using Juice;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cross.Events.Domain.Commands.Events
{
    public class AbandonTcpEventCommand : IRequest<IOperationResult>
    {
        public string Id { get; private set; }

        public AbandonTcpEventCommand(string id)
        {
            Id = id;
        }
    }
}
