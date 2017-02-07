// Copyright (c) Aura development team - Licensed under GNU GPL
// For more information, see license file in the main folder

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Yggdrasil.IO;

namespace Yggdrasil.Configuration
{
	/// <summary>
	/// Configuration options manager.
	/// </summary>
	/// <remarks>
	/// Uses <see cref="FileReader"/> to read conf files, that are parsed in key:value pairs.
	/// Separating character is a colon ':'.
	/// </remarks>
	public class ConfFile
	{
		protected readonly Dictionary<string, string> _options;

		/// <summary>
		/// Initializes instance.
		/// </summary>
		public ConfFile()
		{
			_options = new Dictionary<string, string>();
		}

		/// <summary>
		/// Loads all options in the file and included files.
		/// Does nothing if file doesn't exist.
		/// </summary>
		/// <param name="filePath"></param>
		public void Include(string filePath)
		{
			if (!File.Exists(filePath))
				return;

			this.LoadFile(filePath);
		}

		/// <summary>
		/// Loads all options in the file and included files.
		/// Throws FileNotFoundException if file couldn't be found.
		/// </summary>
		/// <param name="filePath"></param>
		public void Require(string filePath)
		{
			this.LoadFile(filePath);
		}

		/// <summary>
		/// Loads all options in the file and included files.
		/// </summary>
		/// <param name="filePath"></param>
		private void LoadFile(string filePath)
		{
			using (var fr = new FileReader(filePath))
			{
				foreach (var line in fr)
				{
					int pos = -1;

					// Check for seperator
					if ((pos = line.Value.IndexOf(':')) < 0)
						return;
					lock (_options)
						_options[line.Value.Substring(0, pos).Trim()] = line.Value.Substring(pos + 1).Trim();
				}
			}
		}

		/// <summary>
		/// Returns the option as bool, or the default value if the option
		/// doesn't exist or is invalid.
		/// </summary>
		/// <param name="option"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public bool GetBool(string option, bool defaultValue = false)
		{
			string value;
			lock (_options)
			{
				if (!_options.TryGetValue(option, out value))
					return defaultValue;
			}

			value = value.ToLower().Trim();

			return (value == "1" || value == "yes" || value == "true");
		}

		/// <summary>
		/// Returns the option as byte, or the default value if the option
		/// doesn't exist or is invalid.
		/// </summary>
		/// <param name="option"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public byte GetByte(string option, byte defaultValue = 0)
		{
			string value;
			lock (_options)
			{
				if (!_options.TryGetValue(option, out value))
					return defaultValue;
			}

			byte ret;
			if (byte.TryParse(value, out ret))
				return ret;

			return defaultValue;
		}

		/// <summary>
		/// Returns the option as short, or the default value if the option
		/// doesn't exist or is invalid.
		/// </summary>
		/// <param name="option"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public short GetShort(string option, short defaultValue = 0)
		{
			string value;
			lock (_options)
			{
				if (!_options.TryGetValue(option, out value))
					return defaultValue;
			}

			short ret;
			if (short.TryParse(value, out ret))
				return ret;

			return defaultValue;
		}

		/// <summary>
		/// Returns the option as int, or the default value if the option
		/// doesn't exist or is invalid.
		/// </summary>
		/// <param name="option"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public int GetInt(string option, int defaultValue = 0)
		{
			string value;
			lock (_options)
			{
				if (!_options.TryGetValue(option, out value))
					return defaultValue;
			}

			int ret;
			if (int.TryParse(value, out ret))
				return ret;

			return defaultValue;
		}

		/// <summary>
		/// Returns the option as long, or the default value if the option
		/// doesn't exist or is invalid.
		/// </summary>
		/// <param name="option"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public long GetLong(string option, long defaultValue = 0)
		{
			string value;
			lock (_options)
			{
				if (!_options.TryGetValue(option, out value))
					return defaultValue;
			}

			long ret;
			if (long.TryParse(value, out ret))
				return ret;

			return defaultValue;
		}

		/// <summary>
		/// Returns the option as string, or the default value if the option
		/// doesn't exist or is invalid.
		/// </summary>
		/// <param name="option"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public string GetString(string option, string defaultValue = "")
		{
			string value;
			lock (_options)
			{
				if (!_options.TryGetValue(option, out value))
					return defaultValue;
			}

			return value;
		}

		/// <summary>
		/// Returns the option as float, or the default value if the option
		/// doesn't exist or is invalid.
		/// </summary>
		/// <param name="option"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public float GetFloat(string option, float defaultValue = 0)
		{
			string value;
			lock (_options)
			{
				if (!_options.TryGetValue(option, out value))
					return defaultValue;
			}

			float ret;
			if (float.TryParse(value, out ret))
				return ret;

			return defaultValue;
		}

		/// <summary>
		/// Returns the option as a DateTime, or the default value if the option
		/// doesn't exist or is invalid.
		/// </summary>
		/// <remarks>
		/// For acceptable value formatting, see <see href="http://msdn.microsoft.com/en-us/library/system.datetime.parse(v=vs.110).aspx">MSDN</see>.
		/// </remarks>
		/// <param name="option"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public DateTime GetDateTime(string option, DateTime defaultValue = default(DateTime))
		{
			string value;
			lock (_options)
			{
				if (!_options.TryGetValue(option, out value))
					return defaultValue;
			}

			DateTime ret;
			if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out ret))
				return ret;

			return defaultValue;

		}

		/// <summary>
		/// Returns the option as an enum, or the default value if the option
		/// doesn't exist or is invalid.
		/// </summary>
		/// <typeparam name="T">The type of the enum</typeparam>
		/// <param name="option"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public T GetEnum<T>(string option, T defaultValue = default(T)) where T : struct
		{
			var type = typeof(T);

			if (!type.IsEnum)
				throw new NotSupportedException("Type " + typeof(T) + " is not an enum.");

			string value;
			lock (_options)
			{
				if (!_options.TryGetValue(option, out value))
					return defaultValue;
			}

			if (Enum.IsDefined(type, value))
				return (T)Enum.Parse(type, value, true);

			return defaultValue;
		}
	}
}
