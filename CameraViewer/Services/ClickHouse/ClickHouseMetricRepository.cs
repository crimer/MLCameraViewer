using System;
using System.Data;
using System.Reflection;
using System.Threading.Tasks;
using CameraViewer.Configuration;
using CameraViewer.Models.MetricEvents;
using CameraViewer.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CameraViewer.Services.ClickHouse
{
    /// <summary>
    /// Реализация репозитория по отправке различных метрик в ClickHouse
    /// </summary>
    public class ClickHouseMetricRepository : IMetricRepository
    {
        private readonly ILogger<ClickHouseMetricRepository> _logger;
        private readonly string _clickHouseConnectionString;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="config">Конфиг</param>
        /// <param name="logger">Логгер</param>
        public ClickHouseMetricRepository(IOptions<AppConfig> config, ILogger<ClickHouseMetricRepository> logger)
        {
            _logger = logger;
            _clickHouseConnectionString = config.Value.ClickHouseConnection;
        }
        
        /// <inheritdoc /> 
        public Task PushButtonPressingMetricAsync(ButtonPressingMetric pressingMetric)
        {
            try
            {
                var query = ClickHouseHelper.GetQueryString(Assembly.GetCallingAssembly(), "CH_INS_ButtonPressed_Event");

                var parameters = CreateCommandParams(
                    ("SysRowDate", DbType.Date, ClickHouseFormatter.ToClickHouseDateTime(pressingMetric.SysRowDate)),
                    ("SessionGuid", DbType.StringFixedLength, pressingMetric.SessionId),
                    ("EventDateUTC", DbType.DateTime, ClickHouseFormatter.ToClickHouseDateTime(pressingMetric.EventDateUTC)),
                    ("UserTimeZone", DbType.UInt16, pressingMetric.UserTimeZone),
                    ("ButtonGuid", DbType.StringFixedLength, pressingMetric.ButtonId),
                    ("ButtonName", DbType.String, pressingMetric.ButtonName),
                    ("EventTitle", DbType.String, pressingMetric.EventTitle),
                    ("EventDescription", DbType.String, pressingMetric.EventDescription),
                    ("DivisionGuid", DbType.StringFixedLength, pressingMetric.DivisionId),
                    ("DivisionName", DbType.String, pressingMetric.DivisionName),
                    ("RDCGuid", DbType.StringFixedLength, pressingMetric.RDCId),
                    ("RDCName", DbType.String, pressingMetric.RDCName),
                    ("BranchGuid", DbType.StringFixedLength, pressingMetric.BranchId),
                    ("BranchName", DbType.String, pressingMetric.BranchName));
                
                DbClickHouse.ExecuteNonQuery(query, _clickHouseConnectionString, parameters);
            }
            catch (Exception ex)
            {
                _logger.LogError($"При отправке в ClickHouse метрики нажатия кнопки произошла ошибка: {ex}");
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Вставка параметров в команду
        /// </summary>
        /// <param name="commandParams">Массив кортежей параметров</param>
        private ClickHouseParameter[] CreateCommandParams(params (string paramName, DbType dbType, object value)[] commandParams)
        {
            var parameters = new ClickHouseParameter[commandParams.Length];
            
            for (var i = 0; i < commandParams.Length; i++)
            {
                parameters[i] = new ClickHouseParameter()
                {
                    ParameterName = commandParams[i].paramName,
                    Value = commandParams[i].value,
                    DbType = commandParams[i].dbType
                };
            }

            return parameters;
        }
    }
}