using MongoDB.Driver.Linq;
using System.Linq.Expressions;

namespace Cross.MongoDB.Extensions
{
	public static class MongoQueryableExtensions
	{
		public static IMongoQueryable<TSource> OrderBy<TSource>(
			this IMongoQueryable<TSource> query, string propertyName)
		{
			var entityType = typeof(TSource);
			if (entityType.GetProperty(propertyName) == null)
			{
				return query;
			}
			//Create x=>x.PropName
			ParameterExpression arg = Expression.Parameter(entityType, "x");
			MemberExpression property = Expression.Property(arg, propertyName);
			var selector = Expression.Lambda<Func<TSource, object>>(
				Expression.Convert(property, typeof(object)), new ParameterExpression[] { arg });

			return query.OrderBy(selector);
		}

		public static IOrderedMongoQueryable<TSource> ThenBy<TSource>(
			this IOrderedMongoQueryable<TSource> query, string propertyName)
		{
			var entityType = typeof(TSource);
			if (entityType.GetProperty(propertyName) == null)
			{
				return query;
			}
			//Create x=>x.PropName
			ParameterExpression arg = Expression.Parameter(entityType, "x");
			MemberExpression property = Expression.Property(arg, propertyName);
			var selector = Expression.Lambda<Func<TSource, object>>(
				Expression.Convert(property, typeof(object)), new ParameterExpression[] { arg });

			return query.ThenBy(selector);
		}

		public static IMongoQueryable<TSource> OrderByDescending<TSource>(
			this IMongoQueryable<TSource> query, string propertyName)
		{
			var entityType = typeof(TSource);
			if (entityType.GetProperty(propertyName) == null)
			{
				return query;
			}
			//Create x=>x.PropName
			ParameterExpression arg = Expression.Parameter(entityType, "x");
			MemberExpression property = Expression.Property(arg, propertyName);
			var selector = Expression.Lambda<Func<TSource, object>>(
				Expression.Convert(property, typeof(object)), new ParameterExpression[] { arg });

			return query.OrderByDescending(selector);
		}

		public static IOrderedMongoQueryable<TSource> ThenByDescending<TSource>(
			this IOrderedMongoQueryable<TSource> query, string propertyName)
		{
			var entityType = typeof(TSource);
			if (entityType.GetProperty(propertyName) == null)
			{
				return query;
			}
			//Create x=>x.PropName
			ParameterExpression arg = Expression.Parameter(entityType, "x");
			MemberExpression property = Expression.Property(arg, propertyName);
			var selector = Expression.Lambda<Func<TSource, object>>(
				Expression.Convert(property, typeof(object)), new ParameterExpression[] { arg });

			return query.ThenByDescending(selector);
		}

	}
}
