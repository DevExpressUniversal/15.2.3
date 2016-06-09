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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraEditors.Controls;
namespace DevExpress.XtraEditors.Frames {
	public abstract class SimpleLayoutFrame : DevExpress.XtraEditors.Designer.Utils.XtraFrame {
		TableLayoutPanel tableLayoutPanel1;
		SimpleButton btnLoadLayout;
		Panel panel1;
		SimpleButton btnSaveLayout;
		LabelControl lbDescription;
		IContainer components = null;
		public SimpleLayoutFrame() {
			InitializeComponent();
			lbCaption.Visible = false;
			lbCaption.VisibleChanged += OnlbCaptionVisibleChanged;
			lbDescription.Text = GetDescription();
		}
		void OnlbCaptionVisibleChanged(object sender, EventArgs e) {
			lbCaption.Visible = false;
		}
		protected virtual string GetDescription() { return string.Empty; }
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		protected virtual void RestoreLayoutFromXml(string path) { }
		protected virtual void SaveLayoutToXml(string path) { }
		const int DefaultInterval = 16;
		void OnPanelResize(object sender, EventArgs e) {
			int allSize = btnSaveLayout.Width + btnLoadLayout.Width + DefaultInterval;
			int x = (panel1.Width - allSize) / 2;
			btnLoadLayout.Location = new Point(x, 0);
			btnSaveLayout.Location = new Point(btnLoadLayout.Right + DefaultInterval, 0);
		}
		void btnRestoreLaoyout_Click(object sender, EventArgs e) {
			RestoreLayout();
		}
		void btnSaveLayout_Click(object sender, EventArgs e) {
			SaveLayout();
		}
		protected virtual void RestoreLayout() {
			OpenFileDialog dlg = new OpenFileDialog();
			dlg.Filter = Localizer.Active.GetLocalizedString(StringId.RestoreLayoutDialogFileFilter);
			dlg.Title = Localizer.Active.GetLocalizedString(StringId.RestoreLayoutDialogTitle);
			if(dlg.ShowDialog() == DialogResult.OK) {
				try {
					RestoreLayoutFromXml(dlg.FileName);
				}
				catch { }
			}
		}
		protected virtual void SaveLayout() {
			SaveFileDialog dlg = new SaveFileDialog();
			dlg.Filter = Localizer.Active.GetLocalizedString(StringId.SaveLayoutDialogFileFilter);
			dlg.Title = Localizer.Active.GetLocalizedString(StringId.SaveLayoutDialogTitle);
			if(dlg.ShowDialog() == DialogResult.OK) {
				SaveLayoutToXml(dlg.FileName);
			}
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.panel1 = new System.Windows.Forms.Panel();
			this.btnLoadLayout = new DevExpress.XtraEditors.SimpleButton();
			this.btnSaveLayout = new DevExpress.XtraEditors.SimpleButton();
			this.lbDescription = new DevExpress.XtraEditors.LabelControl();
			((System.ComponentModel.ISupportInitialize)(this.pnlMain)).BeginInit();
			this.pnlMain.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.horzSplitter)).BeginInit();
			this.tableLayoutPanel1.SuspendLayout();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			this.lbCaption.Appearance.Font = new System.Drawing.Font("Segoe UI Light", 34F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
			this.lbCaption.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Bottom;
			this.lbCaption.Size = new System.Drawing.Size(1239, 44);
			this.pnlMain.Controls.Add(this.tableLayoutPanel1);
			this.pnlMain.Location = new System.Drawing.Point(0, 48);
			this.pnlMain.Size = new System.Drawing.Size(1239, 504);
			this.horzSplitter.Location = new System.Drawing.Point(0, 44);
			this.horzSplitter.Size = new System.Drawing.Size(1239, 4);
			this.tableLayoutPanel1.ColumnCount = 1;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this.lbDescription, 0, 0);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 3;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 140F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.00001F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(1239, 504);
			this.tableLayoutPanel1.TabIndex = 0;
			this.panel1.AutoSize = true;
			this.panel1.Controls.Add(this.btnLoadLayout);
			this.panel1.Controls.Add(this.btnSaveLayout);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new System.Drawing.Point(0, 181);
			this.panel1.Margin = new System.Windows.Forms.Padding(0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(1239, 140);
			this.panel1.TabIndex = 1;
			this.panel1.Resize += new System.EventHandler(this.OnPanelResize);
			this.btnLoadLayout.Appearance.Font = new System.Drawing.Font("Segoe UI Light", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
			this.btnLoadLayout.Appearance.Options.UseFont = true;
			this.btnLoadLayout.Location = new System.Drawing.Point(331, 0);
			this.btnLoadLayout.Name = "btnLoadLayout";
			this.btnLoadLayout.Size = new System.Drawing.Size(246, 138);
			this.btnLoadLayout.TabIndex = 0;
			this.btnLoadLayout.Text = "&Load Layout...";
			this.btnLoadLayout.Click += new System.EventHandler(this.btnRestoreLaoyout_Click);
			this.btnSaveLayout.Appearance.Font = new System.Drawing.Font("Segoe UI Light", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
			this.btnSaveLayout.Appearance.Options.UseFont = true;
			this.btnSaveLayout.Location = new System.Drawing.Point(616, 0);
			this.btnSaveLayout.Name = "btnSaveLayout";
			this.btnSaveLayout.Size = new System.Drawing.Size(246, 138);
			this.btnSaveLayout.TabIndex = 1;
			this.btnSaveLayout.Text = "Sa&ve Layout...";
			this.btnSaveLayout.Click += new System.EventHandler(this.btnSaveLayout_Click);
			this.lbDescription.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
			this.lbDescription.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
			this.lbDescription.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Bottom;
			this.lbDescription.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
			this.lbDescription.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
			this.lbDescription.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lbDescription.Location = new System.Drawing.Point(3, 3);
			this.lbDescription.Name = "lbDescription";
			this.lbDescription.Padding = new System.Windows.Forms.Padding(33);
			this.lbDescription.Size = new System.Drawing.Size(1233, 175);
			this.lbDescription.TabIndex = 2;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Name = "SimpleLayoutFrame";
			this.Size = new System.Drawing.Size(1239, 552);
			((System.ComponentModel.ISupportInitialize)(this.pnlMain)).EndInit();
			this.pnlMain.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.horzSplitter)).EndInit();
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		#endregion
	}
}
