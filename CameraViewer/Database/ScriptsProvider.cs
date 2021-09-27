using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace CameraViewer.Database
{
    /// <summary>
    /// Провайдер, который предоставляет доступ к скриптам работы с хранилищами
    /// </summary>
    public class ScriptsProvider
    {
        private readonly Dictionary<string, string> _queries = new Dictionary<string, string>();

        /// <summary>
        /// Конструктор
        /// </summary>
        public ScriptsProvider()
        {
            var executingAssembly = Assembly.GetExecutingAssembly();
            var procedures = executingAssembly
                .GetManifestResourceNames();

            foreach (var procedure in procedures)
            {
                var procedurePath = procedure.Substring(0, procedure.LastIndexOf("."));
                var procedureName = procedurePath.Substring(procedurePath.LastIndexOf(".") + 1);

                using (var reader = new StreamReader(executingAssembly.GetManifestResourceStream(procedure)))
                _queries.Add(procedureName, reader.ReadToEnd());
            }
        }
        
        /// <summary>
        /// Получение текста запроса по названию
        /// </summary>
        /// <param name="queryName">Название запроса</param>
        /// <exception cref="KeyNotFoundException">Если запрос, с указанным названием не найден, то бросат исключение</exception>
        public string GetScriptByName(string queryName)
        {
            if (_queries.TryGetValue(queryName, out var queryText))
                return queryText;
            
            throw new KeyNotFoundException($"Не найден запрос с названием {queryName}");
        }
    }
}