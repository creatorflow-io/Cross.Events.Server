using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cross
{
	public interface IIdentifiable<TKey> where TKey : IEquatable<TKey>
	{
		TKey Id { get; }
	}
}
