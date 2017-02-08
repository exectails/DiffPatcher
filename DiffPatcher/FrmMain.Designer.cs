namespace DiffPatcher
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
			this.BtnPatch = new System.Windows.Forms.Button();
			this.ProgressBar = new System.Windows.Forms.ProgressBar();
			this.BtnStart = new System.Windows.Forms.Button();
			this.LblStatus = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// BtnPatch
			// 
			this.BtnPatch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.BtnPatch.Location = new System.Drawing.Point(246, 12);
			this.BtnPatch.Name = "BtnPatch";
			this.BtnPatch.Size = new System.Drawing.Size(75, 23);
			this.BtnPatch.TabIndex = 0;
			this.BtnPatch.Text = "Patch";
			this.BtnPatch.UseVisualStyleBackColor = true;
			this.BtnPatch.Click += new System.EventHandler(this.BtnPatch_Click);
			// 
			// ProgressBar
			// 
			this.ProgressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.ProgressBar.Location = new System.Drawing.Point(12, 46);
			this.ProgressBar.Name = "ProgressBar";
			this.ProgressBar.Size = new System.Drawing.Size(390, 12);
			this.ProgressBar.TabIndex = 1;
			// 
			// BtnStart
			// 
			this.BtnStart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.BtnStart.Location = new System.Drawing.Point(327, 12);
			this.BtnStart.Name = "BtnStart";
			this.BtnStart.Size = new System.Drawing.Size(75, 23);
			this.BtnStart.TabIndex = 2;
			this.BtnStart.Text = "Start";
			this.BtnStart.UseVisualStyleBackColor = true;
			this.BtnStart.Click += new System.EventHandler(this.BtnStart_Click);
			// 
			// LblStatus
			// 
			this.LblStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.LblStatus.AutoSize = true;
			this.LblStatus.Location = new System.Drawing.Point(12, 17);
			this.LblStatus.Name = "LblStatus";
			this.LblStatus.Size = new System.Drawing.Size(37, 13);
			this.LblStatus.TabIndex = 3;
			this.LblStatus.Text = "Status";
			// 
			// FrmMain
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(414, 70);
			this.Controls.Add(this.LblStatus);
			this.Controls.Add(this.BtnStart);
			this.Controls.Add(this.ProgressBar);
			this.Controls.Add(this.BtnPatch);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FrmMain";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Diff Patcher";
			this.Load += new System.EventHandler(this.FrmMain_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button BtnPatch;
		private System.Windows.Forms.ProgressBar ProgressBar;
		private System.Windows.Forms.Button BtnStart;
		private System.Windows.Forms.Label LblStatus;
	}
}

