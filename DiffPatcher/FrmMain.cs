using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
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
		private const string VersionFileName = "version";
		private const string TempDirName = "tmp_patch";
		private const string ChangesFileName = "changes.txt";
		private const string FilesDirName = "files";
		private const string PatchFileExtension = ".patch";

		private DiffPatcherConf _conf;
		private PatchList _patchList;

		/// <summary>
		/// Creates new instance.
		/// </summary>
		public FrmMain()
		{
			this.InitializeComponent();
		}

		/// <summary>
		/// Runs action either directly or via Invoke if necessary.
		/// </summary>
		/// <param name="action"></param>
		private void InvokeIfRequired(Action action)
		{
			if (this.InvokeRequired)
				this.Invoke((MethodInvoker)delegate { action(); });
			else
				action();
		}

		/// <summary>
		/// Shows error in message box and resets form.
		/// </summary>
		/// <param name="message"></param>
		private void ShowError(string message)
		{
			MessageBox.Show(message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
			this.SetStatus("Error.");
			this.UpdateProgress(0, 0);
		}

		/// <summary>
		/// Sets status text.
		/// </summary>
		/// <param name="format"></param>
		/// <param name="args"></param>
		private void SetStatus(string format, params object[] args)
		{
			var text = (args.Length == 0 ? format : string.Format(format, args));
			this.InvokeIfRequired(() => this.LblStatus.Text = text);
		}

		/// <summary>
		/// Sets the buttons according to the parameters.
		/// </summary>
		/// <param name="patchEnabled"></param>
		/// <param name="startEnabled"></param>
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

		/// <summary>
		/// Called when form is loaded.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void FrmMain_Load(object sender, EventArgs e)
		{
			var assemblyName = Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly().Location);
			var confFileName = assemblyName + ".conf";

			if (!File.Exists(confFileName))
			{
				this.ShowError("File not found: " + confFileName);
				this.Close();
				return;
			}

			try
			{
				_conf = new DiffPatcherConf();
				_conf.Load(confFileName);
			}
			catch (Exception ex)
			{
				this.ShowError("Failed to read configuration, error: " + ex.Message);
				this.Close();
				return;
			}

			this.ToggleButtons(false, false);

			Task.Run(() =>
			{
				var patchListUri = _conf.PatchUri + _conf.PatchList;
				this.CheckPatches(patchListUri);
			});
		}

		/// <summary>
		/// Checks for new patches.
		/// </summary>
		/// <param name="patchListUri"></param>
		private void CheckPatches(string patchListUri)
		{
			this.SetStatus("Checking for updates...");

			try
			{
				var patchList = this.GetPatchList(patchListUri);
				var highestVersion = this.GetHighestVersion(patchList);
				var version = this.GetLocalVersion();
				var diff = (highestVersion - version);

				if (diff > 0)
				{
					this.SetStatus("{0} update(s) found.", diff);
					this.ToggleButtons(true, false);

					_patchList = patchList;
				}
				else
				{
					this.SetStatus("No updates found.");
					this.ToggleButtons(false, true);
				}
			}
			catch (Exception ex)
			{
				this.ShowError("Failed to check for updates, error: " + ex.Message);
				this.ToggleButtons(false, false);
			}
		}

		/// <summary>
		/// Downloads and returns patch list.
		/// </summary>
		/// <param name="uri"></param>
		/// <returns></returns>
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

		/// <summary>
		/// Returns the patch with the highest version in list.
		/// </summary>
		/// <param name="patchList"></param>
		/// <returns></returns>
		private int GetHighestVersion(PatchList patchList)
		{
			return patchList.Max(a => a.Version);
		}

		/// <summary>
		/// Returns the local patch version.
		/// </summary>
		/// <returns></returns>
		private int GetLocalVersion()
		{
			if (!File.Exists(VersionFileName))
				this.UpdateLocalVersion(0);

			return Convert.ToInt32(File.ReadAllText(VersionFileName));
		}

		/// <summary>
		/// Changes local patch version.
		/// </summary>
		/// <param name="version"></param>
		private void UpdateLocalVersion(int version)
		{
			File.WriteAllText(VersionFileName, version.ToString());
		}

		/// <summary>
		/// Returns true if URI exists.
		/// </summary>
		/// <param name="uri"></param>
		/// <returns></returns>
		private bool DoesUriExist(string uri)
		{
			try
			{
				var request = WebRequest.Create(new Uri(uri));
				request.Method = "HEAD";

				using (var response = request.GetResponse())
					return true;
			}
			catch (WebException)
			{
				return false;
			}
		}

		/// <summary>
		/// Downloads patch from given URI and extracts it to temp path.
		/// </summary>
		/// <param name="uri"></param>
		/// <param name="fileName"></param>
		/// <param name="tmpPath"></param>
		private void DownloadAndExtractPatch(string uri, string fileName, string tmpPath)
		{
			try
			{
				Exception downloadException = null;
				var wc = new WebClient();

				wc.DownloadProgressChanged += (sender, args) =>
				{
					this.UpdateProgress(args.ProgressPercentage, 100);
				};

				wc.DownloadFileCompleted += (sender, args) =>
				{
					lock (args.UserState)
					{
						this.UpdateProgress(1, 1);
						Monitor.Pulse(args.UserState);
					}

					downloadException = args.Error;
				};

				// To get the progress while using synchronous downloading
				// the thread is locked using lock and Monitor, to be
				// unlocked from DownloadFileCompleted.
				var syncLock = new Object();
				lock (syncLock)
				{
					wc.DownloadFileAsync(new Uri(uri), fileName, syncLock);
					Monitor.Wait(syncLock);
				}

				if (downloadException != null)
					throw downloadException;
			}
			catch (WebException ex)
			{
				throw new WebException("Failed to download '" + fileName + "', error: " + ex.Message);
			}

			if (Directory.Exists(tmpPath))
				Directory.Delete(tmpPath, true);

			using (var zip = new ZipFile(fileName))
			{
				zip.ExtractAll(tmpPath);
			}

			File.Delete(fileName);
		}

		/// <summary>
		/// Parses patch list from temp path and populates out lists.
		/// </summary>
		/// <param name="tempPath"></param>
		/// <param name="added"></param>
		/// <param name="removed"></param>
		/// <param name="changed"></param>
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

		/// <summary>
		/// Updates progress bar.
		/// </summary>
		/// <param name="value"></param>
		/// <param name="max"></param>
		private void UpdateProgress(int value, int max)
		{
			this.InvokeIfRequired(() =>
			{
				this.ProgressBar.Maximum = max;
				this.ProgressBar.Value = value;
			});
		}

		/// <summary>
		/// Applies patch from given folder.
		/// </summary>
		/// <param name="patchDirPath"></param>
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
				process.StartInfo.Arguments = string.Format("-v -d -f -s {0} {1} {2}", oldFilePath, patchFilePath, outFilePath);
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

		/// <summary>
		/// Throws if patch can't be applied.
		/// </summary>
		/// <param name="patchDirPath"></param>
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

		/// <summary>
		/// Removes given folder if it exists.
		/// </summary>
		/// <param name="tempPath"></param>
		private void RemoveTempFolder(string tempPath)
		{
			if (Directory.Exists(tempPath))
				Directory.Delete(tempPath, true);
		}

		/// <summary>
		/// Called when Patch is clicked.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
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

		/// <summary>
		/// Downloads all updates and applies them.
		/// </summary>
		private void DownloadAndApplyUpdates()
		{
			try
			{
				var version = this.GetLocalVersion();
				var list = _patchList.OrderBy(a => a.Version).SkipWhile(a => a.Version <= version);

				foreach (var patchFile in list)
				{
					var patchFileName = patchFile.FileName;
					var patchFileUri = Path.Combine(_conf.PatchUri, patchFileName);

					this.SetStatus("Downloading {0}...", patchFileName);
					this.DownloadAndExtractPatch(patchFileUri, patchFileName, TempDirName);

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
			catch (WebException ex)
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

		/// <summary>
		/// Called after all updates where applied.
		/// </summary>
		private void UpdateComplete()
		{
			this.SetStatus("Update complete.");
			this.UpdateProgress(1, 1);
			this.InvokeIfRequired(() =>
			{
				this.ToggleButtons(false, true);
			});
		}

		/// <summary>
		/// Called when Start is clicked.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void BtnStart_Click(object sender, EventArgs e)
		{
			var exe = _conf.Exe;
			var arguments = _conf.Arguments;

			if (!File.Exists(exe))
			{
				this.ShowError("File not found: " + exe);
				return;
			}

			this.ToggleButtons(false, false);

			try
			{
				var process = new Process();
				process.StartInfo.FileName = exe;
				process.StartInfo.Arguments = arguments;
				process.Start();
			}
			catch (Exception ex)
			{
				this.ShowError("Failed to start '" + exe + "', error: " + ex.Message);
				return;
			}

			this.Close();
		}
	}

	/// <summary>
	/// Represents a patch file.
	/// </summary>
	public class PatchFile
	{
		/// <summary>
		/// The version this file patches to.
		/// </summary>
		public int Version { get; private set; }

		/// <summary>
		/// Patch's file name.
		/// </summary>
		public string FileName { get; private set; }

		/// <summary>
		/// Creates new instance.
		/// </summary>
		/// <param name="version"></param>
		/// <param name="fileName"></param>
		public PatchFile(int version, string fileName)
		{
			this.Version = version;
			this.FileName = fileName;
		}

		/// <summary>
		/// Returns string representation of PatchFile.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return string.Format("[{0}, {1}]", this.Version, this.FileName);
		}
	}

	/// <summary>
	/// List of PatchFiles.
	/// </summary>
	public class PatchList : List<PatchFile>
	{
	}
}
