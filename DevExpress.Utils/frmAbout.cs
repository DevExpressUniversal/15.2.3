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
using DevExpress.Data.Utils;
namespace DevExpress.Utils.About {
	public class frmAbout : System.Windows.Forms.Form {
		private System.ComponentModel.IContainer components;
		string text, copyright1, copyright2 = "ALL RIGHTS RESERVED";
		Font font1, font2;
		double dOpacity;
		Bitmap backgroundBitmap;
		private System.Windows.Forms.Timer timer1;
		private System.Windows.Forms.LinkLabel llbDevExpress;
		public static void Show(string title) {
			frmAbout box = new frmAbout(title);
			box.ShowDialog();
		}
		public frmAbout(string s) {
			InitializeComponent();
			text = s;
			this.dOpacity = 0.15;
			int year = DateTime.Now.Year;
			if(year < 2007) year = 2007;
			copyright1 = "Copyright  1999-" + year.ToString() + " Developer Express Inc.";
			backgroundBitmap = ImageTool.ImageFromStream(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("DevExpress.Utils.about.jpg")) as Bitmap;
			backgroundBitmap.MakeTransparent();
			ClientSize = backgroundBitmap.Size;
			llbDevExpress.Text = "www.devexpress.com";
			llbDevExpress.Width = Width;
			llbDevExpress.Location = new Point(0, 154);
			try {
				llbDevExpress.Font = new Font("Tahoma", 9);
				font1 = new Font("Tahoma", 9);
				font2 = new Font("Tahoma", 7);
			}
			catch { }
			if(IsRegionSupported)
				Region = DevExpress.Utils.Win.BitmapToRegion.Convert(backgroundBitmap, Color.Empty);
		}
		bool IsRegionSupported {
			get {
				return (System.Environment.OSVersion.Version.Major > 5 || (System.Environment.OSVersion.Version.Major == 5 && System.Environment.OSVersion.Version.Minor > 0));
			}
		}
		protected override CreateParams CreateParams {
			get {
				CreateParams cp = base.CreateParams;
				if(IsRegionSupported)
					cp.ClassStyle |= 0x20000; 
				return cp;
			}
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			this.llbDevExpress = new System.Windows.Forms.LinkLabel();
			this.timer1 = new System.Windows.Forms.Timer(this.components);
			this.SuspendLayout();
			this.llbDevExpress.BackColor = System.Drawing.Color.Transparent;
			this.llbDevExpress.LinkColor = System.Drawing.Color.FromArgb(171, 222, 255);
			this.llbDevExpress.Name = "llbDevExpress";
			this.llbDevExpress.Size = new System.Drawing.Size(342, 20);
			this.llbDevExpress.TabIndex = 1;
			this.llbDevExpress.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			this.llbDevExpress.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llbDevExpress_LinkClicked);
			this.llbDevExpress.MouseEnter += new System.EventHandler(this.llbDevExpress_MouseEnter);
			this.llbDevExpress.MouseLeave += new System.EventHandler(this.llbDevExpress_MouseLeave);
			this.timer1.Enabled = true;
			this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(344, 173);
			this.ControlBox = false;
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.llbDevExpress});
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.KeyPreview = true;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "frmAbout";
			this.Opacity = 0;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.TopMost = true;
			this.BackColor = Color.Magenta;
			this.TransparencyKey = Color.Magenta;
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmAbout_KeyDown);
			this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.frmAbout_MouseDown);
			this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.frmAbout_MouseUp);
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.frmAbout_Paint);
			this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.frmAbout_MouseMove);
			this.ResumeLayout(false);
		}
		#endregion
		private void frmAbout_Paint(object sender, System.Windows.Forms.PaintEventArgs e) {
			e.Graphics.DrawImage(backgroundBitmap, 0, 0);
			if(font1 == null) return;
			using(StringFormat stf = new StringFormat()) {
				stf.Alignment = StringAlignment.Center;
				stf.Trimming = StringTrimming.EllipsisCharacter;
				SolidBrush brush1 = new SolidBrush(Color.FromArgb(190, 190, 190));
				SolidBrush brush2 = new SolidBrush(Color.FromArgb(255, 255, 255));
				e.Graphics.DrawString(copyright1, font2, brush1, new Rectangle(0, 175, Width, 15), stf);
				e.Graphics.DrawString(copyright2, font2, brush1, new Rectangle(0, 185, Width, 15), stf);
				e.Graphics.DrawString(text, font1, brush2, new Rectangle(10, 115, Width - 20, 30), stf);
			}
		}
		protected void llbDevExpress_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e) {
			llbDevExpress.LinkVisited = true;
			System.Diagnostics.Process process = new System.Diagnostics.Process();
			process.StartInfo.FileName = AssemblyInfo.DXLinkGetStarted;
			process.StartInfo.Verb = "Open";
			process.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
			process.Start();
		}
		protected void llbDevExpress_MouseLeave(object sender, System.EventArgs e) {
			llbDevExpress.LinkColor = Color.FromArgb(171, 222, 255);
		}
		protected void llbDevExpress_MouseEnter(object sender, System.EventArgs e) {
			llbDevExpress.LinkColor = Color.FromArgb(49, 118, 214);
		}
		private void timer1_Tick(object sender, System.EventArgs e) {
			if(Opacity < 1)
				Opacity += dOpacity;
			else
				timer1.Enabled = false;
		}
		private void frmAbout_KeyDown(object sender, KeyEventArgs e) {
			if(e.KeyData == Keys.Escape) Close();
		}
		bool blnMoving = false;
		int MouseDownX, MouseDownY;
		Rectangle moveFormRectangle = new Rectangle(120, 45, 35, 35);
		private void frmAbout_MouseDown(object sender, MouseEventArgs e) {
			if(e.Button != MouseButtons.Left) return;
			if(!moveFormRectangle.Contains(e.X, e.Y)) Close();
			blnMoving = true;
			MouseDownX = e.X;
			MouseDownY = e.Y;
		}
		private void frmAbout_MouseMove(object sender, MouseEventArgs e) {
			if(blnMoving)
				this.Location = new Point(
					this.Location.X + (e.X - MouseDownX),
					this.Location.Y + (e.Y - MouseDownY));
		}
		private void frmAbout_MouseUp(object sender, MouseEventArgs e) {
			if(e.Button == MouseButtons.Left)
				blnMoving = false;
		}
	}
}
