using Cross.Events.Domain.AggregateModels.EventAggregate;
using Cross.Events.Domain.Commands.Events;
using Juice;
using Juice.Domain;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Cross.Events.Api.Domain.CommandHandlers
{
    internal class AbandonTcpEventCommandHandler : IRequestHandler<AbandonTcpEventCommand, IOperationResult>
	{
		private IRepository<TcpEvent, string> _repository;
		private IHttpContextAccessor _httpContextAccessor;
		public AbandonTcpEventCommandHandler(IRepository<TcpEvent, string> repository, IHttpContextAccessor httpContextAccessor)
		{
			_repository = repository;
			_httpContextAccessor = httpContextAccessor;
		}

		public async Task<IOperationResult> Handle(AbandonTcpEventCommand request, CancellationToken cancellationToken)
		{
			try
			{
				var tcpEvent = await _repository.GetByIdAsync(request.Id);
				if (tcpEvent == null)
				{
					return (OperationResult.Failed("TcpEvent not found"));
				}
				tcpEvent.Abandon(_httpContextAccessor.HttpContext?.User?.Identity?.Name);
				tcpEvent.ThrowIfHasErrors();
				await _repository.UpdateAsync(tcpEvent);
				return (OperationResult.Success);
			}
			catch (Exception ex)
			{
				return (OperationResult.Failed(ex.Message));
			}
		}
	}
}
