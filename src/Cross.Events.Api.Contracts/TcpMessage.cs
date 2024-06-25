namespace Cross.Events.Api.Contracts
{
	public record TcpMessage
	{
		public static readonly string MessagesSeparator = Environment.NewLine;
		public static readonly uint MaxLength = 50;

		private long _timestamp;
		public DateTimeOffset Timestamp { get; init; }
		public string Message { get; init; }

		public TcpMessage(DateTimeOffset timestamp, string message): this(timestamp.ToUnixTimeMilliseconds(), message) {
			Timestamp = timestamp;
		}

		TcpMessage(long timestamp, string message)
		{
			_timestamp = timestamp;

			if (Timestamp == default)
				Timestamp = DateTimeOffset.FromUnixTimeMilliseconds(_timestamp);

			var tmp = $"{_timestamp}: ";
			message = message.Trim();
			
			// Validate message length
			var remaining = MaxLength - tmp.Length;
			if (message.Length > remaining)
			{
				throw new ArgumentException($"Message length must be less than {remaining} characters");
			}
			Message = message;
		}

		public override string ToString() => $"{_timestamp}: {Message}";

		public static TcpMessage Parse(string message)
		{
			var parts = message.Split(':');
			if (parts.Length != 2)
			{
				throw new FormatException("Invalid message format");
			}
			return new TcpMessage(long.Parse(parts[0]), parts[1]);
		}
	}
}
