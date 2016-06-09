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

namespace DevExpress.XtraEditors.Filtering.Templates.DateTimeRange {
	partial class DefaultTemplate {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
			this.Part_FromLabel = new DevExpress.XtraEditors.LabelControl();
			this.Part_ToLabel = new DevExpress.XtraEditors.LabelControl();
			this.Part_FromValue = new DevExpress.XtraEditors.DateEdit();
			this.Part_ToValue = new DevExpress.XtraEditors.DateEdit();
			this.tableLayoutPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.Part_FromValue.Properties.CalendarTimeProperties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.Part_FromValue.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.Part_ToValue.Properties.CalendarTimeProperties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.Part_ToValue.Properties)).BeginInit();
			this.SuspendLayout();
			this.tableLayoutPanel.AutoSize = true;
			this.tableLayoutPanel.ColumnCount = 4;
			this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel.Controls.Add(this.Part_FromLabel, 0, 0);
			this.tableLayoutPanel.Controls.Add(this.Part_ToLabel, 2, 0);
			this.tableLayoutPanel.Controls.Add(this.Part_FromValue, 1, 0);
			this.tableLayoutPanel.Controls.Add(this.Part_ToValue, 4, 0);
			this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel.Location = new System.Drawing.Point(9, 1);
			this.tableLayoutPanel.Name = "tableLayoutPanel";
			this.tableLayoutPanel.RowCount = 1;
			this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
			this.tableLayoutPanel.Size = new System.Drawing.Size(222, 26);
			this.tableLayoutPanel.TabIndex = 2;
			this.Part_FromLabel.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Horizontal;
			this.Part_FromLabel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.Part_FromLabel.Location = new System.Drawing.Point(3, 6);
			this.Part_FromLabel.Margin = new System.Windows.Forms.Padding(3, 6, 3, 3);
			this.Part_FromLabel.Name = "Part_FromLabel";
			this.Part_FromLabel.Size = new System.Drawing.Size(24, 13);
			this.Part_FromLabel.TabIndex = 4;
			this.Part_FromLabel.Text = "From";
			this.Part_ToLabel.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Horizontal;
			this.Part_ToLabel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.Part_ToLabel.Location = new System.Drawing.Point(120, 6);
			this.Part_ToLabel.Margin = new System.Windows.Forms.Padding(3, 6, 3, 3);
			this.Part_ToLabel.Name = "Part_ToLabel";
			this.Part_ToLabel.Size = new System.Drawing.Size(12, 13);
			this.Part_ToLabel.TabIndex = 5;
			this.Part_ToLabel.Text = "To";
			this.Part_FromValue.Dock = System.Windows.Forms.DockStyle.Fill;
			this.Part_FromValue.EditValue = null;
			this.Part_FromValue.Location = new System.Drawing.Point(33, 3);
			this.Part_FromValue.Name = "Part_FromValue";
			this.Part_FromValue.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			this.Part_FromValue.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			this.Part_FromValue.Properties.DisplayFormat.FormatString = "";
			this.Part_FromValue.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
			this.Part_FromValue.Properties.EditFormat.FormatString = "";
			this.Part_FromValue.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
			this.Part_FromValue.Properties.Mask.EditMask = "";
			this.Part_FromValue.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
			this.Part_FromValue.Size = new System.Drawing.Size(81, 20);
			this.Part_FromValue.TabIndex = 2;
			this.Part_ToValue.Dock = System.Windows.Forms.DockStyle.Fill;
			this.Part_ToValue.EditValue = null;
			this.Part_ToValue.Location = new System.Drawing.Point(138, 3);
			this.Part_ToValue.Name = "Part_ToValue";
			this.Part_ToValue.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			this.Part_ToValue.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			this.Part_ToValue.Properties.DisplayFormat.FormatString = "";
			this.Part_ToValue.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
			this.Part_ToValue.Properties.EditFormat.FormatString = "";
			this.Part_ToValue.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
			this.Part_ToValue.Properties.Mask.EditMask = "";
			this.Part_ToValue.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
			this.Part_ToValue.Size = new System.Drawing.Size(81, 20);
			this.Part_ToValue.TabIndex = 3;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tableLayoutPanel);
			this.Name = "DefaultTemplate";
			this.Padding = new System.Windows.Forms.Padding(9, 1, 9, 1);
			this.Size = new System.Drawing.Size(240, 28);
			this.tableLayoutPanel.ResumeLayout(false);
			this.tableLayoutPanel.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.Part_FromValue.Properties.CalendarTimeProperties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.Part_FromValue.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.Part_ToValue.Properties.CalendarTimeProperties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.Part_ToValue.Properties)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
		private LabelControl Part_FromLabel;
		private LabelControl Part_ToLabel;
		private DateEdit Part_FromValue;
		private DateEdit Part_ToValue;
	}
}
