using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace RedisOperations;

public class RedisRepository
{
    private readonly ILogger<RedisRepository> _logger;

    public RedisRepository(ILogger<RedisRepository> logger)
    {
        _logger = logger;
    }

    public async Task AddValue<TKeyType, TValueType> (string key, IEnumerable<KeyValuePair<TKeyType, TValueType>> valuesToAdd)
    {
        var redisDatabase = RedisConnectorHelper.Database;

        var values = valuesToAdd
            .Select(x => new HashEntry(x.Key!.ToString(),
                JsonConvert.SerializeObject(x.Value)))
            .ToArray();
        await redisDatabase.HashSetAsync(key, values);
    }

    public async Task<IReadOnlyCollection<TValueType>> Get<TValueType>(string key)
    {
        var redisDatabase = RedisConnectorHelper.Database;
        var values = await redisDatabase.HashValuesAsync(key);
        return values.Select(value => JsonConvert.DeserializeObject<TValueType>(value)).ToArray();
    }

    public async Task DeleteValues<TKeyType>(string keyRoot, IReadOnlyCollection<TKeyType> keysToDelete)
    {
        var redisDatabase = RedisConnectorHelper.Database;
        var response = await redisDatabase.HashDeleteAsync(
            keyRoot,
            keysToDelete.Select(x => new RedisValue(x!.ToString()!)).ToArray()
        );
        
        if (response == 0)
        {
            throw new Exception($"Не смогли найти книгу с идентификатором {keyRoot}");
        }
    }
    
    public async Task DeleteValue<TKeyType>(TKeyType keyToDelete)
    {
        var redisDatabase = RedisConnectorHelper.Database;
        
        var keys = RedisConnectorHelper.Connection.GetServers().Single().Keys();
        foreach (var key in keys)
        {
            var response = await redisDatabase.HashDeleteAsync(
                key,
                new RedisValue(keyToDelete!.ToString()!)
            );

            if (response)
            {
                _logger.LogInformation($"Успешно удалили книгу с идентификатором {keyToDelete}");
                return;
            }
        }
        
        throw new Exception($"Не смогли найти книгу с идентификатором {keyToDelete}");
    }
    
    public async Task DeleteValuesByRootKey(string keyRoot)
    {
        var redisDatabase = RedisConnectorHelper.Database;
        var response = await redisDatabase.KeyDeleteAsync(keyRoot);

        if (!response)
        {
            throw new Exception($"Не смогли найти книги в жанре {keyRoot}");
        }
    }
}