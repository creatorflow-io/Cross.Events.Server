using MediatR;
using Juice;
using System.Net;

namespace Cross.TcpServer.Core
{
	/// <summary>
	/// Represents a request to process a TCP message.
	/// </summary>
	/// <param name="ClientEndpoint"></param>
	/// <param name="Data"></param>
	/// <param name="ReceivedTime"></param>
	/// Consider using <see cref="INotification"/> instead
	public record TcpMessageRequest(IPEndPoint? ClientEndpoint, string Data, DateTimeOffset ReceivedTime) : IRequest<IOperationResult>;
}
