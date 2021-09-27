using System.Threading.Tasks;
using CameraViewer.Models.MetricEvents;

namespace CameraViewer.Services.Interfaces
{
    /// <summary>
    /// Абстракция для обработки различных метрик
    /// </summary>
    public interface IMetricRepository
    {
        /// <summary>
        /// Отправка метрики нажатия кнопки в ClickHouse
        /// </summary>
        /// <param name="pressingMetric">Модель метрики нажатия кнопки</param>
        Task PushButtonPressingMetricAsync(ButtonPressingMetric pressingMetric);
    }
}