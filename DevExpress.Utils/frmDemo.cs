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

namespace DevExpress.Utils
{
	using System;
	using System.Drawing;
	using System.Collections;
	using System.Windows.Forms;
	using System.Diagnostics;
	using System.Reflection;
	public class DemoBox : System.Windows.Forms.Form {
		private System.Windows.Forms.Label lbCopyRight;
		private System.Windows.Forms.LinkLabel llbDevExpress;
		private System.Windows.Forms.Label lbDescription;
		private System.Windows.Forms.GroupBox gbSeparator;
		private System.Windows.Forms.Button btnOK;
		private Image aIcon;
		public static void Show(string title, Type t) {
			return;
		}
		public DemoBox(System.Drawing.Rectangle rec,System.Drawing.Image ico, string s, Type t) {
			InitializeComponent();
			try {
				this.llbDevExpress.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold);
			} catch {}
			aIcon = ico;
			if(rec.IsEmpty)
				this.StartPosition = FormStartPosition.CenterScreen;
			else
				this.Location = new Point(rec.X + (rec.Width - this.Width) / 2, rec.Y + (rec.Height - this.Height) / 2);
			string versionText = "";
			string ver = t.Assembly.ToString();
			if(ver.IndexOf("Version=") != -1) {
				ver = ver.Substring(ver.IndexOf("Version=") + 8);
				if(ver.IndexOf(", ") != -1) ver = ver.Substring(0, ver.IndexOf(", "));
				versionText = ver;
			}
			lbDescription.Text = String.Format(s + "\n(Demo version for evaluation purposes only)", versionText);
			btnOK.Focus();
		}
		protected override void OnClosed(EventArgs e) {
			base.OnClosed(e);
		}
		private void InitializeComponent()
		{
			this.lbDescription = new System.Windows.Forms.Label();
			this.btnOK = new System.Windows.Forms.Button();
			this.llbDevExpress = new System.Windows.Forms.LinkLabel();
			this.lbCopyRight = new System.Windows.Forms.Label();
			this.gbSeparator = new System.Windows.Forms.GroupBox();
			this.SuspendLayout();
			this.lbDescription.Location = new System.Drawing.Point(0, 8);
			this.lbDescription.Name = "lbDescription";
			this.lbDescription.Size = new System.Drawing.Size(248, 32);
			this.lbDescription.TabIndex = 2;
			this.lbDescription.Text = "Description";
			this.lbDescription.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOK.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.btnOK.Location = new System.Drawing.Point(80, 112);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(96, 24);
			this.btnOK.TabIndex = 4;
			this.btnOK.Text = "&OK";
			this.llbDevExpress.Location = new System.Drawing.Point(8, 48);
			this.llbDevExpress.Name = "llbDevExpress";
			this.llbDevExpress.Size = new System.Drawing.Size(240, 22);
			this.llbDevExpress.TabIndex = 1;
			this.llbDevExpress.TabStop = true;
			this.llbDevExpress.Text = "www.devexpress.com";
			this.llbDevExpress.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.llbDevExpress.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llbDevExpress_LinkClicked);
			this.llbDevExpress.MouseEnter += new System.EventHandler(this.llbDevExpress_MouseEnter);
			this.llbDevExpress.MouseLeave += new System.EventHandler(this.llbDevExpress_MouseLeave);
			this.lbCopyRight.Location = new System.Drawing.Point(8, 72);
			this.lbCopyRight.Name = "lbCopyRight";
			this.lbCopyRight.Size = new System.Drawing.Size(238, 24);
			this.lbCopyRight.TabIndex = 0;
			this.lbCopyRight.Text = "Copyright  2000-2003 Developer Express Inc.  ALL RIGHTS RESERVED";
			this.lbCopyRight.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.gbSeparator.Location = new System.Drawing.Point(5, 104);
			this.gbSeparator.Name = "gbSeparator";
			this.gbSeparator.Size = new System.Drawing.Size(243, 3);
			this.gbSeparator.TabIndex = 3;
			this.gbSeparator.TabStop = false;
			this.AcceptButton = this.btnOK;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.btnOK;
			this.ClientSize = new System.Drawing.Size(254, 142);
			this.ControlBox = false;
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.lbCopyRight,
																		  this.llbDevExpress,
																		  this.lbDescription,
																		  this.gbSeparator,
																		  this.btnOK});
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "AboutBox";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "About";
			this.TopMost = true;
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.OnPaint);
			this.ResumeLayout(false);
		}
		protected void llbDevExpress_LinkClicked (object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e) {
			llbDevExpress.LinkVisited = true;
			Process process = new Process();
			process.StartInfo.FileName = "http://www.devexpress.com";
			process.StartInfo.Verb = "Open";
			process.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
			process.Start();
		}
		protected void llbDevExpress_MouseLeave (object sender, System.EventArgs e)	{
			llbDevExpress.LinkColor = (System.Drawing.Color) System.Drawing.Color.FromArgb (0, 0, 255);
		}
		protected void llbDevExpress_MouseEnter (object sender, System.EventArgs e)	{
			llbDevExpress.LinkColor = (System.Drawing.Color) System.Drawing.Color.FromArgb (100, 100, 255);
		}
		protected void OnPaint(object sender, PaintEventArgs e) {
			if(aIcon != null)
				e.Graphics.DrawImageUnscaled(aIcon, 10, 10);
		}
	}
}
