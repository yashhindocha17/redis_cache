using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Npgsql;
using StackExchange.Redis;
using System.Data;

namespace redis_cache.Controllers
{
    public interface IRedisCacheHelper
    {
        Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan expiration);
    }

    public class RedisCacheHelper : IRedisCacheHelper
    {
        private readonly IConnectionMultiplexer _redis;

        public RedisCacheHelper(IConnectionMultiplexer redis)
        {
            _redis = redis;
        }


        public async Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan expiration)
        {
            var database = _redis.GetDatabase();
            var cachedValue = await database.StringGetAsync(key);

            if (cachedValue.HasValue)
            {
                return Deserialize<T>(cachedValue);
            }

            var value = await factory();

            await database.StringSetAsync(key, Serialize(value), expiration);

            return value;
        }

        private T Deserialize<T>(RedisValue cachedValue)
        {
            throw new NotImplementedException();
        }

        private RedisValue Serialize<T>(T? value)
        {
            throw new NotImplementedException();
        }
        /*private byte[] Serialize<T>(T value)
{

}
private T Deserialize<T>(byte[] value)
{

}*/
    }
}
