using Cross.Events.MongoDB;
using Cross.Identity.Api.Contracts.Models.Users;
using Cross.MongoDB.Extensions;
using Juice.AspNetCore.Models;
using Microsoft.AspNetCore.Routing;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Cross.Identity.Api.Services
{
    public class RoleViewService
    {
        private readonly MongoRepository<ApplicationRole, Guid> _repository;

        public RoleViewService(MongoRepository<ApplicationRole, Guid> repository)
        {
            _repository = repository;
        }

        public async Task<DatasourceResult<ApplicationRole>> GetDatasourceResultAsync(DatasourceRequest request, CancellationToken token = default)
        {
            var collection = _repository.GetCollection();

            var pipeline = new List<BsonDocument>();

            if (!string.IsNullOrEmpty(request.Query))
            {
                var name = request.Query;
                pipeline.Add(new BsonDocument("$match", new BsonDocument
                {
                    { "Name" , new BsonDocument
                        {
                            { "$regex", name },
                            { "$options", "i" }
                        }
                    }
                }));
            }

            var countPipeline = new List<BsonDocument>(pipeline)
            {
                new BsonDocument("$count", "TotalCount")
            };
            var count = await collection.Aggregate<BsonDocument>(countPipeline).FirstAsync(token);

            BsonDocument? sortDocument = null;
            foreach (var sort in request.Sorts)
            {
                sortDocument = sortDocument == null
                    ? new BsonDocument(sort.Property, sort.Direction == Juice.AspNetCore.Models.SortDirection.Asc ? 1 : -1)
                    : sortDocument.Add(sort.Property, sort.Direction == Juice.AspNetCore.Models.SortDirection.Asc ? 1 : -1);
            }
            sortDocument = sortDocument ?? new BsonDocument("_id", 1);

            pipeline.AddRange(new[]
            {
                new BsonDocument("$sort", sortDocument),
                new BsonDocument("$skip", request.SkipCount),
                new BsonDocument("$limit", request.PageSize)
                 });

            return new DatasourceResult<ApplicationRole>
            {
                Data = await collection.Aggregate<ApplicationRole>(pipeline).ToListAsync(token),
                Count = count["TotalCount"].AsInt32
            };
        }
    }
}
