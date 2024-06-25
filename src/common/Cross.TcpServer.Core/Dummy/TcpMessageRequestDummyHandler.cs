using Juice;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cross.TcpServer.Core.Dummy
{
	internal class TcpMessageRequestDummyHandler : IRequestHandler<TcpMessageRequest, IOperationResult>
	{
		public Task<IOperationResult> Handle(TcpMessageRequest request, CancellationToken cancellationToken)
		{
			return Task.FromResult<IOperationResult>(OperationResult.Success);
		}
	}
}
