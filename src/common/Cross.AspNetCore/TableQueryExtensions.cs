using Juice.AspNetCore.Models;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using SortDirection = Juice.AspNetCore.Models.SortDirection;
using Cross.MongoDB.Extensions;

namespace Cross.AspNetCore
{
	public static class TableQueryExtensions
	{
		private static IMongoQueryable<TSource> ApplyQuery<TSource>(
			IMongoQueryable<TSource> query, DatasourceRequest request)
		{
			foreach (var sort in request.Sorts)
			{
				var property = string.Concat(sort.Property[0].ToString().ToUpper(), sort.Property.AsSpan(1));

				if (sort.Direction == SortDirection.Asc)
				{
					query = query is IOrderedMongoQueryable<TSource> ordered
						&& query.Expression.Type == typeof(IOrderedMongoQueryable<TSource>)
						? ordered.ThenBy(property)
						: query.OrderBy(property);
				}
				else
				{
					query = query is IOrderedMongoQueryable<TSource> ordered
						&& query.Expression.Type == typeof(IOrderedMongoQueryable<TSource>)
						? ordered.ThenByDescending(property)
						: query.OrderByDescending(property);
				}
			}

			return query.Skip(request.SkipCount).Take(request.PageSize);
		}

		public static async Task<DatasourceResult<TSource>> ToDatasourceResultAsync<TSource>(this IMongoQueryable<TSource> query, DatasourceRequest request, CancellationToken token)
		{
			var count = await query.CountAsync(token);

			var result = new DatasourceResult<TSource>
			{
				Page = request.Page,
				PageSize = request.PageSize,
				Count = count,
				Data = await ApplyQuery(query, request).ToListAsync(token)
			};
			return result;
		}

	}
}
