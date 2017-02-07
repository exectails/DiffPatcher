namespace DiffPatchCreator
{
	partial class FrmMain
	{
		/// <summary>
		/// Erforderliche Designervariable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Verwendete Ressourcen bereinigen.
		/// </summary>
		/// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Vom Windows Form-Designer generierter Code

		/// <summary>
		/// Erforderliche Methode für die Designerunterstützung.
		/// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
		/// </summary>
		private void InitializeComponent()
		{
			this.label1 = new System.Windows.Forms.Label();
			this.TxtOldPath = new System.Windows.Forms.TextBox();
			this.BtnBrowseOld = new System.Windows.Forms.Button();
			this.FolderBrowser = new System.Windows.Forms.FolderBrowserDialog();
			this.BtnBrowseNew = new System.Windows.Forms.Button();
			this.TxtNewPath = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.BtnCreate = new System.Windows.Forms.Button();
			this.BtnBrowsePatchFilename = new System.Windows.Forms.Button();
			this.TxtPatchFileName = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.SaveFile = new System.Windows.Forms.SaveFileDialog();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(60, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Old version";
			// 
			// TxtOldPath
			// 
			this.TxtOldPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
			| System.Windows.Forms.AnchorStyles.Right)));
			this.TxtOldPath.Location = new System.Drawing.Point(15, 25);
			this.TxtOldPath.Name = "TxtOldPath";
			this.TxtOldPath.Size = new System.Drawing.Size(211, 20);
			this.TxtOldPath.TabIndex = 1;
			// 
			// BtnBrowseOld
			// 
			this.BtnBrowseOld.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.BtnBrowseOld.Location = new System.Drawing.Point(232, 25);
			this.BtnBrowseOld.Name = "BtnBrowseOld";
			this.BtnBrowseOld.Size = new System.Drawing.Size(25, 20);
			this.BtnBrowseOld.TabIndex = 2;
			this.BtnBrowseOld.Text = "...";
			this.BtnBrowseOld.UseVisualStyleBackColor = true;
			this.BtnBrowseOld.Click += new System.EventHandler(this.BtnBrowseOld_Click);
			// 
			// BtnBrowseNew
			// 
			this.BtnBrowseNew.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.BtnBrowseNew.Location = new System.Drawing.Point(232, 69);
			this.BtnBrowseNew.Name = "BtnBrowseNew";
			this.BtnBrowseNew.Size = new System.Drawing.Size(25, 20);
			this.BtnBrowseNew.TabIndex = 5;
			this.BtnBrowseNew.Text = "...";
			this.BtnBrowseNew.UseVisualStyleBackColor = true;
			this.BtnBrowseNew.Click += new System.EventHandler(this.BtnBrowseNew_Click);
			// 
			// TxtNewPath
			// 
			this.TxtNewPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
			| System.Windows.Forms.AnchorStyles.Right)));
			this.TxtNewPath.Location = new System.Drawing.Point(15, 69);
			this.TxtNewPath.Name = "TxtNewPath";
			this.TxtNewPath.Size = new System.Drawing.Size(211, 20);
			this.TxtNewPath.TabIndex = 4;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(12, 53);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(66, 13);
			this.label2.TabIndex = 3;
			this.label2.Text = "New version";
			// 
			// BtnCreate
			// 
			this.BtnCreate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
			| System.Windows.Forms.AnchorStyles.Right)));
			this.BtnCreate.Location = new System.Drawing.Point(15, 151);
			this.BtnCreate.Name = "BtnCreate";
			this.BtnCreate.Size = new System.Drawing.Size(242, 35);
			this.BtnCreate.TabIndex = 6;
			this.BtnCreate.Text = "Create";
			this.BtnCreate.UseVisualStyleBackColor = true;
			this.BtnCreate.Click += new System.EventHandler(this.BtnCreate_Click);
			// 
			// BtnBrowsePatchFilename
			// 
			this.BtnBrowsePatchFilename.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.BtnBrowsePatchFilename.Location = new System.Drawing.Point(232, 113);
			this.BtnBrowsePatchFilename.Name = "BtnBrowsePatchFilename";
			this.BtnBrowsePatchFilename.Size = new System.Drawing.Size(25, 20);
			this.BtnBrowsePatchFilename.TabIndex = 9;
			this.BtnBrowsePatchFilename.Text = "...";
			this.BtnBrowsePatchFilename.UseVisualStyleBackColor = true;
			this.BtnBrowsePatchFilename.Click += new System.EventHandler(this.BtnBrowsePatchFilename_Click);
			// 
			// TxtPatchFileName
			// 
			this.TxtPatchFileName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
			| System.Windows.Forms.AnchorStyles.Right)));
			this.TxtPatchFileName.Location = new System.Drawing.Point(15, 113);
			this.TxtPatchFileName.Name = "TxtPatchFileName";
			this.TxtPatchFileName.Size = new System.Drawing.Size(211, 20);
			this.TxtPatchFileName.TabIndex = 8;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(12, 97);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(85, 13);
			this.label3.TabIndex = 7;
			this.label3.Text = "Patch File Name";
			// 
			// SaveFile
			// 
			this.SaveFile.Filter = "Zip-File|*.zip";
			// 
			// FrmMain
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(272, 198);
			this.Controls.Add(this.BtnBrowsePatchFilename);
			this.Controls.Add(this.TxtPatchFileName);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.BtnCreate);
			this.Controls.Add(this.BtnBrowseNew);
			this.Controls.Add(this.TxtNewPath);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.BtnBrowseOld);
			this.Controls.Add(this.TxtOldPath);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FrmMain";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Diff Patch Creator";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox TxtOldPath;
		private System.Windows.Forms.Button BtnBrowseOld;
		private System.Windows.Forms.FolderBrowserDialog FolderBrowser;
		private System.Windows.Forms.Button BtnBrowseNew;
		private System.Windows.Forms.TextBox TxtNewPath;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button BtnCreate;
		private System.Windows.Forms.Button BtnBrowsePatchFilename;
		private System.Windows.Forms.TextBox TxtPatchFileName;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.SaveFileDialog SaveFile;
	}
}

