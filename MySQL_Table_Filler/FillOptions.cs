using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using MySql.Data.MySqlClient;

namespace MySQL_Table_Filler
{
	public struct FillOptions
	{
		public int rangeNumberStart;
		public int rangeNumberEnd;
		public DateTime rangeDateTimeStart;
		public DateTime rangeDateTimeEnd;
		public String fileName;
		public String tableName;

		static private Dictionary<String, List<String>> filesLines = new Dictionary<String, List<String>>();
		static private Dictionary<String, List<String>> tableColumnRows = new Dictionary<String, List<String>>();
		static private Random random = new Random();

		static public String GetRandomFromRange(int rangeNumberStart, int rangeNumberEnd)
		{
			return random.Next(rangeNumberStart, rangeNumberEnd + 1).ToString();
		}

		static public String GetRandomFromRange(DateTime rangeDateTimeStart, DateTime rangeDateTimeEnd)
		{
			DateTime result = rangeDateTimeStart.AddDays(random.Next((rangeDateTimeEnd - rangeDateTimeStart).Days)).AddHours(random.Next(0, 24)).AddMinutes(random.Next(0, 60)).AddSeconds(random.Next(0, 60));
			return result.Year.ToString() + "-" + result.Month + "-" + result.Day;
		}

		static public bool ReadList(String fileName)
		{
			if (!filesLines.ContainsKey(fileName))
			{
				filesLines.Add(fileName, new List<String>());
				try
				{
					using (StreamReader streamReader = new StreamReader(fileName))
					{
						String line = String.Empty;
						while ((line = streamReader.ReadLine()) != null)
						{
							filesLines[fileName].Add(line);
						}
						if (filesLines[fileName].Count == 0)
						{
							Console.WriteLine("Ошибка: Файл пуст.");
							return false;
						}
						return true;
					}
				}
				catch (Exception e)
				{
					Console.WriteLine("Ошибка: Файл '" + fileName + "' не может быть прочитан:");
					Console.WriteLine(e.Message);
					return false;
				}
			}
			return true;
		}

		static public String GetRandomFromList(String fileName)
		{
			return filesLines[fileName][random.Next(filesLines[fileName].Count)];
		}

		static public bool ReadAnotherTableColumn(MySqlConnection mySqlConnection, String tableName, String columnName)
		{
			if (!tableColumnRows.ContainsKey(tableName + columnName))
			{
				tableColumnRows.Add(tableName + columnName, new List<String>());
				MySqlCommand mySqlCommand = new MySqlCommand("SELECT " + columnName + " FROM " + tableName, mySqlConnection);
				try
				{
					using (MySqlDataReader mySqlDataReader = mySqlCommand.ExecuteReader())
					{
						while (mySqlDataReader.Read())
						{
							tableColumnRows[tableName + columnName].Add(mySqlDataReader.GetString(0));
						}
						if (tableColumnRows[tableName + columnName].Count == 0)
						{
							Console.WriteLine("Ошибка: Таблица пуста.");
							return false;
						}
						return true;
					}
				}
				catch (Exception e)
				{
					Console.WriteLine("Ошибка: Таблица '" + tableName + "' не может быть прочитана:");
					Console.WriteLine(e.Message);
					return false;
				}
			}
			return true;
		}

		static public String GetRandomFromAnotherTable(String tableName, String columnName)
		{
			return tableColumnRows[tableName + columnName][random.Next(tableColumnRows[tableName + columnName].Count)];
		}
	}
}
