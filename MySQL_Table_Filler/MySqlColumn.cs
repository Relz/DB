using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Linq;

namespace MySQL_Table_Filler
{
	public class MySqlColumn
	{
		public String name { get; set; }
		public Type type { get; set; }
		public FillRule fillRule 
		{
			get
			{
				return _fillRule;
			}
			set
			{
				_fillRule = value;
				if (fillRule == FillRule.RANDOM_FROM_RANGE)
				{
					if (type == typeof(UInt32))
					{
						_fillOptions.rangeNumberStart = UserAsk.Number("Введите начало числового диапазона: ");
						_fillOptions.rangeNumberEnd = UserAsk.Number("Введите конец числового диапазона: ");
					}
					else if (type == typeof(DateTime))
					{
						_fillOptions.rangeDateTimeStart = UserAsk.Date("Введите начало диапазона даты(YYYY-MM-DD): ");
						_fillOptions.rangeDateTimeEnd = UserAsk.Date("Введите конец диапазона даты(YYYY-MM-DD): ");
					}
				}
				else if (fillRule == FillRule.RANDOM_FROM_LIST)
				{
					_fillOptions.fileName = UserAsk.String("Введите имя файла со списком: ");
				}
				else if (fillRule == FillRule.RANDOM_FROM_ANOTHER_TABLE)
				{
					_fillOptions.tableName = UserAsk.String("Введите имя таблицы с одноимённым названием столбца: ");
				}
			} 
		}
		public FillOptions fillOptions 
		{ 
			get
			{
				return _fillOptions;
			}
		}

		private FillOptions _fillOptions;
		private FillRule _fillRule = FillRule.DO_NOT_FILL;

		public MySqlColumn(String name, Type type)
		{
			this.name = name;
			this.type = type;
		}
	}
}
