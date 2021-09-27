using System.Collections;

namespace CameraViewer.Utils
{
    /// <summary>
    /// Методы расширения для коллекций
    /// </summary>
    public static class CollectionExtensions
    {
        /// <summary>
        /// Коллекция пуста или null
        /// </summary>
        /// <param name="list">Коллекция</param>
        /// <returns>Bool</returns>
        public static bool IsNullOrEmpty(this IList list) => list == null || list.Count == 0;
    }
}