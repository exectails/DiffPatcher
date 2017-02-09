using System;
using Yggdrasil.Configuration;

namespace DiffPatcher
{
	/// <summary>
	/// Represents DiffPatcher.conf.
	/// </summary>
	public class DiffPatcherConf : ConfFile
	{
		/// <summary>
		/// URI to the folder that contains the patch list and files.
		/// </summary>
		public string PatchUri { get; private set; }

		/// <summary>
		/// Name of the patch list file in patch folder.
		/// </summary>
		public string PatchList { get; private set; }

		/// <summary>
		/// Executable to start when the Start button is clicked.
		/// </summary>
		public string Exe { get; private set; }

		/// <summary>
		/// Arguments to pass to executable.
		/// </summary>
		public string Arguments { get; private set; }

		/// <summary>
		/// Loads conf options from given file.
		/// </summary>
		/// <param name="confFileName"></param>
		public void Load(string confFileName)
		{
			this.Require(confFileName);

			this.PatchUri = this.GetString("patch_uri");
			this.PatchList = this.GetString("patch_list");
			this.Exe = this.GetString("exe");
			this.Arguments = this.GetString("arguments");

			if (string.IsNullOrWhiteSpace(this.PatchUri))
				throw new ConfigException("Configuration value patch_uri must not be empty.");

			if (string.IsNullOrWhiteSpace(this.PatchList))
				throw new ConfigException("Configuration value patch_list must not be empty.");

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
