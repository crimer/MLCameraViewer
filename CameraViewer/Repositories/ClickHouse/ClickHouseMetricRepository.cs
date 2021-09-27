using System;
using System.Data;
using System.Threading.Tasks;
using CameraViewer.Database;
using CameraViewer.Repositories.ClickHouse.MetricEvents;
using FluentResults;
using Microsoft.Extensions.Logging;

namespace CameraViewer.Repositories.ClickHouse
{
    /// <summary>
    /// Реализация репозитория по отправке различных метрик в ClickHouse
    /// </summary>
    public class ClickHouseMetricRepository
    {
        private readonly ClickHouseDbProvider _dbProvider;
        private readonly ScriptsProvider _scriptsProvider;
        private readonly ILogger<ClickHouseMetricRepository> _logger;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="dbProvider">Провайдер базы данных</param>
        /// <param name="scriptsProvider">Провайдер скриптов</param>
        /// <param name="logger">Логгер</param>
        public ClickHouseMetricRepository(ClickHouseDbProvider dbProvider, ScriptsProvider scriptsProvider, ILogger<ClickHouseMetricRepository> logger)
        {
            _dbProvider = dbProvider;
            _scriptsProvider = scriptsProvider;
            _logger = logger;
        }
        
        public async Task<Result> PushWhoWasSeenMetricAsync(WhoWasSeenMetric metric)
        {
            try
            {
                var query = _scriptsProvider.GetScriptByName("INS_WhoWasSeen_Event");

                var parameters = _dbProvider.CreateCommandParams(
                    ("DateTime", DbType.Date, ClickHouseFormatter.ToClickHouseDateTime(metric.DateTime)),
                    ("UserId", DbType.StringFixedLength, metric.UserId));
                
                _dbProvider.ExecuteNonQuery(query, parameters);
                return Result.Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError($"При отправке в ClickHouse метрики \"WhoWasSeen_Event\" приозошла ошибка: {ex}");
                return Result.Fail($"При отправке в ClickHouse метрики \"WhoWasSeen_Event\" приозошла ошибка");
            }
        }
    }
}