using System;
using System.Threading.Tasks;
using FluentResults;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CameraViewer.Repositories.Redis
{
    /// <summary>
    /// Репозиторий для работы с Redis
    /// </summary>
    public class RedisRepository
    {
        private readonly RedisDbProvider _redisDbProvider;
        private readonly ILogger<RedisRepository> _logger;
        private readonly string _usersKey = "MLNetwork";
        
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="redisDbProvider">Клиент Redis</param>
        /// <param name="logger">Логгер</param>
        public RedisRepository(RedisDbProvider redisDbProvider, ILogger<RedisRepository> logger)
        {
            _redisDbProvider = redisDbProvider;
            _logger = logger;
        }

        /// <summary>
        /// Сохранения участника чата
        /// </summary>
        /// <param name="userId">Идентификатор пользователя</param>
        public async Task<Result> SaveAsync(Guid userId)
        {
            try
            {
                var redis = _redisDbProvider.GetDatabase();
                
                // var jsonUser = JsonConvert.SerializeObject(); 
                //
                // var containUser = await redis.SetContainsAsync(GetKey(chatId, chetMemberName), jsonUser);
                //
                // if(!containUser)
                //     await redis.HashSetAsync(_usersKey, GetKey(chatId, chetMemberName), jsonUser);
                
                return Result.Ok();
            }
            catch (Exception e)
            {
                _logger.LogError($"При сохранении информации о пользователе произошла ошибка: {e}");
                return Result.Fail($"При сохранении информации о пользователе произошла ошибка");
            }
        }
    }
}