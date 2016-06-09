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

namespace DevExpress.XtraEditors.FormatRule.Forms {
	partial class FormatRuleEditFormBase {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			this.pnlButtons = new DevExpress.XtraEditors.PanelControl();
			this.sbOK = new DevExpress.XtraEditors.SimpleButton();
			this.pnlSeparator1 = new DevExpress.XtraEditors.PanelControl();
			this.sbCancel = new DevExpress.XtraEditors.SimpleButton();
			this.pnlCheckEdit = new DevExpress.XtraEditors.PanelControl();
			this.ceRowFormat = new DevExpress.XtraEditors.CheckEdit();
			this.pnlCaption = new DevExpress.XtraEditors.PanelControl();
			this.lbCaption = new DevExpress.XtraEditors.LabelControl();
			this.pnlMain = new DevExpress.XtraEditors.PanelControl();
			this.pnlEditors = new DevExpress.XtraEditors.PanelControl();
			this.lbWith = new DevExpress.XtraEditors.LabelControl();
			this.pnlSeparator2 = new DevExpress.XtraEditors.PanelControl();
			this.icbFormat = new DevExpress.XtraEditors.ImageComboBoxEdit();
			this.pnlRightIndent = new DevExpress.XtraEditors.PanelControl();
			((System.ComponentModel.ISupportInitialize)(this.pnlButtons)).BeginInit();
			this.pnlButtons.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlSeparator1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlCheckEdit)).BeginInit();
			this.pnlCheckEdit.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.ceRowFormat.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlCaption)).BeginInit();
			this.pnlCaption.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlMain)).BeginInit();
			this.pnlMain.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlEditors)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlSeparator2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.icbFormat.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlRightIndent)).BeginInit();
			this.SuspendLayout();
			this.pnlButtons.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlButtons.Controls.Add(this.sbOK);
			this.pnlButtons.Controls.Add(this.pnlSeparator1);
			this.pnlButtons.Controls.Add(this.sbCancel);
			this.pnlButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.pnlButtons.Location = new System.Drawing.Point(15, 139);
			this.pnlButtons.Name = "pnlButtons";
			this.pnlButtons.Size = new System.Drawing.Size(449, 25);
			this.pnlButtons.TabIndex = 3;
			this.sbOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.sbOK.Dock = System.Windows.Forms.DockStyle.Right;
			this.sbOK.Location = new System.Drawing.Point(274, 0);
			this.sbOK.Name = "sbOK";
			this.sbOK.Size = new System.Drawing.Size(80, 25);
			this.sbOK.TabIndex = 0;
			this.sbOK.Text = "OK";
			this.pnlSeparator1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlSeparator1.Dock = System.Windows.Forms.DockStyle.Right;
			this.pnlSeparator1.Location = new System.Drawing.Point(354, 0);
			this.pnlSeparator1.Name = "pnlSeparator1";
			this.pnlSeparator1.Size = new System.Drawing.Size(10, 25);
			this.pnlSeparator1.TabIndex = 2;
			this.sbCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.sbCancel.Dock = System.Windows.Forms.DockStyle.Right;
			this.sbCancel.Location = new System.Drawing.Point(364, 0);
			this.sbCancel.Name = "sbCancel";
			this.sbCancel.Size = new System.Drawing.Size(85, 25);
			this.sbCancel.TabIndex = 1;
			this.sbCancel.Text = "Cancel";
			this.pnlCheckEdit.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlCheckEdit.Controls.Add(this.ceRowFormat);
			this.pnlCheckEdit.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.pnlCheckEdit.Location = new System.Drawing.Point(15, 114);
			this.pnlCheckEdit.Name = "pnlCheckEdit";
			this.pnlCheckEdit.Padding = new System.Windows.Forms.Padding(0, 0, 0, 10);
			this.pnlCheckEdit.Size = new System.Drawing.Size(449, 25);
			this.pnlCheckEdit.TabIndex = 2;
			this.ceRowFormat.Dock = System.Windows.Forms.DockStyle.Left;
			this.ceRowFormat.Location = new System.Drawing.Point(0, 0);
			this.ceRowFormat.Name = "ceRowFormat";
			this.ceRowFormat.Properties.AutoWidth = true;
			this.ceRowFormat.Properties.Caption = "Apply formatting to an entire row";
			this.ceRowFormat.Size = new System.Drawing.Size(182, 19);
			this.ceRowFormat.TabIndex = 0;
			this.pnlCaption.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlCaption.Controls.Add(this.lbCaption);
			this.pnlCaption.Dock = System.Windows.Forms.DockStyle.Top;
			this.pnlCaption.Location = new System.Drawing.Point(15, 15);
			this.pnlCaption.Name = "pnlCaption";
			this.pnlCaption.Size = new System.Drawing.Size(449, 18);
			this.pnlCaption.TabIndex = 0;
			this.lbCaption.Dock = System.Windows.Forms.DockStyle.Left;
			this.lbCaption.Location = new System.Drawing.Point(0, 0);
			this.lbCaption.Name = "lbCaption";
			this.lbCaption.Size = new System.Drawing.Size(37, 13);
			this.lbCaption.TabIndex = 0;
			this.lbCaption.Text = "Caption";
			this.pnlMain.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlMain.Controls.Add(this.pnlEditors);
			this.pnlMain.Controls.Add(this.lbWith);
			this.pnlMain.Controls.Add(this.pnlSeparator2);
			this.pnlMain.Controls.Add(this.icbFormat);
			this.pnlMain.Controls.Add(this.pnlRightIndent);
			this.pnlMain.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnlMain.Location = new System.Drawing.Point(15, 33);
			this.pnlMain.Name = "pnlMain";
			this.pnlMain.Padding = new System.Windows.Forms.Padding(0, 20, 0, 20);
			this.pnlMain.Size = new System.Drawing.Size(449, 81);
			this.pnlMain.TabIndex = 1;
			this.pnlEditors.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlEditors.Dock = System.Windows.Forms.DockStyle.Right;
			this.pnlEditors.Location = new System.Drawing.Point(227, 20);
			this.pnlEditors.Name = "pnlEditors";
			this.pnlEditors.Size = new System.Drawing.Size(0, 41);
			this.pnlEditors.TabIndex = 0;
			this.lbWith.Dock = System.Windows.Forms.DockStyle.Right;
			this.lbWith.Location = new System.Drawing.Point(227, 20);
			this.lbWith.Name = "lbWith";
			this.lbWith.Size = new System.Drawing.Size(20, 13);
			this.lbWith.TabIndex = 12;
			this.lbWith.Text = "with";
			this.pnlSeparator2.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlSeparator2.Dock = System.Windows.Forms.DockStyle.Right;
			this.pnlSeparator2.Location = new System.Drawing.Point(247, 20);
			this.pnlSeparator2.Name = "pnlSeparator2";
			this.pnlSeparator2.Size = new System.Drawing.Size(10, 41);
			this.pnlSeparator2.TabIndex = 11;
			this.icbFormat.Dock = System.Windows.Forms.DockStyle.Right;
			this.icbFormat.Location = new System.Drawing.Point(257, 20);
			this.icbFormat.Name = "icbFormat";
			this.icbFormat.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			this.icbFormat.Size = new System.Drawing.Size(192, 20);
			this.icbFormat.TabIndex = 10;
			this.pnlRightIndent.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlRightIndent.Dock = System.Windows.Forms.DockStyle.Right;
			this.pnlRightIndent.Location = new System.Drawing.Point(449, 20);
			this.pnlRightIndent.Name = "pnlRightIndent";
			this.pnlRightIndent.Size = new System.Drawing.Size(0, 41);
			this.pnlRightIndent.TabIndex = 13;
			this.AcceptButton = this.sbOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.sbCancel;
			this.ClientSize = new System.Drawing.Size(479, 179);
			this.Controls.Add(this.pnlMain);
			this.Controls.Add(this.pnlCaption);
			this.Controls.Add(this.pnlCheckEdit);
			this.Controls.Add(this.pnlButtons);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "FormatRuleEditFormBase";
			this.Padding = new System.Windows.Forms.Padding(15);
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "FormatRuleEditFormBase";
			((System.ComponentModel.ISupportInitialize)(this.pnlButtons)).EndInit();
			this.pnlButtons.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pnlSeparator1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlCheckEdit)).EndInit();
			this.pnlCheckEdit.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.ceRowFormat.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlCaption)).EndInit();
			this.pnlCaption.ResumeLayout(false);
			this.pnlCaption.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlMain)).EndInit();
			this.pnlMain.ResumeLayout(false);
			this.pnlMain.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlEditors)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlSeparator2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.icbFormat.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlRightIndent)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		private PanelControl pnlButtons;
		private SimpleButton sbOK;
		private PanelControl pnlSeparator1;
		private SimpleButton sbCancel;
		private PanelControl pnlCheckEdit;
		private CheckEdit ceRowFormat;
		private PanelControl pnlCaption;
		private PanelControl pnlMain;
		private LabelControl lbCaption;
		private ImageComboBoxEdit icbFormat;
		private LabelControl lbWith;
		private PanelControl pnlSeparator2;
		private PanelControl pnlEditors;
		private PanelControl pnlRightIndent;
	}
}
