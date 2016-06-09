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

namespace DevExpress.XtraEditors.Filtering.Templates.Lookup {
	partial class TokenTemplate {
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
			this.Part_ValuesLabel = new DevExpress.XtraEditors.LabelControl();
			this.Part_Values = new DevExpress.XtraEditors.TokenEdit();
			this.tableLayoutPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.Part_Values.Properties)).BeginInit();
			this.SuspendLayout();
			this.tableLayoutPanel.AutoSize = true;
			this.tableLayoutPanel.ColumnCount = 3;
			this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel.Controls.Add(this.Part_ValuesLabel, 0, 0);
			this.tableLayoutPanel.Controls.Add(this.Part_Values, 1, 0);
			this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel.Location = new System.Drawing.Point(9, 1);
			this.tableLayoutPanel.Name = "tableLayoutPanel";
			this.tableLayoutPanel.RowCount = 1;
			this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
			this.tableLayoutPanel.Size = new System.Drawing.Size(222, 26);
			this.tableLayoutPanel.TabIndex = 2;
			this.Part_ValuesLabel.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Horizontal;
			this.Part_ValuesLabel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.Part_ValuesLabel.Location = new System.Drawing.Point(3, 6);
			this.Part_ValuesLabel.Margin = new System.Windows.Forms.Padding(3, 6, 3, 3);
			this.Part_ValuesLabel.Name = "Part_ValuesLabel";
			this.Part_ValuesLabel.Size = new System.Drawing.Size(31, 13);
			this.Part_ValuesLabel.TabIndex = 4;
			this.Part_ValuesLabel.Text = "Values";
			this.Part_Values.Dock = System.Windows.Forms.DockStyle.Fill;
			this.Part_Values.Location = new System.Drawing.Point(40, 3);
			this.Part_Values.Name = "Part_Values";
			this.Part_Values.Properties.EditMode = TokenEditMode.TokenList;
			this.Part_Values.Properties.EditValueType = DevExpress.XtraEditors.TokenEditValueType.List;
			this.Part_Values.Properties.PopupFilterMode = TokenEditPopupFilterMode.Contains;
			this.Part_Values.Properties.Separators.AddRange(new string[] {
			","});
			this.Part_Values.Size = new System.Drawing.Size(179, 20);
			this.Part_Values.TabIndex = 2;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tableLayoutPanel);
			this.Name = "TokenTemplate";
			this.Padding = new System.Windows.Forms.Padding(9, 1, 9, 1);
			this.Size = new System.Drawing.Size(240, 28);
			this.tableLayoutPanel.ResumeLayout(false);
			this.tableLayoutPanel.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.Part_Values.Properties)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
		private LabelControl Part_ValuesLabel;
		private TokenEdit Part_Values;
	}
}
