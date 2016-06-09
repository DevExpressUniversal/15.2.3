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
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using DevExpress.XtraEditors;
namespace DevExpress.XtraBars.Design.Frames {
	[ToolboxItem(false)]
	public class PaintStyle : System.Windows.Forms.UserControl {
		private DevExpress.XtraEditors.PictureEdit pictureEdit1;
		private DevExpress.XtraEditors.PictureEdit pictureEdit2;
		private DevExpress.XtraEditors.PictureEdit pictureEdit3;
		private DevExpress.XtraEditors.PictureEdit pictureEdit4;
		private DevExpress.XtraEditors.RadioGroup radioGroup1;
		private System.Windows.Forms.Panel pnlMain;
		private System.ComponentModel.Container components = null;
		public PaintStyle() {
			InitializeComponent();
			SetStyle(ControlStyles.DoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
		}
		protected override void Dispose( bool disposing ) {
			if( disposing ) {
				if(components != null) {
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			this.pictureEdit1 = new DevExpress.XtraEditors.PictureEdit();
			this.pictureEdit2 = new DevExpress.XtraEditors.PictureEdit();
			this.pictureEdit3 = new DevExpress.XtraEditors.PictureEdit();
			this.pictureEdit4 = new DevExpress.XtraEditors.PictureEdit();
			this.radioGroup1 = new DevExpress.XtraEditors.RadioGroup();
			this.pnlMain = new System.Windows.Forms.Panel();
			((System.ComponentModel.ISupportInitialize)(this.pictureEdit1.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureEdit2.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureEdit3.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureEdit4.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.radioGroup1.Properties)).BeginInit();
			this.pnlMain.SuspendLayout();
			this.SuspendLayout();
			this.pictureEdit1.Location = new System.Drawing.Point(136, 8);
			this.pictureEdit1.Name = "pictureEdit1";
			this.pictureEdit1.Properties.ReadOnly = true;
			this.pictureEdit1.Properties.ShowMenu = false;
			this.pictureEdit1.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Clip;
			this.pictureEdit1.Size = new System.Drawing.Size(304, 74);
			this.pictureEdit1.TabIndex = 5;
			this.pictureEdit1.Tag = "WindowsXP";
			this.pictureEdit1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureEdit_MouseDown);
			this.pictureEdit2.Location = new System.Drawing.Point(136, 92);
			this.pictureEdit2.Name = "pictureEdit2";
			this.pictureEdit2.Properties.ReadOnly = true;
			this.pictureEdit2.Properties.ShowMenu = false;
			this.pictureEdit2.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Clip;
			this.pictureEdit2.Size = new System.Drawing.Size(304, 74);
			this.pictureEdit2.TabIndex = 6;
			this.pictureEdit2.Tag = "OfficeXP";
			this.pictureEdit2.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureEdit_MouseDown);
			this.pictureEdit3.Location = new System.Drawing.Point(136, 176);
			this.pictureEdit3.Name = "pictureEdit3";
			this.pictureEdit3.Properties.ReadOnly = true;
			this.pictureEdit3.Properties.ShowMenu = false;
			this.pictureEdit3.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Clip;
			this.pictureEdit3.Size = new System.Drawing.Size(304, 74);
			this.pictureEdit3.TabIndex = 7;
			this.pictureEdit3.Tag = "Office2000";
			this.pictureEdit3.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureEdit_MouseDown);
			this.pictureEdit4.Location = new System.Drawing.Point(136, 260);
			this.pictureEdit4.Name = "pictureEdit4";
			this.pictureEdit4.Properties.ReadOnly = true;
			this.pictureEdit4.Properties.ShowMenu = false;
			this.pictureEdit4.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Clip;
			this.pictureEdit4.Size = new System.Drawing.Size(304, 74);
			this.pictureEdit4.TabIndex = 8;
			this.pictureEdit4.Tag = "Office2003";
			this.pictureEdit4.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureEdit_MouseDown);
			this.radioGroup1.Location = new System.Drawing.Point(8, 8);
			this.radioGroup1.Name = "radioGroup1";
			this.radioGroup1.Properties.Items.AddRange(new DevExpress.XtraEditors.Controls.RadioGroupItem[] {
																												new DevExpress.XtraEditors.Controls.RadioGroupItem("WindowsXP", "Windows XP"),
																												new DevExpress.XtraEditors.Controls.RadioGroupItem("OfficeXP", "Office XP"),
																												new DevExpress.XtraEditors.Controls.RadioGroupItem("Office2000", "Office 2000"),
																												new DevExpress.XtraEditors.Controls.RadioGroupItem("Office2003", "Office 2003"),
																												new DevExpress.XtraEditors.Controls.RadioGroupItem("Skin", "Skin"),
																												new DevExpress.XtraEditors.Controls.RadioGroupItem("Default", "Default")});
			this.radioGroup1.Size = new System.Drawing.Size(120, 326);
			this.radioGroup1.TabIndex = 10;
			this.radioGroup1.SelectedIndexChanged += new System.EventHandler(this.radioGroup1_SelectedIndexChanged);
			this.pnlMain.Controls.AddRange(new System.Windows.Forms.Control[] {
																				  this.pictureEdit4,
																				  this.pictureEdit2,
																				  this.pictureEdit3,
																				  this.radioGroup1,
																				  this.pictureEdit1});
			this.pnlMain.Name = "pnlMain";
			this.pnlMain.Size = new System.Drawing.Size(448, 340);
			this.pnlMain.TabIndex = 11;
			this.pnlMain.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pnlMain_MouseDown);
			this.AutoScroll = true;
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.pnlMain});
			this.Name = "PaintStyle";
			this.Size = new System.Drawing.Size(664, 428);
			this.Resize += new System.EventHandler(this.PaintStyle_Resize);
			((System.ComponentModel.ISupportInitialize)(this.pictureEdit1.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureEdit2.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureEdit3.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureEdit4.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.radioGroup1.Properties)).EndInit();
			this.pnlMain.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		#endregion
		public string PaintStyleName { get { return radioGroup1.EditValue.ToString(); }}
		string defaultStyle = "...";
		public void InitStyle(string paintStyleName) {
			if(DevExpress.Utils.WXPaint.Painter.ThemesEnabled) defaultStyle = "Skin";
			radioGroup1.EditValue = paintStyleName;
			radioGroup1_SelectedIndexChanged(radioGroup1, EventArgs.Empty);
		}
		private void pictureEdit_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e) {
			PictureEdit pe = sender as PictureEdit;
			string tag = pe.Tag.ToString();
		}
		private void radioGroup1_SelectedIndexChanged(object sender, System.EventArgs e) {
			string tag = radioGroup1.EditValue.ToString();
			foreach(Control c in this.pnlMain.Controls)
				if(c is PictureEdit) {
					c.Enabled = (tag == c.Tag.ToString()) || (tag == "Default" && defaultStyle == c.Tag.ToString());
				}			
		}
		public void SetImages(string s) {
			Image image = Image.FromStream(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(s));
			SetImage(pictureEdit1, image, 0);
			SetImage(pictureEdit2, image, 300);
			SetImage(pictureEdit3, image, 600);
			SetImage(pictureEdit4, image, 900);
		}
		private void SetImage(PictureEdit pe, Image image, int dx) {
			Bitmap bitmap = new Bitmap(300, 70);
			Graphics g = Graphics.FromImage(bitmap);
			g.DrawImage(image, 0, 0, new RectangleF(dx, 0, 300, 70), GraphicsUnit.Pixel);
			pe.Image = bitmap;
		}
		private void PaintStyle_Resize(object sender, System.EventArgs e) {
			pnlMain.Location = new Point(Math.Max((Width - pnlMain.Width) / 2, 0), 
				Math.Max((Height - pnlMain.Height) / 2, 0));
		}
		private void pnlMain_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e) {
			foreach(Control c in this.pnlMain.Controls)
				if(c is PictureEdit && c.Bounds.Contains(new Point(e.X, e.Y))) {
					radioGroup1.EditValue = c.Tag.ToString();
					break;
				}
		}
	}
}
