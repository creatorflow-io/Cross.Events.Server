using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cross
{
	public class DataEvent<T> : INotification
	{
		public DataEvent(T data)
		{
			Entity = data;
		}

		public T Entity { get; }

	}

	public class DataInserted<T> : DataEvent<T>
	{
		public DataInserted(T data) : base(data)
		{
		}
	}

	public class DataDeleted<T> : DataEvent<T>
    {
		public DataDeleted(T data) : base(data)
		{
		}
    }

}
