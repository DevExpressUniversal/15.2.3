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
	partial class DropDownTemplate {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			this.Part_Values = new DevExpress.XtraEditors.CheckedComboBoxEdit();
			this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
			this.Part_MoreButton = new DevExpress.XtraEditors.SimpleButton();
			this.Part_FewerButton = new DevExpress.XtraEditors.SimpleButton();
			((System.ComponentModel.ISupportInitialize)(this.Part_Values.Properties)).BeginInit();
			this.tableLayoutPanel.SuspendLayout();
			this.SuspendLayout();
			this.Part_Values.Dock = System.Windows.Forms.DockStyle.Fill;
			this.Part_Values.EditValue = "";
			this.Part_Values.Location = new System.Drawing.Point(3, 3);
			this.Part_Values.Name = "Part_Values";
			this.Part_Values.Properties.AllowMultiSelect = true;
			this.Part_Values.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			this.Part_Values.Properties.EditValueType = DevExpress.XtraEditors.Repository.EditValueTypeCollection.List;
			this.Part_Values.Size = new System.Drawing.Size(216, 20);
			this.Part_Values.TabIndex = 0;
			this.tableLayoutPanel.ColumnCount = 1;
			this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel.Controls.Add(this.Part_Values, 0, 0);
			this.tableLayoutPanel.Controls.Add(this.Part_MoreButton, 0, 1);
			this.tableLayoutPanel.Controls.Add(this.Part_FewerButton, 0, 1);
			this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel.Location = new System.Drawing.Point(9, 1);
			this.tableLayoutPanel.Name = "tableLayoutPanel";
			this.tableLayoutPanel.RowCount = 2;
			this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel.Size = new System.Drawing.Size(222, 54);
			this.tableLayoutPanel.TabIndex = 1;
			this.Part_MoreButton.AutoSize = true;
			this.Part_MoreButton.Dock = System.Windows.Forms.DockStyle.Left;
			this.Part_MoreButton.Location = new System.Drawing.Point(3, 57);
			this.Part_MoreButton.Name = "Part_MoreButton";
			this.Part_MoreButton.Padding = new System.Windows.Forms.Padding(12, 0, 12, 0);
			this.Part_MoreButton.Size = new System.Drawing.Size(71, 14);
			this.Part_MoreButton.TabIndex = 2;
			this.Part_MoreButton.Text = "More...";
			this.Part_FewerButton.AutoSize = true;
			this.Part_FewerButton.Dock = System.Windows.Forms.DockStyle.Left;
			this.Part_FewerButton.Location = new System.Drawing.Point(3, 29);
			this.Part_FewerButton.Name = "Part_FewerButton";
			this.Part_FewerButton.Padding = new System.Windows.Forms.Padding(12, 0, 12, 0);
			this.Part_FewerButton.Size = new System.Drawing.Size(77, 22);
			this.Part_FewerButton.TabIndex = 3;
			this.Part_FewerButton.Text = "Fewer...";
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tableLayoutPanel);
			this.Name = "DropDownTemplate";
			this.Padding = new System.Windows.Forms.Padding(9, 1, 9, 1);
			this.Size = new System.Drawing.Size(240, 56);
			((System.ComponentModel.ISupportInitialize)(this.Part_Values.Properties)).EndInit();
			this.tableLayoutPanel.ResumeLayout(false);
			this.tableLayoutPanel.PerformLayout();
			this.ResumeLayout(false);
		}
		#endregion
		private CheckedComboBoxEdit Part_Values;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
		private SimpleButton Part_MoreButton;
		private SimpleButton Part_FewerButton;
	}
}
