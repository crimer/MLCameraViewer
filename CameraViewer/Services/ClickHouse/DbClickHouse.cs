using System;
using System.Collections.Generic;
using System.Data;
using ClickHouse.Ado;

namespace CameraViewer.Services.ClickHouse
{
    /// <summary>Класс для работы с запросами к ClickHouse</summary>
  public static class DbClickHouse
  {
    /// <summary>Получение списка данных</summary>
    /// <typeparam name="T">Тип данных</typeparam>
    /// <param name="query">Запрос на выборку</param>
    /// <param name="connectionString">Строка подключения</param>
    /// <param name="binding">Биндинг</param>
    /// <param name="parameters">Параметры запроса</param>
    /// <returns>Список данных</returns>
    public static List<T> GetList<T>(
      string query,
      string connectionString,
      Func<IDataReader, T> binding,
      params ClickHouseParameter[] parameters)
    {
      using (ClickHouseConnection connection = DbClickHouse.GetConnection(connectionString))
        return DbClickHouse.ExecuteReader<T>(DbClickHouse.GetCommand(connection, query, parameters), binding);
    }

    /// <summary>Вставка, удаление или обновление данных</summary>
    /// <param name="query">Строка запроса</param>
    /// <param name="connectionString">Строка подключения</param>
    /// <param name="parameter">Параметр запроса</param>
    public static void ExecuteNonQuery(
      string query,
      string connectionString,
      params ClickHouseParameter[] parameter)
    {
      using (ClickHouseConnection connection = DbClickHouse.GetConnection(connectionString))
        DbClickHouse.GetCommand(connection, query, parameter).ExecuteNonQuery();
    }

    /// <summary>Выполнение запроса возвращающего скалярное значение</summary>
    /// <typeparam name="T">Тип данных</typeparam>
    /// <param name="query">Запрос на выборку</param>
    /// <param name="connectionString">Строка подключения</param>
    /// <param name="parameters">Параметры запроса</param>
    /// <returns>Список данных</returns>
    public static T ExecuteScalar<T>(
      string query,
      string connectionString,
      params ClickHouseParameter[] parameters)
    {
      using (ClickHouseConnection connection = DbClickHouse.GetConnection(connectionString))
        return TypeConvertUtility.Convert<T>(DbClickHouse.GetCommand(connection, query, parameters).ExecuteScalar());
    }

    /// <summary>Получение строки подключения</summary>
    /// <param name="connectionString">Строка подключения</param>
    /// <returns></returns>
    private static ClickHouseConnection GetConnection(string connectionString)
    {
      ClickHouseConnection clickHouseConnection = new ClickHouseConnection(new ClickHouseConnectionSettings(connectionString));
      clickHouseConnection.Open();
      return clickHouseConnection;
    }

    /// <summary>Получение команды</summary>
    /// <param name="connection">Соединение</param>
    /// <param name="query">Запрос</param>
    /// <param name="parameters">Параметры</param>
    /// <returns>Команда</returns>
    private static ClickHouseCommand GetCommand(
      ClickHouseConnection connection,
      string query,
      params ClickHouseParameter[] parameters)
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
    private static List<T> ExecuteReader<T>(
      ClickHouseCommand command,
      Func<IDataReader, T> binding)
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
        }
        while (dataReader.NextResult());
      }
      return objList;
    }
  }
}