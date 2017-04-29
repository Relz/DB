using System;
namespace MySQL_Table_Filler
{
	public class UserAsk
	{
		static public int Number(String message)
		{
			int result = 0;
			Console.Write(message);
			String resultString = Console.ReadLine();
			if (!int.TryParse(resultString, out result))
			{
				Console.WriteLine("Ошибка. Не могу понять, какое число вы ввели");
				Environment.Exit(exitCode: 1);
			}
			return result;
		}

		static public String String(String message)
		{
			Console.Write(message);
			return Console.ReadLine();
		}

		static public DateTime Date(String message)
		{
			DateTime result = new DateTime(1, 1, 1);
			Console.Write(message);
			String resultString = Console.ReadLine();
			if (!DateTime.TryParse(resultString, out result))
			{
				Console.WriteLine("Ошибка. Не могу понять, какую дату вы ввели");
				Environment.Exit(exitCode: 1);
			}
			return result;
		}
	}
}
