using System;
using System.Collections.Generic;

namespace MySQL_Table_Filler
{
	static public class CONSTANT
	{
		static public readonly Dictionary<Type, FillRule[]> FILL_RULES = new Dictionary<Type, FillRule[]>()
		{
			{ typeof(UInt32), new FillRule[] {FillRule.DO_NOT_FILL, FillRule.RANDOM_FROM_RANGE, FillRule.RANDOM_FROM_LIST, FillRule.RANDOM_FROM_ANOTHER_TABLE} },
			{ typeof(String), new FillRule[] {FillRule.DO_NOT_FILL, FillRule.RANDOM_FROM_LIST} },
			{ typeof(DateTime), new FillRule[] {FillRule.DO_NOT_FILL, FillRule.RANDOM_FROM_RANGE, FillRule.RANDOM_FROM_LIST} },
		};

		static public readonly Dictionary<FillRule, String> RULE_EXPLAINATION = new Dictionary<FillRule, String>()
		{
			{ FillRule.DO_NOT_FILL, "Не заполнять" },
			{ FillRule.RANDOM_FROM_RANGE, "Случайное из диапазона" },
			{ FillRule.RANDOM_FROM_LIST, "Случайное из списка" },
			{ FillRule.RANDOM_FROM_ANOTHER_TABLE, "Случайное из одноимённого столбца другой таблицы" }
		};

		static public readonly Dictionary<Type, String> TYPE_EXPLAINATION = new Dictionary<Type, String>()
		{
			{ typeof(UInt32), "Unsigned INT" },
			{ typeof(String), "VARCHAR" },
			{ typeof(DateTime), "DATETIME" },
		};
		static public class LOG
		{
			static public readonly String FILE_NAME = "logs";
		}

		static public class CONFIG
		{
			static public readonly String FILE_NAME = "db_config";
			static public readonly String[] REQUIRED_KEYS = { "hostname", "database", "username", "password" };
		}
	}
}
