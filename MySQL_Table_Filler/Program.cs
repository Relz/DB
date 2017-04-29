using System;
using System.Data;
using System.ComponentModel;
using System.IO;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Data.SqlTypes;
using MySQL_Table_Filler;
using System.Linq;

namespace DB_MySQL_Table_FILLER
{
	class Program
	{
		static private void GetTablesName(MySqlConnection mySqlConnection, String databaseName, out List<String> result)
		{
			result = new List<String>();
			MySqlCommand mySqlCommand = new MySqlCommand("SHOW TABLES FROM " + databaseName, mySqlConnection);
			using (MySqlDataReader mySqlDataReader = mySqlCommand.ExecuteReader())
			{
				while (mySqlDataReader.Read())
				{
					result.Add(mySqlDataReader.GetString(0));
				}
				if (result.Count == 0)
				{
					Console.WriteLine("Ошибка: База данных не содержит таблиц.");
					Environment.Exit(exitCode: 1);
				}
			}
		}

		static private void GetColumns(MySqlConnection mySqlConnection, String tableName, out List<MySqlColumn> result)
		{
			result = new List<MySqlColumn>();
			MySqlCommand mySqlCommand = new MySqlCommand("SELECT * FROM " + tableName, mySqlConnection);
			using (MySqlDataReader mySqlDataReader = mySqlCommand.ExecuteReader())
			{
				if (mySqlDataReader.FieldCount == 0)
				{
					Console.WriteLine("Ошибка: Таблица не содержит столбцов.");
					Environment.Exit(exitCode: 1);
				}
				for (int i = 0; i < mySqlDataReader.FieldCount; i++)
				{
					string columnName = mySqlDataReader.GetName(i);
					result.Add(new MySqlColumn(columnName, mySqlDataReader.GetFieldType(columnName)));
				}
			}
		}

		static private void ShowDatabaseTables(List<String> tablesName, String databaseName)
		{
			Console.WriteLine("Найдены следующие таблицы в базе данных '" + databaseName + "'");
			for (int i = 0; i < tablesName.Count; ++i)
			{
				Console.WriteLine((i + 1) + ") " + tablesName[i]);
			}
		}

		static private String GetInsertValue(MySqlConnection mySqlConnection, MySqlColumn column)
		{
			if (column.fillRule == FillRule.RANDOM_FROM_RANGE)
			{
				if (column.type == typeof(UInt32))
				{
					return "'" + FillOptions.GetRandomFromRange(column.fillOptions.rangeNumberStart, column.fillOptions.rangeNumberEnd) + "'";
				}
				else if (column.type == typeof(DateTime))
				{
					return "'" + FillOptions.GetRandomFromRange(column.fillOptions.rangeDateTimeStart, column.fillOptions.rangeDateTimeEnd) + "'";
				}
			}
			if (column.fillRule == FillRule.RANDOM_FROM_LIST)
			{
				return "'" + FillOptions.GetRandomFromList(column.fillOptions.fileName) + "'";
			}
			if (column.fillRule == FillRule.RANDOM_FROM_ANOTHER_TABLE)
			{
				return "'" + FillOptions.GetRandomFromAnotherTable(column.fillOptions.tableName, column.name) + "'";
			}
			return "";
		}

		static private String MakeQueryString(MySqlConnection mySqlConnection, String tableName, List<MySqlColumn> columns)
		{
			String result = "INSERT INTO `" + tableName + "`" + "(";
			int currentInsertItem = 0;
			for (int i = 0; i < columns.Count; ++i)
			{
				if (columns[i].fillRule != FillRule.DO_NOT_FILL)
				{
					++currentInsertItem;
					if (currentInsertItem != 1)
					{
						result += ",";
					}
					result += "`" + columns[i].name + "`";
				}
			}
			result += ") VALUES (";
			currentInsertItem = 0;
			for (int i = 0; i < columns.Count; ++i)
			{
				if (columns[i].fillRule != FillRule.DO_NOT_FILL)
				{
					++currentInsertItem;
					if (currentInsertItem != 1)
					{
						result += ",";
					}
					result += GetInsertValue(mySqlConnection, columns[i]);
				}
			}
			result += ")";
			return result;
		}

		static private void ReadRequiredInformationForColumnFilling(MySqlColumn column, MySqlConnection mySqlConnection)
		{
			if (column.fillRule == FillRule.RANDOM_FROM_LIST)
			{
				while (!FillOptions.ReadList(column.fillOptions.fileName))
				{
					column.fillRule = column.fillRule;
				}
			}
			else if (column.fillRule == FillRule.RANDOM_FROM_ANOTHER_TABLE)
			{
				while (!FillOptions.ReadAnotherTableColumn(mySqlConnection, column.fillOptions.tableName, column.name))
				{
					column.fillRule = column.fillRule;
				}
			}
		}

		static private void ExecuteMySqlQuery(MySqlConnection mySqlConnection, string queryString)
		{
			MySqlCommand mySqlCommand = new MySqlCommand(queryString, mySqlConnection);
			mySqlCommand.ExecuteNonQuery();
		}

		static private void DrawTextProgressBar(int progress, int total)
		{
			Console.CursorLeft = 0;
			Console.Write("[");
			Console.CursorLeft = 52;
			Console.Write("]");
			Console.CursorLeft = 1;
			float oneChunk = 50.0f / total;

			int position = 1;
			for (int i = 0; i < oneChunk * progress; ++i)
			{
				Console.CursorLeft = position++;
				Console.BackgroundColor = ConsoleColor.Gray;
				Console.Write(" ");
			}

			Console.ResetColor();
			Console.CursorLeft = 55;
			Console.Write(((int)(((float)progress / total) * 100)).ToString() + "%");
		}

		static void Main()
		{
			Console.Clear();
			LogFile logFile = new LogFile(CONSTANT.LOG.FILE_NAME);
			MySqlConfig mySqlConfig = new MySqlConfig(CONSTANT.CONFIG.FILE_NAME);
			mySqlConfig.Read();

			if (!mySqlConfig.Check(CONSTANT.CONFIG.REQUIRED_KEYS))
			{
				Environment.Exit(exitCode: 1);
			}

			MySqlConnectionStringBuilder mySqlConnectionStringBuilder = new MySqlConnectionStringBuilder();
			mySqlConnectionStringBuilder.Server = mySqlConfig.get[CONSTANT.CONFIG.REQUIRED_KEYS[0]];
			mySqlConnectionStringBuilder.Database = mySqlConfig.get[CONSTANT.CONFIG.REQUIRED_KEYS[1]];
			mySqlConnectionStringBuilder.UserID = mySqlConfig.get[CONSTANT.CONFIG.REQUIRED_KEYS[2]];
			mySqlConnectionStringBuilder.Password = mySqlConfig.get[CONSTANT.CONFIG.REQUIRED_KEYS[3]];

			using (MySqlConnection mySqlConnection = new MySqlConnection())
			{
				mySqlConnection.ConnectionString = mySqlConnectionStringBuilder.ConnectionString;
				try
				{
					mySqlConnection.Open();
				}
				catch (Exception e)
				{
					Console.WriteLine(e.Message);
				}

				List<String> tablesName;
				GetTablesName(mySqlConnection, mySqlConnectionStringBuilder.Database, out tablesName);

				ShowDatabaseTables(tablesName, mySqlConnectionStringBuilder.Database);

				int tableIndex = UserAsk.Number("Введите порядковый номер таблицы, которую хотите заполнить: ") - 1;
				String tableName = tablesName[tableIndex];

				List<MySqlColumn> columns;
				GetColumns(mySqlConnection, tableName, out columns);

				Console.Clear();
				Console.WriteLine("Таблица: '" + tableName + "'");

				for (int i = 0; i < columns.Count; ++i)
				{
					if (!CONSTANT.TYPE_EXPLAINATION.ContainsKey(columns[i].type))
					{
						Console.WriteLine("Ошибка: Столбец " + columns[i].name + " неизвестный программе тип " + columns[i].type);
						Environment.Exit(exitCode: 1);
					}
					Console.Clear();
					Console.WriteLine("Столбец " + columns[i].name + "(" + CONSTANT.TYPE_EXPLAINATION[columns[i].type] + ")" + " может быть заполнен следующими способами:");

					for (int j = 0; j < CONSTANT.FILL_RULES[columns[i].type].Count(); ++j)
					{
						Console.WriteLine((j + 1) + ") " + CONSTANT.RULE_EXPLAINATION[CONSTANT.FILL_RULES[columns[i].type][j]]);
					}
					int rulesIndex = UserAsk.Number("Введите порядковый номер способа, наиболее подходящего для данного столбца: ") - 1;
					columns[i].fillRule = CONSTANT.FILL_RULES[columns[i].type][rulesIndex];
					ReadRequiredInformationForColumnFilling(columns[i], mySqlConnection);
				}
				Console.Clear();
				int rowCount = UserAsk.Number("Сколько записей добавить в таблицу: ");

				Console.Clear();
				for (int i = 0; i < rowCount; ++i)
				{
					DrawTextProgressBar(i, rowCount);
					String queryString = MakeQueryString(mySqlConnection, tableName, columns);
					logFile.AppendLog("[" + mySqlConnectionStringBuilder.Database + "] " + queryString);
					ExecuteMySqlQuery(mySqlConnection, queryString);
				}
				DrawTextProgressBar(rowCount, rowCount);
				Console.ResetColor();
				Console.WriteLine();
				mySqlConnection.Close();
			}
			logFile.Flush();
			logFile.Close();
		}
	}
}
