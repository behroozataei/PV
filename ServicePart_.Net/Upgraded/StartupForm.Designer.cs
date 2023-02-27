
namespace RPC_Service_App
{
	partial class frmStartupRPC
	{

		#region "Upgrade Support "
		private static frmStartupRPC m_vb6FormDefInstance;
		private static bool m_InitializingDefInstance;
		public static frmStartupRPC DefInstance
		{
			get
			{
				if (m_vb6FormDefInstance == null || m_vb6FormDefInstance.IsDisposed)
				{
					m_InitializingDefInstance = true;
					m_vb6FormDefInstance = CreateInstance();
					m_InitializingDefInstance = false;
				}
				return m_vb6FormDefInstance;
			}
			set
			{
				m_vb6FormDefInstance = value;
			}
		}

		#endregion
		#region "Windows Form Designer generated code "
		public static frmStartupRPC CreateInstance()
		{
			frmStartupRPC theInstance = new frmStartupRPC();
			theInstance.Form_Load();
			return theInstance;
		}
		private string[] visualControls = new string[]{"components", "ToolTipMain", "Timer2", "Timer1", "cmdClose"};
		//Required by the Windows Form Designer
		private System.ComponentModel.IContainer components;
		public System.Windows.Forms.ToolTip ToolTipMain;
		public System.Windows.Forms.Timer Timer2;
		public System.Windows.Forms.Timer Timer1;
		public System.Windows.Forms.Button cmdClose;
		//NOTE: The following procedure is required by the Windows Form Designer
		//It can be modified using the Windows Form Designer.
		//Do not modify it using the code editor.
		[System.Diagnostics.DebuggerStepThrough()]
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmStartupRPC));
			this.ToolTipMain = new System.Windows.Forms.ToolTip(this.components);
			this.Timer2 = new System.Windows.Forms.Timer(components);
			this.Timer1 = new System.Windows.Forms.Timer(components);
			this.cmdClose = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// Timer2
			// 
			this.Timer2.Enabled = false;
			this.Timer2.Interval = 1000;
			// 
			// Timer1
			// 
			this.Timer1.Enabled = true;
			this.Timer1.Interval = 10000;
			this.Timer1.Tick += new System.EventHandler(this.Timer1_Tick);
			// 
			// cmdClose
			// 
			this.cmdClose.AllowDrop = true;
			this.cmdClose.BackColor = System.Drawing.SystemColors.Control;
			this.cmdClose.ForeColor = System.Drawing.SystemColors.ControlText;
			this.cmdClose.Location = new System.Drawing.Point(120, 48);
			this.cmdClose.Name = "cmdClose";
			this.cmdClose.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.cmdClose.Size = new System.Drawing.Size(81, 25);
			this.cmdClose.TabIndex = 0;
			this.cmdClose.Text = "Close";
			this.cmdClose.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
			this.cmdClose.UseVisualStyleBackColor = false;
			this.cmdClose.Click += new System.EventHandler(this.cmdClose_Click);
			// 
			// frmStartupRPC
			// 
			this.AllowDrop = true;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6, 13);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.Control;
			this.ClientSize = new System.Drawing.Size(312, 144);
			this.Controls.Add(this.cmdClose);
			this.Location = new System.Drawing.Point(4, 23);
			this.MaximizeBox = true;
			this.MinimizeBox = true;
			this.Name = "frmStartupRPC";
			this.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.Text = "Startup RPC Model Function";
			this.Closed += new System.EventHandler(this.Form_Closed);
			this.ResumeLayout(false);
		}
		#endregion
	}
}