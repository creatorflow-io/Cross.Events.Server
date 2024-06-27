using Cross.Events.MongoDB;
using Cross.Identity.Api.Contracts.Models.Users;
using Cross.MongoDB.Extensions;
using Juice.AspNetCore.Models;
using MongoDB.Bson;
using MongoDB.Driver;
namespace Cross.Identity.Api.Services
{
    public class UserViewService
    {
        private readonly MongoRepository<ApplicationUser, Guid> _repository;

        public UserViewService(MongoRepository<ApplicationUser, Guid> repository)
        {
            _repository = repository;
        }

        public async Task<DatasourceResult<UserRecordModel>> GetDatasourceResultAsync(DatasourceRequest request, CancellationToken token = default)
        {
            var collection = _repository.GetCollection();
            var roleCollection = MongoDatabaseExtensions.GetCollectionName<ApplicationRole>();

            var pipeline = new List<BsonDocument>();

            if (!string.IsNullOrEmpty(request.Query))
            {
                var name = request.Query;
                pipeline.Add(new BsonDocument("$match", new BsonDocument
                {
                    { "UserName" , new BsonDocument
                        {
                            { "$regex", name },
                            { "$options", "i" }
                        }
                    },
                    { "Email" , new BsonDocument
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
                new BsonDocument("$lookup",
                    new BsonDocument
                    {
                        { "from", roleCollection },
                        { "localField", "Roles" },
                        { "foreignField", "_id" },
                        { "as", "Roles" }
                    }),
                new BsonDocument("$addFields",
                    new BsonDocument
                    {
                        { "NameClaim", new BsonDocument("$first", new BsonDocument("$filter",
                            new BsonDocument
                            {
                                { "input", "$Claims" },
                                { "as", "claim" },
                                { "cond", new BsonDocument("$eq", new BsonArray { "$$claim.ClaimType", "name" }) }
                            })) },
                        { "GivenName", new BsonDocument("$first", new BsonDocument("$filter",
                            new BsonDocument
                            {
                                { "input", "$Claims" },
                                { "as", "claim" },
                                { "cond", new BsonDocument("$eq", new BsonArray { "$$claim.ClaimType", "given_name" }) }
                            })) },
                        { "SurnameClaim", new BsonDocument("$first", new BsonDocument("$filter",
                            new BsonDocument
                            {
                                { "input", "$Claims" },
                                { "as", "claim" },
                                { "cond", new BsonDocument("$eq", new BsonArray { "$$claim.ClaimType", "family_name" }) }
                            })) }
                    }),
                new BsonDocument("$project",
                    new BsonDocument
                    {
                        { "_id", 1 },
                        { "Roles", "$Roles.Name" },
                        { "UserName", 1},
                        { "Name", "$NameClaim.ClaimValue"},
                         { "FirstName", "$GivenName.ClaimValue"},
                        { "Surname", "$SurnameClaim.ClaimValue"},
                        { "Email", 1},
                        { "PhoneNumber", 1}
                    }),
                new BsonDocument("$sort", sortDocument),
                new BsonDocument("$skip", request.SkipCount),
                new BsonDocument("$limit", request.PageSize)
                 });

            return new DatasourceResult<UserRecordModel>
            {
                Data = await collection.Aggregate<UserRecordModel>(pipeline).ToListAsync(token),
                Count = count["TotalCount"].AsInt32
            };
        }

        public async Task<UserRecordModel> GetUserAsync(Guid id, CancellationToken token = default)
        {
            var collection = _repository.GetCollection();
            var roleCollection = MongoDatabaseExtensions.GetCollectionName<ApplicationRole>();

            var pipeline = new[]
            {
                new BsonDocument("$match", new BsonDocument("_id", id)),
                new BsonDocument("$lookup",
                    new BsonDocument
                    {
                        { "from", roleCollection },
                        { "localField", "Roles" },
                        { "foreignField", "_id" },
                        { "as", "Roles" }
                    }),
                new BsonDocument("$addFields",
                    new BsonDocument
                    {
                        { "NameClaim", new BsonDocument("$first", new BsonDocument("$filter",
                            new BsonDocument
                            {
                                { "input", "$Claims" },
                                { "as", "claim" },
                                { "cond", new BsonDocument("$eq", new BsonArray { "$$claim.ClaimType", "name" }) }
                            })) },
                        { "GivenName", new BsonDocument("$first", new BsonDocument("$filter",
                            new BsonDocument
                            {
                                { "input", "$Claims" },
                                { "as", "claim" },
                                { "cond", new BsonDocument("$eq", new BsonArray { "$$claim.ClaimType", "given_name" }) }
                            })) },
                        { "SurnameClaim", new BsonDocument("$first", new BsonDocument("$filter",
                            new BsonDocument
                            {
                                { "input", "$Claims" },
                                { "as", "claim" },
                                { "cond", new BsonDocument("$eq", new BsonArray { "$$claim.ClaimType", "family_name" }) }
                            })) }
                    }),
                new BsonDocument("$project",
                    new BsonDocument
                    {
                        { "_id", 1 },
                        { "Roles", "$Roles.Name" },
                        { "UserName", 1},
                        { "Name", "$NameClaim.ClaimValue"},
                        { "FirstName", "$GivenName.ClaimValue"},
                        { "Surname", "$SurnameClaim.ClaimValue"},
                        { "Email", 1},
                        { "PhoneNumber", 1}
                    })
                 };

            return await collection.Aggregate<UserRecordModel>(pipeline).FirstOrDefaultAsync(token);
        }
    }
}
