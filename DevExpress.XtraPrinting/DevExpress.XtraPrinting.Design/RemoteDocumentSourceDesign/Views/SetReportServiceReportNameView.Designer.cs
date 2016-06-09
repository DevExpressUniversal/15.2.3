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

namespace DevExpress.XtraPrinting.Design.RemoteDocumentSourceDesign.Views {
	partial class SetReportServiceReportNameView {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
			this.labelControl4 = new DevExpress.XtraEditors.LabelControl();
			this.uriEdit = new DevExpress.XtraEditors.ComboBoxEdit();
			this.reportNameEdit = new DevExpress.XtraEditors.ComboBoxEdit();
			this.tableLayoutPanel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.uriEdit.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.reportNameEdit.Properties)).BeginInit();
			this.SuspendLayout();
			this.tableLayoutPanel1.ColumnCount = 4;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 60F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
			this.tableLayoutPanel1.Controls.Add(this.labelControl1, 1, 1);
			this.tableLayoutPanel1.Controls.Add(this.labelControl4, 1, 2);
			this.tableLayoutPanel1.Controls.Add(this.uriEdit, 2, 1);
			this.tableLayoutPanel1.Controls.Add(this.reportNameEdit, 2, 2);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 4;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 35F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 65F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(571, 226);
			this.tableLayoutPanel1.TabIndex = 1;
			this.labelControl1.Appearance.Font = new System.Drawing.Font("Segoe UI Light", 11F);
			this.labelControl1.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
			this.labelControl1.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
			this.labelControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.labelControl1.Location = new System.Drawing.Point(85, 58);
			this.labelControl1.Margin = new System.Windows.Forms.Padding(0);
			this.labelControl1.Name = "labelControl1";
			this.labelControl1.Padding = new System.Windows.Forms.Padding(0, 0, 24, 0);
			this.labelControl1.Size = new System.Drawing.Size(144, 30);
			this.labelControl1.TabIndex = 0;
			this.labelControl1.Text = "Service Uri:";
			this.labelControl4.Appearance.Font = new System.Drawing.Font("Segoe UI Light", 11F);
			this.labelControl4.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
			this.labelControl4.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
			this.labelControl4.Dock = System.Windows.Forms.DockStyle.Fill;
			this.labelControl4.Location = new System.Drawing.Point(85, 88);
			this.labelControl4.Margin = new System.Windows.Forms.Padding(0);
			this.labelControl4.Name = "labelControl4";
			this.labelControl4.Padding = new System.Windows.Forms.Padding(0, 0, 24, 0);
			this.labelControl4.Size = new System.Drawing.Size(144, 30);
			this.labelControl4.TabIndex = 3;
			this.labelControl4.Text = "Report Name:";
			this.uriEdit.Dock = System.Windows.Forms.DockStyle.Fill;
			this.uriEdit.EditValue = "";
			this.uriEdit.Location = new System.Drawing.Point(229, 61);
			this.uriEdit.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
			this.uriEdit.Name = "uriEdit";
			this.uriEdit.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI Light", 10F);
			this.uriEdit.Properties.Appearance.Options.UseFont = true;
			this.uriEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			this.uriEdit.Size = new System.Drawing.Size(256, 24);
			this.uriEdit.TabIndex = 1;
			this.uriEdit.TextChanged += new System.EventHandler(this.OnReportStorageParametersChanged);
			this.reportNameEdit.Dock = System.Windows.Forms.DockStyle.Fill;
			this.reportNameEdit.EditValue = "";
			this.reportNameEdit.Location = new System.Drawing.Point(229, 91);
			this.reportNameEdit.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
			this.reportNameEdit.Name = "reportNameEdit";
			this.reportNameEdit.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI Light", 10F);
			this.reportNameEdit.Properties.Appearance.Options.UseFont = true;
			this.reportNameEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			this.reportNameEdit.Size = new System.Drawing.Size(256, 24);
			this.reportNameEdit.TabIndex = 2;
			this.reportNameEdit.TextChanged += new System.EventHandler(this.OnReportStorageParametersChanged);
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tableLayoutPanel1);
			this.Name = "SetReportServiceReportNameView";
			this.Size = new System.Drawing.Size(571, 226);
			this.tableLayoutPanel1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.uriEdit.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.reportNameEdit.Properties)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private XtraEditors.LabelControl labelControl1;
		private XtraEditors.LabelControl labelControl4;
		private XtraEditors.ComboBoxEdit uriEdit;
		private XtraEditors.ComboBoxEdit reportNameEdit;
	}
}
