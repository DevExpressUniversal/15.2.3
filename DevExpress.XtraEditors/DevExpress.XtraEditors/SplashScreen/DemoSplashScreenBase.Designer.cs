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

using DevExpress.XtraEditors;
namespace DevExpress.XtraSplashScreen {
	partial class DemoSplashScreenBase {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(components != null)
					components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DemoSplashScreenBase));
			this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
			this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
			this.panelControl = new DevExpress.XtraEditors.PanelControl();
			this.labelProductText = new DevExpress.XtraEditors.LabelControl();
			this.labelDemoText = new DevExpress.XtraEditors.LabelControl();
			this.pictureEdit2 = new DevExpress.XtraEditors.PictureEdit();
			this.pictureEdit1 = new DevExpress.XtraEditors.PictureEdit();
			this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
			((System.ComponentModel.ISupportInitialize)(this.panelControl)).BeginInit();
			this.panelControl.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureEdit2.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureEdit1.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
			this.panelControl1.SuspendLayout();
			this.SuspendLayout();
			this.labelControl1.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
			this.labelControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.labelControl1.Location = new System.Drawing.Point(21, 4);
			this.labelControl1.Name = "labelControl1";
			this.labelControl1.Size = new System.Drawing.Size(320, 48);
			this.labelControl1.TabIndex = 6;
			this.labelControl1.Text = "Copyright © 1998-2013 Developer Express inc.\r\nAll Rights Reserved";
			this.labelControl2.Location = new System.Drawing.Point(71, 225);
			this.labelControl2.Name = "labelControl2";
			this.labelControl2.Size = new System.Drawing.Size(50, 13);
			this.labelControl2.TabIndex = 7;
			this.labelControl2.Text = "Starting...";
			this.panelControl.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(247)))), ((int)(((byte)(148)))), ((int)(((byte)(30)))));
			this.panelControl.Appearance.Options.UseBackColor = true;
			this.panelControl.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.panelControl.Controls.Add(this.labelProductText);
			this.panelControl.Controls.Add(this.labelDemoText);
			this.panelControl.Location = new System.Drawing.Point(12, 12);
			this.panelControl.Name = "panelControl";
			this.panelControl.Size = new System.Drawing.Size(474, 180);
			this.panelControl.TabIndex = 9;
			this.labelProductText.Appearance.Font = new System.Drawing.Font("Segoe UI Light", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.labelProductText.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(252)))), ((int)(((byte)(216)))), ((int)(((byte)(188)))));
			this.labelProductText.Location = new System.Drawing.Point(169, 119);
			this.labelProductText.Name = "labelProductText";
			this.labelProductText.Size = new System.Drawing.Size(91, 25);
			this.labelProductText.TabIndex = 9;
			this.labelProductText.Text = "DevExpress";
			this.labelProductText.TextChanged += new System.EventHandler(this.labelProductText_TextChanged);
			this.labelProductText.ParentChanged += new System.EventHandler(this.labelProductText_ParentChanged);
			this.labelDemoText.Appearance.Font = new System.Drawing.Font("Segoe UI Light", 26.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.labelDemoText.Appearance.ForeColor = System.Drawing.Color.White;
			this.labelDemoText.Location = new System.Drawing.Point(123, 66);
			this.labelDemoText.Name = "labelDemoText";
			this.labelDemoText.Size = new System.Drawing.Size(263, 47);
			this.labelDemoText.TabIndex = 8;
			this.labelDemoText.Text = "WinForms Demos";
			this.labelDemoText.TextChanged += new System.EventHandler(this.labelDemoText_TextChanged);
			this.labelDemoText.ParentChanged += new System.EventHandler(this.labelDemoText_ParentChanged);
			this.pictureEdit2.Location = new System.Drawing.Point(12, 12);
			this.pictureEdit2.Name = "pictureEdit2";
			this.pictureEdit2.Properties.AllowFocused = false;
			this.pictureEdit2.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
			this.pictureEdit2.Properties.Appearance.Options.UseBackColor = true;
			this.pictureEdit2.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pictureEdit2.Properties.ShowMenu = false;
			this.pictureEdit2.Size = new System.Drawing.Size(474, 180);
			this.pictureEdit2.TabIndex = 11;
			this.pictureEdit2.Visible = false;
			this.pictureEdit2.ImageChanged += new System.EventHandler(this.pictureEdit2_ImageChanged);
			this.pictureEdit1.EditValue = ((object)(resources.GetObject("pictureEdit1.EditValue")));
			this.pictureEdit1.Location = new System.Drawing.Point(360, 4);
			this.pictureEdit1.Name = "pictureEdit1";
			this.pictureEdit1.Properties.AllowFocused = false;
			this.pictureEdit1.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
			this.pictureEdit1.Properties.Appearance.Options.UseBackColor = true;
			this.pictureEdit1.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pictureEdit1.Properties.ShowMenu = false;
			this.pictureEdit1.Size = new System.Drawing.Size(119, 48);
			this.pictureEdit1.TabIndex = 8;
			this.panelControl1.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(238)))), ((int)(((byte)(238)))));
			this.panelControl1.Appearance.Options.UseBackColor = true;
			this.panelControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.panelControl1.Controls.Add(this.labelControl1);
			this.panelControl1.Controls.Add(this.pictureEdit1);
			this.panelControl1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panelControl1.Location = new System.Drawing.Point(1, 311);
			this.panelControl1.Name = "panelControl1";
			this.panelControl1.Size = new System.Drawing.Size(496, 56);
			this.panelControl1.TabIndex = 10;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.White;
			this.ClientSize = new System.Drawing.Size(498, 368);
			this.Controls.Add(this.pictureEdit2);
			this.Controls.Add(this.panelControl1);
			this.Controls.Add(this.panelControl);
			this.Controls.Add(this.labelControl2);
			this.Name = "DemoSplashScreenBase";
			this.Padding = new System.Windows.Forms.Padding(1);
			this.Text = "Form1";
			((System.ComponentModel.ISupportInitialize)(this.panelControl)).EndInit();
			this.panelControl.ResumeLayout(false);
			this.panelControl.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureEdit2.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureEdit1.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
			this.panelControl1.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		protected DevExpress.XtraEditors.LabelControl labelControl1;
		protected DevExpress.XtraEditors.LabelControl labelControl2;
		protected DevExpress.XtraEditors.PictureEdit pictureEdit1;
		protected PanelControl panelControl;
		protected PanelControl panelControl1;
		protected LabelControl labelDemoText;
		protected PictureEdit pictureEdit2;
		protected LabelControl labelProductText;
	}
}
