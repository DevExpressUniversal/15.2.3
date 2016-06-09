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

namespace DevExpress.XtraRichEdit.Forms {
	partial class TableCellOptionsForm {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TableCellOptionsForm));
			this.lblCellMargins = new DevExpress.XtraEditors.LabelControl();
			this.chkSameAsWholeTable = new DevExpress.XtraEditors.CheckEdit();
			this.spnTopMargin = new DevExpress.XtraRichEdit.Design.RichTextIndentEdit();
			this.lblTop = new DevExpress.XtraEditors.LabelControl();
			this.spnBottomMargin = new DevExpress.XtraRichEdit.Design.RichTextIndentEdit();
			this.lblBottom = new DevExpress.XtraEditors.LabelControl();
			this.lblLeft = new DevExpress.XtraEditors.LabelControl();
			this.spnLeftMargin = new DevExpress.XtraRichEdit.Design.RichTextIndentEdit();
			this.lblRight = new DevExpress.XtraEditors.LabelControl();
			this.spnRightMargin = new DevExpress.XtraRichEdit.Design.RichTextIndentEdit();
			this.lblOptions = new DevExpress.XtraEditors.LabelControl();
			this.chkWrapText = new DevExpress.XtraEditors.CheckEdit();
			this.chkFitText = new DevExpress.XtraEditors.CheckEdit();
			this.btnOk = new DevExpress.XtraEditors.SimpleButton();
			this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
			((System.ComponentModel.ISupportInitialize)(this.chkSameAsWholeTable.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.spnTopMargin.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.spnBottomMargin.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.spnLeftMargin.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.spnRightMargin.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chkWrapText.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chkFitText.Properties)).BeginInit();
			this.SuspendLayout();
			this.lblCellMargins.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			resources.ApplyResources(this.lblCellMargins, "lblCellMargins");
			this.lblCellMargins.LineVisible = true;
			this.lblCellMargins.Name = "lblCellMargins";
			resources.ApplyResources(this.chkSameAsWholeTable, "chkSameAsWholeTable");
			this.chkSameAsWholeTable.Name = "chkSameAsWholeTable";
			this.chkSameAsWholeTable.Properties.AutoWidth = true;
			this.chkSameAsWholeTable.Properties.Caption = resources.GetString("chkSameAsWholeTable.Properties.Caption");
			resources.ApplyResources(this.spnTopMargin, "spnTopMargin");
			this.spnTopMargin.Name = "spnTopMargin";
			this.spnTopMargin.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.spnTopMargin.Properties.IsValueInPercent = false;
			this.lblTop.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			resources.ApplyResources(this.lblTop, "lblTop");
			this.lblTop.Name = "lblTop";
			resources.ApplyResources(this.spnBottomMargin, "spnBottomMargin");
			this.spnBottomMargin.Name = "spnBottomMargin";
			this.spnBottomMargin.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.spnBottomMargin.Properties.IsValueInPercent = false;
			this.lblBottom.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			resources.ApplyResources(this.lblBottom, "lblBottom");
			this.lblBottom.Name = "lblBottom";
			this.lblLeft.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			resources.ApplyResources(this.lblLeft, "lblLeft");
			this.lblLeft.Name = "lblLeft";
			resources.ApplyResources(this.spnLeftMargin, "spnLeftMargin");
			this.spnLeftMargin.Name = "spnLeftMargin";
			this.spnLeftMargin.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.spnLeftMargin.Properties.IsValueInPercent = false;
			this.lblRight.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			resources.ApplyResources(this.lblRight, "lblRight");
			this.lblRight.Name = "lblRight";
			resources.ApplyResources(this.spnRightMargin, "spnRightMargin");
			this.spnRightMargin.Name = "spnRightMargin";
			this.spnRightMargin.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.spnRightMargin.Properties.IsValueInPercent = false;
			this.lblOptions.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			resources.ApplyResources(this.lblOptions, "lblOptions");
			this.lblOptions.LineVisible = true;
			this.lblOptions.Name = "lblOptions";
			resources.ApplyResources(this.chkWrapText, "chkWrapText");
			this.chkWrapText.Name = "chkWrapText";
			this.chkWrapText.Properties.AutoWidth = true;
			this.chkWrapText.Properties.Caption = resources.GetString("chkWrapText.Properties.Caption");
			resources.ApplyResources(this.chkFitText, "chkFitText");
			this.chkFitText.Name = "chkFitText";
			this.chkFitText.Properties.AutoWidth = true;
			this.chkFitText.Properties.Caption = resources.GetString("chkFitText.Properties.Caption");
			resources.ApplyResources(this.btnOk, "btnOk");
			this.btnOk.Name = "btnOk";
			this.btnOk.Click += new System.EventHandler(this.OnBtnOkClick);
			resources.ApplyResources(this.btnCancel, "btnCancel");
			this.btnCancel.CausesValidation = false;
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Name = "btnCancel";
			this.AcceptButton = this.btnOk;
			this.AccessibleRole = System.Windows.Forms.AccessibleRole.Window;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOk);
			this.Controls.Add(this.chkFitText);
			this.Controls.Add(this.chkWrapText);
			this.Controls.Add(this.lblOptions);
			this.Controls.Add(this.spnRightMargin);
			this.Controls.Add(this.lblRight);
			this.Controls.Add(this.spnLeftMargin);
			this.Controls.Add(this.lblLeft);
			this.Controls.Add(this.lblBottom);
			this.Controls.Add(this.spnBottomMargin);
			this.Controls.Add(this.lblTop);
			this.Controls.Add(this.spnTopMargin);
			this.Controls.Add(this.chkSameAsWholeTable);
			this.Controls.Add(this.lblCellMargins);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "TableCellOptionsForm";
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			((System.ComponentModel.ISupportInitialize)(this.chkSameAsWholeTable.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.spnTopMargin.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.spnBottomMargin.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.spnLeftMargin.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.spnRightMargin.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chkWrapText.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chkFitText.Properties)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		protected DevExpress.XtraEditors.LabelControl lblCellMargins;
		protected DevExpress.XtraEditors.CheckEdit chkSameAsWholeTable;
		protected DevExpress.XtraRichEdit.Design.RichTextIndentEdit spnTopMargin;
		protected DevExpress.XtraEditors.LabelControl lblTop;
		protected DevExpress.XtraRichEdit.Design.RichTextIndentEdit spnBottomMargin;
		protected DevExpress.XtraEditors.LabelControl lblBottom;
		protected DevExpress.XtraEditors.LabelControl lblLeft;
		protected DevExpress.XtraRichEdit.Design.RichTextIndentEdit spnLeftMargin;
		protected DevExpress.XtraEditors.LabelControl lblRight;
		protected DevExpress.XtraRichEdit.Design.RichTextIndentEdit spnRightMargin;
		protected DevExpress.XtraEditors.LabelControl lblOptions;
		protected DevExpress.XtraEditors.CheckEdit chkWrapText;
		protected DevExpress.XtraEditors.CheckEdit chkFitText;
		protected DevExpress.XtraEditors.SimpleButton btnOk;
		protected DevExpress.XtraEditors.SimpleButton btnCancel;
	}
}
