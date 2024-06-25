using Cross.Attibutes;
using Juice.Domain;
using MediatR;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cross.Events.Domain.AggregateModels.ClientAggregate
{
	[MongoCollection("TcpClients")]
	[BsonIgnoreExtraElements]
	public class TcpClient : AggregateRoot
	{
		public string IpAddress { get; init; }

		public TcpClientStatus Status { get; private set; }

		public DateTimeOffset CreatedTime { get; private set; }

		public DateTimeOffset? LastUpdated { get; private set; }

		public TcpClient(string ipAddress)
		{
			IpAddress = ipAddress;
			Status = TcpClientStatus.New;
			CreatedTime = DateTimeOffset.Now;
		}

		[BsonConstructor]
		public TcpClient() : base()
		{
		}

		public void Accept()
		{
			if (Status == TcpClientStatus.Accepted)
			{
				this.AddValidationError("Client is already accepted");
				return;
			}

			Status = TcpClientStatus.Accepted;
			LastUpdated = DateTimeOffset.Now;
		}

		public void Ban()
		{
			if (Status == TcpClientStatus.Banned)
			{
				this.AddValidationError("Client is already banned");
				return;
			}

			Status = TcpClientStatus.Banned;
			LastUpdated = DateTimeOffset.Now;
		}
	}
}
