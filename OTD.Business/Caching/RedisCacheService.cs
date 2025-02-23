using Microsoft.Extensions.Caching.Distributed;
using OTD.ServiceLayer.Abstract;
using StackExchange.Redis;
using System.Text.Json;
using System.Threading.Tasks;

namespace OTD.ServiceLayer.Caching
{
    public class RedisCacheService : ICacheService
    {
        private readonly IDistributedCache? _cache;
        private readonly IDatabase _redisDatabase;

        public RedisCacheService(IDistributedCache? cache, IConnectionMultiplexer redisConnection)
        {
            _cache = cache;
            _redisDatabase = redisConnection.GetDatabase();
        }

        public T? GetData<T>(string key)
        {
            var data = _cache?.GetString(key);
            if (data == null)
                return default;

            return JsonSerializer.Deserialize<T>(data);
        }

        public void SetData<T>(string key, T data)
        {
            _cache?.SetString(key, JsonSerializer.Serialize(data));
        }

        public async Task<bool> FlushAllDatabases()
        {
            var endpoints = _redisDatabase.Multiplexer.GetEndPoints();
            foreach (var endpoint in endpoints)
            {
                var server = _redisDatabase.Multiplexer.GetServer(endpoint);
                await server.FlushAllDatabasesAsync();
            }
            return true;
        }

        public async Task<bool> FlushAllDatabases(string includeKey)
        {
            var endpoints = _redisDatabase.Multiplexer.GetEndPoints();
            foreach (var endpoint in endpoints)
            {
                var server = _redisDatabase.Multiplexer.GetServer(endpoint);
                var keys = server.Keys();
                foreach (var key in keys)
                {
                    if (key.ToString().Contains(includeKey))
                    {
                        await _redisDatabase.KeyDeleteAsync(key);
                    }
                }
            }
            return true;
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            var data = await _redisDatabase.StringGetAsync(key);
            if (data.IsNullOrEmpty)
                return default;

            return JsonSerializer.Deserialize<T>(data!);
        }

        public async Task<bool> RemoveCacheAsync(string cacheKey)
        {
            return await _redisDatabase.KeyDeleteAsync(cacheKey);
        }

        public async Task<bool> SetCacheAsync<T>(string key, T value, short? minute = null)
        {
            var options = minute.HasValue ? new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(minute.Value)
            } : null;

            var serializedValue = JsonSerializer.Serialize(value);

            if (_cache != null)
            {
                _cache.SetString(key, serializedValue, options);
            }
            await _redisDatabase.StringSetAsync(key, serializedValue, options?.AbsoluteExpirationRelativeToNow);

            return true;
        }
    }
}
