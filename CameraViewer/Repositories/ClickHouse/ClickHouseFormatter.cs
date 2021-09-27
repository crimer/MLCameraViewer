using System;

namespace CameraViewer.Repositories.ClickHouse
{
    /// <summary>
    /// Класс для работы с типами данных ClickHouse
    /// </summary>
    public static class ClickHouseFormatter
    {
        /// <summary>Получить строковую полную дату формата ClickHouse</summary>
        /// <param name="date">Дата для преобразования в формат ClickHouse</param>
        /// <returns>Строковая полная дата в формате ClickHouse</returns>
        public static string ToClickHouseDateTime(DateTime date) => date.ToString("yyyy-MM-dd HH:mm:ss");
    }
}