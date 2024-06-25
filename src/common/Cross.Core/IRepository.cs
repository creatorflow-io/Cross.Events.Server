using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Cross
{
	public interface IRepository<T, TKey> 
		where TKey : IEquatable<TKey>
		where T : class, IIdentifiable<TKey>
	{
		Task<T> GetByIdAsync(TKey id);
		Task InsertAsync(T aggregate);
		Task UpdateAsync(T aggregate);
		Task DeleteAsync(TKey id);
	}
}
