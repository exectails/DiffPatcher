using System;
using Yggdrasil.Configuration;

namespace DiffPatcher
{
	public class DiffPatcherConf : ConfFile
	{
		public string PatchUri { get; private set; }
		public string PatchList { get; private set; }

		public void Load(string confFileName)
		{
			this.Require(confFileName);

			this.PatchUri = this.GetString("patchUri");
			this.PatchList = this.GetString("patchList");

			if (string.IsNullOrWhiteSpace(this.PatchUri))
				throw new ConfigException("Configuration value patchUri must not be empty.");

			if (string.IsNullOrWhiteSpace(this.PatchList))
				throw new ConfigException("Configuration value patchList must not be empty.");

			if (!this.PatchUri.EndsWith("/"))
				this.PatchUri += "/";
		}
	}

	public class ConfigException : Exception
	{
		public ConfigException(string message)
			: base(message)
		{
		}
	}
}
