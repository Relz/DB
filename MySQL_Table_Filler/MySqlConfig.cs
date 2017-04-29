using System;
using System.IO;
using System.Collections.Generic;

namespace MySQL_Table_Filler
{
	public class MySqlConfig
	{
		//private Dictionary<String, String> _config = new Dictionary<String, String>();
		public Dictionary<String, String> get { get; }
		private String _configFileName;
		private StreamReader _streamReader = null;

		public MySqlConfig(String configFileName)
		{
			_configFileName = configFileName;
			get = new Dictionary<String, String>();
			try
			{
				_streamReader = new StreamReader(_configFileName);
			}
			catch (Exception e)
			{
				Console.WriteLine("Файл '" + _configFileName + "' не может быть прочитан:");
				Console.WriteLine(e.Message);
			}
		}

		public void Print()
		{
			foreach (KeyValuePair<String, String> pair in get)
			{
				Console.WriteLine(pair.Key);
				Console.WriteLine(pair.Value);
			}
		}

		public bool Check(String[] requiredKeys)
		{
			bool result = true;
			foreach (String requiredKey in requiredKeys)
			{
				if (!get.ContainsKey(requiredKey) || get[requiredKey] == String.Empty)
				{
					result = false;
					Console.WriteLine("'" + requiredKey + "' key is required in config file: '" + _configFileName + "'");
				}
			}
			return result;
		}

		public void Read()
		{
			using (_streamReader)
			{
				String line = String.Empty;
				while ((line = _streamReader.ReadLine()) != null)
				{
					String[] keyPair = null;
					keyPair = line.Split(new char[] { '=' }, 2);
					get.Add(keyPair[0], keyPair[1]);
				}
			}
		}
	}
}
