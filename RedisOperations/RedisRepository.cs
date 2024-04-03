using Newtonsoft.Json;
using StackExchange.Redis;

namespace RedisOperations;

public class RedisRepository
{
    public static async Task AddValue<TKeyType, TValueType> (string key, IEnumerable<KeyValuePair<TKeyType, TValueType>> valuesToAdd)
    {
        var redisDatabase = RedisConnectorHelper.Database;

        var values = valuesToAdd
            .Select(x => new HashEntry(x.Key!.ToString(),
                JsonConvert.SerializeObject(x.Value)))
            .ToArray();
        await redisDatabase.HashSetAsync(key, values);
    }

    public static async Task<IReadOnlyCollection<TValueType>> Get<TValueType>(string key)
    {
        var redisDatabase = RedisConnectorHelper.Database;
        var values = await redisDatabase.HashValuesAsync(key);
        return values.Select(value => JsonConvert.DeserializeObject<TValueType>(value)).ToArray();
    }

    public static async Task DeleteValue<TKeyType>(string keyRoot, IReadOnlyCollection<TKeyType> keysToDelete)
    {
        var redisDatabase = RedisConnectorHelper.Database;
        await redisDatabase.HashDeleteAsync(
            keyRoot,
            keysToDelete.Select(x => new RedisValue(x!.ToString()!)).ToArray()
        );
    }
}