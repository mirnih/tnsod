using Microsoft.Extensions.Options;
using MongoDB.Driver;
using RateLimiter.Writer.Domain.Configuration;
using RateLimiter.Writer.Domain.Entities;
using RateLimiter.Writer.Domain.Interfaces;
using RateLimiter.Writer.Infrastructure.Data;
namespace RateLimiter.Writer.Infrastructure.Repositories;

public class RateLimitRepository : IRateLimitRepository
{
    private readonly IMongoCollection<RateLimitDbModel> _rateLimitsCollection;
    
    public RateLimitRepository(IOptions<MongoDbOptions> options)
    {
        var mongoClient = new MongoClient(options.Value.ConnectionString);
        var mongoDatabase = mongoClient.GetDatabase(options.Value.DatabaseName);
        _rateLimitsCollection = mongoDatabase.GetCollection<RateLimitDbModel>(options.Value.CollectionName);
    }

    public async Task<bool> CreateLimitAsync(RateLimit limit, CancellationToken cancellationToken = default)
    {
        var dbModel = new RateLimitDbModel
        {
            Route = limit.Route,
            RequestsPerMinute = limit.RequestsPerMinute
        };

        try
        {
            await _rateLimitsCollection.InsertOneAsync(dbModel, cancellationToken: cancellationToken);
            return true;
        }
        catch (MongoWriteException ex) when (ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
        {
            return false;
        }
    }
    
    public async Task<RateLimit?> GetLimitByRouteAsync(string route, CancellationToken cancellationToken = default)
    {
        var filter = Builders<RateLimitDbModel>.Filter.Eq(x => x.Route, route);
        var dbModel = await _rateLimitsCollection.Find(filter).FirstOrDefaultAsync(cancellationToken);

        if (dbModel == null)
        {
            return null;
        }

        return new RateLimit
        {
            Route = dbModel.Route,
            RequestsPerMinute = dbModel.RequestsPerMinute
        };
    }
    
    public async Task<bool> UpdateLimitAsync(RateLimit limit, CancellationToken cancellationToken = default)
    {
        var filter = Builders<RateLimitDbModel>.Filter.Eq(x => x.Route, limit.Route);
        var update = Builders<RateLimitDbModel>.Update.Set(x => x.RequestsPerMinute, limit.RequestsPerMinute);
        var result = await _rateLimitsCollection.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);
        return result.IsAcknowledged && result.ModifiedCount > 0;
    }
    
    public async Task<bool> DeleteLimitAsync(string route, CancellationToken cancellationToken = default)
    {
        var filter = Builders<RateLimitDbModel>.Filter.Eq(x => x.Route, route);
        var result = await _rateLimitsCollection.DeleteOneAsync(filter, cancellationToken);
        return result.IsAcknowledged && result.DeletedCount > 0;
    }
}