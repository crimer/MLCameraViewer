using System;

namespace CameraViewer.Repositories.ClickHouse.MetricEvents
{
    /// <summary>
    /// Событие метрики нажатия кнопки
    /// </summary>
    public class WhoWasSeenMetric
    {
        /// <summary>
        /// Дата
        /// </summary>
        public DateTime DateTime { get; set; }
        
        /// <summary>
        /// Guid пользователя
        /// </summary>
        public Guid UserId { get; set; }
        

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <param name="dateTime">Дата</param>
        public WhoWasSeenMetric(Guid userId, DateTime dateTime)
        {
            DateTime = dateTime;
            UserId = userId;
        }
    }
}