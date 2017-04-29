using System;
using System.IO;
using System.Net.WebSockets;
namespace MySQL_Table_Filler
{
	public class LogFile
	{
		private StreamWriter _streamWriter;

		public LogFile(String logFileName)
		{
			try
			{
				_streamWriter = new StreamWriter(logFileName, true);
			}
			catch (Exception e)
			{
				Console.WriteLine("Файл '" + logFileName + "' не может быть создан:");
				Console.WriteLine(e.Message);
			}
		}

		public void AppendLog(String logMessage)
		{
			_streamWriter.WriteLine("[" + DateTime.Now + "] " + logMessage);
		}

		public void Flush()
		{
			_streamWriter.Flush();
		}

		public void Close()
		{
			_streamWriter.Close();
		}
	}
}
