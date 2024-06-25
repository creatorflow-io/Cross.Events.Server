using Cross.Events.Domain.AggregateModels.EventAggregate;
using Cross.Events.Domain.Commands.Events;
using Juice;
using MediatR;

namespace Cross.Events.Api.Domain.CommandHandlers
{
    internal class CreateTcpEventCommandHandler : IRequestHandler<CreateTcpEventCommand, OperationResult<string>>
	{
		private IRepository<TcpEvent, string> _repository;
		public CreateTcpEventCommandHandler(IRepository<TcpEvent, string> repository)
		{
			_repository = repository;
		}

		public async Task<OperationResult<string>> Handle(CreateTcpEventCommand request, CancellationToken cancellationToken)
		{
			try
			{
				var @event = new TcpEvent(request.Timestamp, request.Data);
				await _repository.InsertAsync(@event);
				return OperationResult.Result(@event.Id);
			}catch(Exception ex)
			{
				return OperationResult.Failed<string>(ex.Message);
			}
		}
	}
}
