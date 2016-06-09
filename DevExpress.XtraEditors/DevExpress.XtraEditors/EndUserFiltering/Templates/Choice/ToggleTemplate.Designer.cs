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
	partial class ToggleTemplate {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			this.Part_Value = new DevExpress.XtraEditors.ToggleSwitch();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.Part_ValueLabel = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.Part_Value.Properties)).BeginInit();
			this.tableLayoutPanel1.SuspendLayout();
			this.SuspendLayout();
			this.Part_Value.Location = new System.Drawing.Point(4, 0);
			this.Part_Value.Margin = new System.Windows.Forms.Padding(4, 0, 0, 0);
			this.Part_Value.Name = "Part_Value";
			this.Part_Value.Properties.AutoWidth = true;
			this.Part_Value.Properties.OffText = "";
			this.Part_Value.Properties.OnText = "";
			this.Part_Value.Properties.ShowText = false;
			this.Part_Value.Size = new System.Drawing.Size(70, 24);
			this.Part_Value.TabIndex = 0;
			this.tableLayoutPanel1.ColumnCount = 2;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Controls.Add(this.Part_Value, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.Part_ValueLabel, 1, 0);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(9, 1);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 1;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.Size = new System.Drawing.Size(222, 26);
			this.tableLayoutPanel1.TabIndex = 1;
			this.Part_ValueLabel.AutoSize = true;
			this.Part_ValueLabel.Location = new System.Drawing.Point(77, 6);
			this.Part_ValueLabel.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
			this.Part_ValueLabel.Name = "Part_ValueLabel";
			this.Part_ValueLabel.Size = new System.Drawing.Size(33, 13);
			this.Part_ValueLabel.TabIndex = 1;
			this.Part_ValueLabel.Text = "Value";
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tableLayoutPanel1);
			this.Name = "ToggleTemplate";
			this.Padding = new System.Windows.Forms.Padding(9, 1, 9, 1);
			this.Size = new System.Drawing.Size(240, 28);
			((System.ComponentModel.ISupportInitialize)(this.Part_Value.Properties)).EndInit();
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			this.ResumeLayout(false);
		}
		#endregion
		private ToggleSwitch Part_Value;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.Label Part_ValueLabel;
	}
}
