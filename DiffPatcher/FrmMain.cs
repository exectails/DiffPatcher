using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Yggdrasil.Configuration;
using Yggdrasil.IO;

namespace DiffPatcher
{
	public partial class FrmMain : Form
	{
		private const string XDeltaFileName = "xdelta3.exe";
		private const string ConfFileName = "DiffPatcher.conf";
		private const string VersionFileName = "version";
		private const string TempDirName = "tmp_patch";
		private const string ChangesFileName = "changes.txt";
		private const string FilesDirName = "files";
		private const string PatchFileExtension = ".patch";

		private DiffPatcherConf _conf;
		private PatchList _patchList;

		public FrmMain()
		{
			InitializeComponent();
		}

		private void InvokeIfRequired(Action action)
		{
			if (this.InvokeRequired)
				this.Invoke((MethodInvoker)delegate { action(); });
			else
				action();
		}

		private void ShowError(string message)
		{
			MessageBox.Show(message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
			this.SetStatus("Error.");
			this.UpdateProgress(0, 0);
		}

		private void SetStatus(string format, params object[] args)
		{
			this.InvokeIfRequired(() => this.LblStatus.Text = string.Format(format, args));
		}

		private void ToggleButtons(bool patchEnabled, bool startEnabled)
		{
			if (string.IsNullOrWhiteSpace(_conf.Exe))
				startEnabled = false;

			this.InvokeIfRequired(() =>
			{
				this.BtnPatch.Enabled = patchEnabled;
				this.BtnStart.Enabled = startEnabled;
			});
		}

		private void FrmMain_Load(object sender, EventArgs e)
		{
			if (!File.Exists(ConfFileName))
			{
				this.ShowError("File not found: " + ConfFileName);
				this.Close();
				return;
			}

			try
			{
				_conf = new DiffPatcherConf();
				_conf.Load(ConfFileName);
			}
			catch (Exception ex)
			{
				this.ShowError("Failed to read configuration, error: " + ex.Message);
				this.Close();
				return;
			}

			this.ToggleButtons(false, false);

			var patchListUri = _conf.PatchUri + _conf.PatchList;
			this.CheckPatches(patchListUri);
		}

		private void CheckPatches(string patchListUri)
		{
			this.SetStatus("Checking patches...");

			var patchList = this.GetPatchList(patchListUri);
			var highestVersion = this.GetHighestVersion(patchList);
			var version = this.GetLocalVersion();
			var diff = (highestVersion - version);

			if (diff > 0)
			{
				this.SetStatus("{0} patch(es) found.", diff);
				this.ToggleButtons(true, false);

				_patchList = patchList;
			}
			else
			{
				this.SetStatus("No patches found.");
				this.ToggleButtons(false, true);
			}
		}

		private PatchList GetPatchList(string uri)
		{
			var patchListFileName = Path.GetFileName(uri);

			var wc = new WebClient();
			wc.DownloadFile(uri, patchListFileName);

			var patchList = new PatchList();
			using (var fr = new FileReader(patchListFileName))
			{
				foreach (var line in fr)
				{
					var index = line.Value.IndexOf(" ");
					var version = Convert.ToInt32(line.Value.Substring(0, index).Trim());
					var fileName = line.Value.Substring(index + 1).Trim();

					var patchFile = new PatchFile(version, fileName);

					patchList.RemoveAll(a => a.Version == version);
					patchList.Add(patchFile);
				}
			}

			return patchList;
		}

		private int GetHighestVersion(PatchList patchList)
		{
			return patchList.Max(a => a.Version);
		}

		private int GetLocalVersion()
		{
			if (!File.Exists(VersionFileName))
				this.UpdateLocalVersion(0);

			return Convert.ToInt32(File.ReadAllText(VersionFileName));
		}

		private void UpdateLocalVersion(int version)
		{
			File.WriteAllText(VersionFileName, version.ToString());
		}

		private void DownloadAndExtractPatch(WebClient wc, string uri, string fileName, string tmpPath)
		{
			wc.DownloadFile(uri, fileName);
			wc.DownloadProgressChanged += (sender, args) => { this.UpdateProgress(args.ProgressPercentage, 100); };

			if (Directory.Exists(tmpPath))
				Directory.Delete(tmpPath, true);

			using (var zip = new ZipFile(fileName))
			{
				zip.ExtractAll(TempDirName);
			}

			File.Delete(fileName);
		}

		private void GetFileLists(string tempPath, out List<string> added, out List<string> removed, out List<string> changed)
		{
			var changesPath = Path.Combine(tempPath, ChangesFileName);

			added = new List<string>();
			removed = new List<string>();
			changed = new List<string>();

			using (var fr = new FileReader(changesPath))
			{
				foreach (var line in fr)
				{
					if (line.Value.Length < 2)
						continue;

					var operation = line.Value[0];
					var fileName = line.Value.Substring(1);

					switch (operation)
					{
						case '+': added.Add(fileName); break;
						case '-': removed.Add(fileName); break;
						case '*': changed.Add(fileName); break;
					}
				}
			}
		}

		private void UpdateProgress(int value, int max)
		{
			this.InvokeIfRequired(() =>
			{
				this.ProgressBar.Maximum = max;
				this.ProgressBar.Value = value;
			});
		}

		private void ApplyPatch(string patchDirPath)
		{
			var filesPath = Path.Combine(TempDirName, FilesDirName);

			List<string> added, removed, changed;
			this.GetFileLists(patchDirPath, out added, out removed, out changed);

			var progress = 0;
			var progressMax = (added.Count() + removed.Count() + changed.Count());

			foreach (var fileName in added)
			{
				var source = Path.Combine(filesPath, fileName);
				var target = fileName;
				var targetDir = Path.GetDirectoryName(fileName);

				if (!string.IsNullOrWhiteSpace(targetDir) && !Directory.Exists(targetDir))
					Directory.CreateDirectory(targetDir);

				File.Copy(source, target, true);

				this.UpdateProgress(++progress, progressMax);
			}

			foreach (var fileName in removed)
			{
				var path = fileName;

				if (File.Exists(path))
					File.Delete(path);

				this.UpdateProgress(++progress, progressMax);
			}

			foreach (var fileName in changed)
			{
				var oldFilePath = fileName;
				var patchFilePath = Path.Combine(filesPath, fileName + PatchFileExtension);
				var outFilePath = fileName + ".patched";

				var process = new Process();
				process.StartInfo.FileName = XDeltaFileName;
				process.StartInfo.Arguments = string.Format("-v -d -s {0} {1} {2}", oldFilePath, patchFilePath, outFilePath);
				process.StartInfo.UseShellExecute = false;
				process.StartInfo.CreateNoWindow = true;
				process.StartInfo.RedirectStandardOutput = true;
				process.StartInfo.RedirectStandardError = true;
				process.Start();
				process.WaitForExit();

				if (process.ExitCode != 0)
				{
					var stdout = process.StandardOutput.ReadToEnd();
					var stderr = process.StandardError.ReadToEnd();

					throw new Exception("Failed to apply patch for '" + fileName + "', xdelta error: " + stderr);
				}

				File.Delete(fileName);
				File.Move(outFilePath, fileName);

				this.UpdateProgress(++progress, progressMax);
			}
		}

		private void AssertPatchApplicable(string patchDirPath)
		{
			if (!File.Exists(XDeltaFileName))
				throw new FileNotFoundException("File not found: " + XDeltaFileName);

			List<string> added, removed, changed;
			this.GetFileLists(patchDirPath, out added, out removed, out changed);

			foreach (var fileName in changed)
			{
				if (!File.Exists(fileName))
					throw new FileNotFoundException("File to be patched not found: " + fileName);
			}
		}

		private void RemoveTempFolder(string tempPath)
		{
			if (Directory.Exists(tempPath))
				Directory.Delete(tempPath, true);
		}

		private void BtnPatch_Click(object sender, EventArgs e)
		{
			this.ToggleButtons(false, false);

			if (_patchList == null || !_patchList.Any())
			{
				this.UpdateComplete();
				return;
			}

			Task.Run(() => DownloadAndApplyUpdates());
		}

		private void DownloadAndApplyUpdates()
		{
			try
			{
				var version = this.GetLocalVersion();
				var list = _patchList.OrderBy(a => a.Version).SkipWhile(a => a.Version <= version);
				var wc = new WebClient();

				foreach (var patchFile in list)
				{
					var patchFileName = patchFile.FileName;
					var patchFileUri = Path.Combine(_conf.PatchUri, patchFileName);

					this.SetStatus("Downloading {0}...", patchFileName);
					this.DownloadAndExtractPatch(wc, patchFileUri, patchFileName, TempDirName);

					this.SetStatus("Checking {0}...", patchFileName);
					this.AssertPatchApplicable(TempDirName);

					this.SetStatus("Applying {0}...", patchFileName);
					this.ApplyPatch(TempDirName);
					this.RemoveTempFolder(TempDirName);

					this.UpdateLocalVersion(patchFile.Version);
				}
			}
			catch (FileNotFoundException ex)
			{
				this.RemoveTempFolder(TempDirName);
				this.ShowError(ex.Message);
				return;
			}
			catch (Exception ex)
			{
				this.RemoveTempFolder(TempDirName);
				this.ShowError(ex.ToString());
				return;
			}

			this.UpdateComplete();
		}

		private void UpdateComplete()
		{
			this.SetStatus("Update complete.");
			this.UpdateProgress(1, 1);
			this.InvokeIfRequired(() =>
			{
				this.ToggleButtons(false, true);
			});
		}

		private void BtnStart_Click(object sender, EventArgs e)
		{
			var exe = _conf.Exe;

			if (!File.Exists(exe))
			{
				this.ShowError("File not found: " + exe);
				return;
			}

			this.ToggleButtons(false, false);
			Process.Start(exe);
		}
	}

	public class PatchFile
	{
		public int Version { get; private set; }
		public string FileName { get; private set; }

		public PatchFile(int version, string fileName)
		{
			this.Version = version;
			this.FileName = fileName;
		}

		public override string ToString()
		{
			return string.Format("[{0}, {1}]", this.Version, this.FileName);
		}
	}

	public class PatchList : List<PatchFile>
	{
	}
}
