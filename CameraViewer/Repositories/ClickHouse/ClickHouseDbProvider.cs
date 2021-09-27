using System;
using System.Collections.Generic;
using System.Data;
using CameraViewer.Configuration;
using ClickHouse.Ado;
using Microsoft.Extensions.Options;

namespace CameraViewer.Repositories.ClickHouse
{
    /// <summary>
    /// Класс для работы с запросами к ClickHouse
    /// </summary>
    public class ClickHouseDbProvider
    {
        private readonly string _clickHouseConnectionString;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="config">Конфиг</param>
        public ClickHouseDbProvider(IOptions<AppConfig> config)
        {
            _clickHouseConnectionString = config.Value.ClickHouseConnection;
        }

        /// <summary>Получение списка данных</summary>
        /// <typeparam name="T">Тип данных</typeparam>
        /// <param name="query">Запрос на выборку</param>
        /// <param name="binding">Биндинг</param>
        /// <param name="parameters">Параметры запроса</param>
        /// <returns>Список данных</returns>
        public List<T> GetList<T>(string query, Func<IDataReader, T> binding, params ClickHouseParameter[] parameters)
        {
            using (var connection = GetConnection(_clickHouseConnectionString))
                return ExecuteReader<T>(GetCommand(connection, query, parameters), binding);
        }

        /// <summary>Вставка, удаление или обновление данных</summary>
        /// <param name="query">Строка запроса</param>
        /// <param name="parameter">Параметр запроса</param>
        public void ExecuteNonQuery(string query, params ClickHouseParameter[] parameter)
        {
            using (var connection = GetConnection(_clickHouseConnectionString))
                GetCommand(connection, query, parameter).ExecuteNonQuery();
        }

        /// <summary>
        /// Вставка параметров в команду
        /// </summary>
        /// <param name="commandParams">Массив кортежей параметров</param>
        public ClickHouseParameter[] CreateCommandParams(params (string paramName, DbType dbType, object value)[] commandParams)
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
        
        /// <summary>Получение строки подключения</summary>
        /// <param name="connectionString">Строка подключения</param>
        /// <returns></returns>
        private ClickHouseConnection GetConnection(string connectionString)
        {
            var clickHouseConnection = new ClickHouseConnection(new ClickHouseConnectionSettings(connectionString));
            clickHouseConnection.Open();
            return clickHouseConnection;
        }

        /// <summary>Получение команды</summary>
        /// <param name="connection">Соединение</param>
        /// <param name="query">Запрос</param>
        /// <param name="parameters">Параметры</param>
        /// <returns>Команда</returns>
        private ClickHouseCommand GetCommand(ClickHouseConnection connection, string query, params ClickHouseParameter[] parameters)
        {
            ClickHouseCommand command = connection.CreateCommand();
            command.CommandText = query;
            foreach (ClickHouseParameter parameter in parameters)
                command.Parameters.Add(parameter);
            return command;
        }

        /// <summary>Считывание выборки запроса</summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="command">Команда</param>
        /// <param name="binding">Биндинг</param>
        /// <returns>Результирующая выборка</returns>
        private List<T> ExecuteReader<T>(ClickHouseCommand command, Func<IDataReader, T> binding)
        {
            List<T> objList = new List<T>();
            using (IDataReader dataReader = command.ExecuteReader())
            {
                do
                {
                    while (dataReader.Read())
                    {
                        T obj = binding(dataReader);
                        objList.Add(obj);
                    }
                } while (dataReader.NextResult());
            }

            return objList;
        }
    }
}