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

namespace DevExpress.XtraEditors.Filtering.Templates.Range {
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
			DevExpress.XtraEditors.Repository.TrackBarLabel trackBarLabel1 = new DevExpress.XtraEditors.Repository.TrackBarLabel();
			DevExpress.XtraEditors.Repository.TrackBarLabel trackBarLabel2 = new DevExpress.XtraEditors.Repository.TrackBarLabel();
			DevExpress.XtraEditors.Repository.TrackBarLabel trackBarLabel3 = new DevExpress.XtraEditors.Repository.TrackBarLabel();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.Part_ToValue = new DevExpress.XtraEditors.TextEdit();
			this.Part_TrackBar = new DevExpress.XtraEditors.RangeTrackBarControl();
			this.Part_FromValue = new DevExpress.XtraEditors.TextEdit();
			this.Part_FromLabel = new DevExpress.XtraEditors.LabelControl();
			this.Part_ToLabel = new DevExpress.XtraEditors.LabelControl();
			this.tableLayoutPanel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.Part_ToValue.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.Part_TrackBar)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.Part_TrackBar.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.Part_FromValue.Properties)).BeginInit();
			this.SuspendLayout();
			this.tableLayoutPanel1.AutoSize = true;
			this.tableLayoutPanel1.ColumnCount = 4;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.Controls.Add(this.Part_ToValue, 3, 0);
			this.tableLayoutPanel1.Controls.Add(this.Part_TrackBar, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this.Part_FromValue, 1, 0);
			this.tableLayoutPanel1.Controls.Add(this.Part_FromLabel, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.Part_ToLabel, 2, 0);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(9, 1);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 2;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.Size = new System.Drawing.Size(222, 82);
			this.tableLayoutPanel1.TabIndex = 2;
			this.Part_ToValue.Dock = System.Windows.Forms.DockStyle.Fill;
			this.Part_ToValue.Location = new System.Drawing.Point(138, 3);
			this.Part_ToValue.Name = "Part_ToValue";
			this.Part_ToValue.Size = new System.Drawing.Size(81, 20);
			this.Part_ToValue.TabIndex = 3;
			this.tableLayoutPanel1.SetColumnSpan(this.Part_TrackBar, 4);
			this.Part_TrackBar.Dock = System.Windows.Forms.DockStyle.Fill;
			this.Part_TrackBar.EditValue = new DevExpress.XtraEditors.Repository.TrackBarRange(0, 0);
			this.Part_TrackBar.Location = new System.Drawing.Point(3, 29);
			this.Part_TrackBar.Name = "Part_TrackBar";
			this.Part_TrackBar.Properties.EditValueChangedFiringMode = XtraEditors.Controls.EditValueChangedFiringMode.Buffered;
			this.Part_TrackBar.Properties.EditValueChangedDelay = 250;
			this.Part_TrackBar.Properties.LabelAppearance.Options.UseTextOptions = true;
			this.Part_TrackBar.Properties.LabelAppearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
			trackBarLabel1.Label = "From";
			trackBarLabel2.Label = "To";
			trackBarLabel2.Value = 100;
			trackBarLabel3.Label = "Avg";
			trackBarLabel3.Value = 50;
			this.Part_TrackBar.Properties.Labels.AddRange(new DevExpress.XtraEditors.Repository.TrackBarLabel[] {
			trackBarLabel1,
			trackBarLabel2,
			trackBarLabel3});
			this.Part_TrackBar.Properties.Maximum = 100;
			this.Part_TrackBar.Properties.ShowValueToolTip = true;
			this.Part_TrackBar.Properties.TickFrequency = 10;
			this.Part_TrackBar.Properties.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
			this.Part_TrackBar.Size = new System.Drawing.Size(216, 50);
			this.Part_TrackBar.TabIndex = 1;
			this.Part_FromValue.Dock = System.Windows.Forms.DockStyle.Fill;
			this.Part_FromValue.Location = new System.Drawing.Point(33, 3);
			this.Part_FromValue.Name = "Part_FromValue";
			this.Part_FromValue.Size = new System.Drawing.Size(81, 20);
			this.Part_FromValue.TabIndex = 2;
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
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tableLayoutPanel1);
			this.Name = "DefaultTemplate";
			this.Padding = new System.Windows.Forms.Padding(9, 1, 9, 1);
			this.Size = new System.Drawing.Size(240, 84);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.Part_ToValue.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.Part_TrackBar.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.Part_TrackBar)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.Part_FromValue.Properties)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private RangeTrackBarControl Part_TrackBar;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private TextEdit Part_FromValue;
		private TextEdit Part_ToValue;
		private LabelControl Part_FromLabel;
		private LabelControl Part_ToLabel;
	}
}
