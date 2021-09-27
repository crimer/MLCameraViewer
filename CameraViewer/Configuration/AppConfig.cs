namespace CameraViewer.Configuration
{
    /// <summary>
    /// Конфиги приложения
    /// </summary>
    public class AppConfig
    {
        /// <summary>
        /// Строка подключения к ClickHouse
        /// </summary>
        public string ClickHouseConnection { get; set; }
        
        /// <summary>
        /// Строка подключения к Redis
        /// </summary>
        public string RedisConnection { get; set; }
        
        /// <summary>
        /// Номер БД Redis
        /// </summary>
        public int RedisDatabaseNumber { get; set; }
    }
}