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

namespace DevExpress.XtraEditors.Filtering.Templates.Choice {
	partial class CheckTemplate {
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
			this.Part_ValueLabel = new System.Windows.Forms.Label();
			this.Part_Value = new DevExpress.XtraEditors.CheckEdit();
			this.tableLayoutPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.Part_Value.Properties)).BeginInit();
			this.SuspendLayout();
			this.tableLayoutPanel.ColumnCount = 2;
			this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel.Controls.Add(this.Part_ValueLabel, 1, 0);
			this.tableLayoutPanel.Controls.Add(this.Part_Value, 0, 0);
			this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel.Location = new System.Drawing.Point(9, 1);
			this.tableLayoutPanel.Name = "tableLayoutPanel";
			this.tableLayoutPanel.RowCount = 1;
			this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel.Size = new System.Drawing.Size(222, 26);
			this.tableLayoutPanel.TabIndex = 2;
			this.Part_ValueLabel.AutoSize = true;
			this.Part_ValueLabel.Location = new System.Drawing.Point(25, 6);
			this.Part_ValueLabel.Margin = new System.Windows.Forms.Padding(1, 6, 3, 0);
			this.Part_ValueLabel.Name = "Part_ValueLabel";
			this.Part_ValueLabel.Size = new System.Drawing.Size(33, 13);
			this.Part_ValueLabel.TabIndex = 1;
			this.Part_ValueLabel.Text = "Value";
			this.Part_Value.Location = new System.Drawing.Point(5, 3);
			this.Part_Value.Margin = new System.Windows.Forms.Padding(5, 3, 0, 0);
			this.Part_Value.Name = "Part_Value";
			this.Part_Value.Properties.AllowGrayed = true;
			this.Part_Value.Properties.AutoWidth = true;
			this.Part_Value.Properties.Caption = "";
			this.Part_Value.Size = new System.Drawing.Size(19, 19);
			this.Part_Value.TabIndex = 0;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tableLayoutPanel);
			this.Name = "CheckTemplate";
			this.Padding = new System.Windows.Forms.Padding(9, 1, 9, 1);
			this.Size = new System.Drawing.Size(240, 28);
			this.tableLayoutPanel.ResumeLayout(false);
			this.tableLayoutPanel.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.Part_Value.Properties)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
		private System.Windows.Forms.Label Part_ValueLabel;
		private CheckEdit Part_Value;
	}
}
