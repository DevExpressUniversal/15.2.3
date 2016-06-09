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

using System.Drawing;
namespace DevExpress.Utils {
	partial class FindPanel {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			this.updateTimer = new System.Windows.Forms.Timer(this.components);
			this.panel = new DevExpress.Utils.RTLPanel();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.btnClose = new DevExpress.XtraEditors.CloseButton();
			this.teFind = new DevExpress.XtraEditors.MRUEdit();
			this.btnFind = new DevExpress.XtraEditors.SimpleButton();
			this.btnClear = new DevExpress.XtraEditors.SimpleButton();
			this.panel.SuspendLayout();
			this.tableLayoutPanel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.teFind.Properties)).BeginInit();
			this.SuspendLayout();
			this.updateTimer.Interval = 1500;
			this.updateTimer.Tick += new System.EventHandler(this.OnUpdateTimerTick);
			this.panel.BackColor = System.Drawing.Color.Transparent;
			this.panel.Controls.Add(this.tableLayoutPanel1);
			this.panel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel.Location = new System.Drawing.Point(10, 10);
			this.panel.MinimumSize = new System.Drawing.Size(220, 26);
			this.panel.Name = "panel";
			this.panel.Size = new System.Drawing.Size(500, 26);
			this.panel.TabIndex = 0;
			this.tableLayoutPanel1.ColumnCount = 4;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel1.Controls.Add(this.btnClose, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.teFind, 1, 0);
			this.tableLayoutPanel1.Controls.Add(this.btnFind, 2, 0);
			this.tableLayoutPanel1.Controls.Add(this.btnClear, 3, 0);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.MaximumSize = new System.Drawing.Size(500, 0);
			this.tableLayoutPanel1.MinimumSize = new System.Drawing.Size(220, 0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.Padding = new System.Windows.Forms.Padding(3);
			this.tableLayoutPanel1.RowCount = 1;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(500, 26);
			this.tableLayoutPanel1.TabIndex = 0;
			this.btnClose.AllowFocus = false;
			this.btnClose.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.btnClose.AutoWidthInLayoutControl = true;
			this.btnClose.Location = new System.Drawing.Point(3, 5);
			this.btnClose.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
			this.btnClose.Name = "btnClose";
			this.btnClose.Size = new System.Drawing.Size(16, 16);
			this.btnClose.TabIndex = 5;
			this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
			this.teFind.Dock = System.Windows.Forms.DockStyle.Fill;
			this.teFind.Location = new System.Drawing.Point(25, 3);
			this.teFind.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
			this.teFind.Name = "teFind";
			this.teFind.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			this.teFind.Properties.MaxItemCount = 10;
			this.teFind.Properties.ValidateOnEnterKey = false;
			this.teFind.Properties.EditValueChanged += new System.EventHandler(this.teFind_EditValueChanged);
			this.teFind.Size = new System.Drawing.Size(334, 20);
			this.teFind.TabIndex = 15;
			this.teFind.EditValueChanged += new System.EventHandler(this.teFind_EditValueChanged);
			this.teFind.Enter += new System.EventHandler(this.teFind_Enter);
			this.teFind.KeyDown += new System.Windows.Forms.KeyEventHandler(this.teFind_KeyDown);
			this.btnFind.AutoWidthInLayoutControl = true;
			this.btnFind.Location = new System.Drawing.Point(365, 3);
			this.btnFind.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
			this.btnFind.Name = "btnFind";
			this.btnFind.Size = new System.Drawing.Size(62, 20);
			this.btnFind.TabIndex = 19;
			this.btnFind.Text = "Find";
			this.btnFind.Click += new System.EventHandler(this.btnFind_Click);
			this.btnClear.AutoWidthInLayoutControl = true;
			this.btnClear.Location = new System.Drawing.Point(433, 3);
			this.btnClear.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
			this.btnClear.Name = "btnClear";
			this.btnClear.Size = new System.Drawing.Size(64, 20);
			this.btnClear.TabIndex = 20;
			this.btnClear.Text = "Clear";
			this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.panel);
			this.Name = "FindPanel";
			this.Padding = new System.Windows.Forms.Padding(10);
			this.Size = new System.Drawing.Size(520, 46);
			this.panel.ResumeLayout(false);
			this.tableLayoutPanel1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.teFind.Properties)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		private System.Windows.Forms.Timer updateTimer;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		protected DevExpress.XtraEditors.MRUEdit teFind;
		protected DevExpress.XtraEditors.SimpleButton btnFind;
		protected DevExpress.XtraEditors.SimpleButton btnClear;
		protected DevExpress.XtraEditors.CloseButton btnClose;
		private RTLPanel panel;
	}
}
