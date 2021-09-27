using CameraViewer.Configuration;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace CameraViewer.Repositories.Redis
{
    /// <summary>
    /// Клиент для работы с Redis
    /// </summary>
    public class RedisDbProvider
    {
        private readonly ConnectionMultiplexer _connection;
        private readonly int _redisDatabaseNumber;

        /// <summary>
        /// Констуктор
        /// </summary>
        /// <param name="options">Конфиг</param>
        public RedisDbProvider(IOptions<AppConfig> options)
        {
            _connection = ConnectionMultiplexer.Connect(options.Value.RedisConnection);
            _redisDatabaseNumber = options.Value.RedisDatabaseNumber;
        }
        
        /// <summary>
        /// Получение <see cref="IDatabase"/>
        /// </summary>
        public IDatabase GetDatabase()
        {
            return _connection.GetDatabase(_redisDatabaseNumber);
        }
    }
}