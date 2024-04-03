using StackExchange.Redis;

namespace RedisOperations;

public class RedisConnectorHelper
{
    static RedisConnectorHelper()
    {
        LazyConnection = new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect("localhost:6380"));
    }

    private static readonly Lazy<ConnectionMultiplexer> LazyConnection;

    public static ConnectionMultiplexer Connection => LazyConnection.Value;
    public static IDatabase Database => Connection.GetDatabase();
}