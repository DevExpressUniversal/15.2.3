#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{                                                                   }
{                                                                   }
{       Copyright (c) 2000-2015 Developer Express Inc.              }
{       ALL RIGHTS RESERVED                                         }
{                                                                   }
{   The entire contents of this file is protected by U.S. and       }
{   International Copyright Laws. Unauthorized reproduction,        }
{   reverse-engineering, and distribution of all or any portion of  }
{   the code contained in this file is strictly prohibited and may  }
{   result in severe civil and criminal penalties and will be       }
{   prosecuted to the maximum extent possible under the law.        }
{                                                                   }
{   RESTRICTIONS                                                    }
{                                                                   }
{   THIS SOURCE CODE AND ALL RESULTING INTERMEDIATE FILES           }
{   ARE CONFIDENTIAL AND PROPRIETARY TRADE                          }
{   SECRETS OF DEVELOPER EXPRESS INC. THE REGISTERED DEVELOPER IS   }
{   LICENSED TO DISTRIBUTE THE PRODUCT AND ALL ACCOMPANYING .NET    }
{   CONTROLS AS PART OF AN EXECUTABLE PROGRAM ONLY.                 }
{                                                                   }
{   THE SOURCE CODE CONTAINED WITHIN THIS FILE AND ALL RELATED      }
{   FILES OR ANY PORTION OF ITS CONTENTS SHALL AT NO TIME BE        }
{   COPIED, TRANSFERRED, SOLD, DISTRIBUTED, OR OTHERWISE MADE       }
{   AVAILABLE TO OTHER INDIVIDUALS WITHOUT EXPRESS WRITTEN CONSENT  }
{   AND PERMISSION FROM DEVELOPER EXPRESS INC.                      }
{                                                                   }
{   CONSULT THE END USER LICENSE AGREEMENT FOR INFORMATION ON       }
{   ADDITIONAL RESTRICTIONS.                                        }
{                                                                   }
{*******************************************************************}
*/
#endregion Copyright (c) 2000-2015 Developer Express Inc.

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.XtraEditors;
namespace DevExpress.DXperience.Demos {
	public class RatingForm : XtraForm {
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label lbPoor;
		private System.Windows.Forms.Label lbExcellent;
		private System.Windows.Forms.Label lbOpinion;
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.Panel pnlOpinion;
		private TrackBarControl trbOpinion;
		private DevExpress.XtraEditors.MemoEdit meOpinion;
		private DevExpress.XtraEditors.SimpleButton btnSend;
		const string notSeen = "You have'nt seen any demo to have your opinion";
		Form form;
		public RatingForm(Form form) {
			this.form = form;
			InitializeComponent();
			this.Icon = form.Icon;
		}
		static string Email { get { return "DemetriusB@devexpress.com"; } }	
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}
		#region Windows Form Designer generated code
		private void InitializeComponent()
		{
			this.label1 = new System.Windows.Forms.Label();
			this.lbPoor = new System.Windows.Forms.Label();
			this.lbExcellent = new System.Windows.Forms.Label();
			this.lbOpinion = new System.Windows.Forms.Label();
			this.pnlOpinion = new System.Windows.Forms.Panel();
			this.trbOpinion = new TrackBarControl();
			this.meOpinion = new DevExpress.XtraEditors.MemoEdit();
			this.btnSend = new DevExpress.XtraEditors.SimpleButton();
			this.pnlOpinion.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.meOpinion.Properties)).BeginInit();
			this.SuspendLayout();
			this.label1.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(1)));
			this.label1.Location = new System.Drawing.Point(32, 13);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(268, 19);
			this.label1.TabIndex = 0;
			this.label1.Text = "How would you rate the quality of this demo?";
			this.lbPoor.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(1)));
			this.lbPoor.Location = new System.Drawing.Point(9, 60);
			this.lbPoor.Name = "lbPoor";
			this.lbPoor.Size = new System.Drawing.Size(31, 27);
			this.lbPoor.TabIndex = 2;
			this.lbPoor.Text = "Poor";
			this.lbExcellent.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(1)));
			this.lbExcellent.Location = new System.Drawing.Point(267, 60);
			this.lbExcellent.Name = "lbExcellent";
			this.lbExcellent.Size = new System.Drawing.Size(50, 27);
			this.lbExcellent.TabIndex = 3;
			this.lbExcellent.Text = "Excellent";
			this.lbOpinion.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(1)));
			this.lbOpinion.Location = new System.Drawing.Point(12, 115);
			this.lbOpinion.Name = "lbOpinion";
			this.lbOpinion.Size = new System.Drawing.Size(300, 18);
			this.lbOpinion.TabIndex = 4;
			this.lbOpinion.Text = "Tell us your opinion about the demos:";
			this.pnlOpinion.Controls.AddRange(new System.Windows.Forms.Control[] {
																					 this.trbOpinion});
			this.pnlOpinion.Location = new System.Drawing.Point(40, 46);
			this.pnlOpinion.Name = "pnlOpinion";
			this.pnlOpinion.Size = new System.Drawing.Size(224, 51);
			this.pnlOpinion.TabIndex = 8;
			this.trbOpinion.Dock = System.Windows.Forms.DockStyle.Fill;
			this.trbOpinion.Name = "trbOpinion";
			this.trbOpinion.Size = new System.Drawing.Size(224, 51);
			this.trbOpinion.TabIndex = 0;
			this.trbOpinion.Value = 5;
			this.meOpinion.EditValue = "";
			this.meOpinion.Location = new System.Drawing.Point(12, 134);
			this.meOpinion.Name = "meOpinion";
			this.meOpinion.Size = new System.Drawing.Size(300, 130);
			this.meOpinion.TabIndex = 9;
			this.btnSend.Location = new System.Drawing.Point(236, 268);
			this.btnSend.Name = "btnSend";
			this.btnSend.Size = new System.Drawing.Size(75, 28);
			this.btnSend.TabIndex = 10;
			this.btnSend.Text = "Email...";
			this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.ClientSize = new System.Drawing.Size(322, 303);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.btnSend,
																		  this.meOpinion,
																		  this.pnlOpinion,
																		  this.lbOpinion,
																		  this.lbExcellent,
																		  this.lbPoor,
																		  this.label1});
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "RatingForm";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Demo rating";
			this.Load += new System.EventHandler(this.RatingForm_Load);
			this.pnlOpinion.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.meOpinion.Properties)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		private void btnSend_Click(object sender, System.EventArgs e) {
			Cursor.Current = Cursors.WaitCursor;
			try {
				string ABody = "Rate = " + trbOpinion.Value.ToString();
				ABody += "\r\n\r\nDescription:\r\n" + meOpinion.Text;
				ABody = ABody.Replace("%", "$prc$");
				ABody = ABody.Replace("$prc$", "%25");
				ABody = ABody.Replace("\r\n", "%0D%0A");
				ABody = ABody.Replace("&", "%26");
				ABody = ABody.Replace(" ", "%20");
				string s = "mailto:" + Email + "?subject=" + EmailSubj + "&body=" + ABody;
				System.Diagnostics.Process.Start(s);
			}
			finally {
				Cursor.Current = Cursors.Default;
				Close();
			}
		}
		string EmailSubj { get { return System.Reflection.Assembly.GetEntryAssembly().GetName().Name + " - user rating"; } }
		void RatingForm_Load(object sender, System.EventArgs e) {
			bool seen = false;
			if (meOpinion.Text == notSeen) meOpinion.Text = "";
			int count = ModulesInfo.Count;
			for (int i = 0; i < count; i++) {
				ModuleInfo moduleInfo = ModulesInfo.GetItem(i);
				if (moduleInfo != null && moduleInfo.WasShown) {
					seen = true;
					if (meOpinion.Text.IndexOf(moduleInfo.Name) == -1)
						meOpinion.Text += moduleInfo.Name + " : \r\n";
				}
			}
			if (!seen) meOpinion.Text = notSeen;
			meOpinion.Enabled = seen;
			btnSend.Enabled = seen;
			trbOpinion.Enabled = seen;
			meOpinion.DeselectAll();
		}
	}
}
