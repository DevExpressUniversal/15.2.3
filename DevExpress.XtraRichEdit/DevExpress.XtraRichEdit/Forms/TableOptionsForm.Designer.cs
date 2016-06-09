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
	partial class TableOptionsForm {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TableOptionsForm));
			this.lblDefaultCellMargins = new DevExpress.XtraEditors.LabelControl();
			this.lblTop = new DevExpress.XtraEditors.LabelControl();
			this.lblBottom = new DevExpress.XtraEditors.LabelControl();
			this.lblLeft = new DevExpress.XtraEditors.LabelControl();
			this.lblRight = new DevExpress.XtraEditors.LabelControl();
			this.btnOk = new DevExpress.XtraEditors.SimpleButton();
			this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
			this.lblDefaultCellSpacing = new DevExpress.XtraEditors.LabelControl();
			this.cbAllowCellSpacing = new DevExpress.XtraEditors.CheckEdit();
			this.lblOptions = new DevExpress.XtraEditors.LabelControl();
			this.chkResizeToFitContent = new DevExpress.XtraEditors.CheckEdit();
			this.spnSpacingBetweenCells = new DevExpress.XtraRichEdit.Design.RichTextIndentEdit();
			this.spnRightMargin = new DevExpress.XtraRichEdit.Design.RichTextIndentEdit();
			this.spnLeftMargin = new DevExpress.XtraRichEdit.Design.RichTextIndentEdit();
			this.spnBottomMargin = new DevExpress.XtraRichEdit.Design.RichTextIndentEdit();
			this.spnTopMargin = new DevExpress.XtraRichEdit.Design.RichTextIndentEdit();
			((System.ComponentModel.ISupportInitialize)(this.cbAllowCellSpacing.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chkResizeToFitContent.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.spnSpacingBetweenCells.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.spnRightMargin.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.spnLeftMargin.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.spnBottomMargin.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.spnTopMargin.Properties)).BeginInit();
			this.SuspendLayout();
			this.lblDefaultCellMargins.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			resources.ApplyResources(this.lblDefaultCellMargins, "lblDefaultCellMargins");
			this.lblDefaultCellMargins.LineVisible = true;
			this.lblDefaultCellMargins.Name = "lblDefaultCellMargins";
			this.lblTop.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			resources.ApplyResources(this.lblTop, "lblTop");
			this.lblTop.Name = "lblTop";
			this.lblBottom.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			resources.ApplyResources(this.lblBottom, "lblBottom");
			this.lblBottom.Name = "lblBottom";
			this.lblLeft.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			resources.ApplyResources(this.lblLeft, "lblLeft");
			this.lblLeft.Name = "lblLeft";
			this.lblRight.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			resources.ApplyResources(this.lblRight, "lblRight");
			this.lblRight.Name = "lblRight";
			resources.ApplyResources(this.btnOk, "btnOk");
			this.btnOk.Name = "btnOk";
			this.btnOk.Click += new System.EventHandler(this.OnBtnOkClick);
			resources.ApplyResources(this.btnCancel, "btnCancel");
			this.btnCancel.CausesValidation = false;
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Name = "btnCancel";
			this.lblDefaultCellSpacing.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			resources.ApplyResources(this.lblDefaultCellSpacing, "lblDefaultCellSpacing");
			this.lblDefaultCellSpacing.LineVisible = true;
			this.lblDefaultCellSpacing.Name = "lblDefaultCellSpacing";
			resources.ApplyResources(this.cbAllowCellSpacing, "cbAllowCellSpacing");
			this.cbAllowCellSpacing.Name = "cbAllowCellSpacing";
			this.cbAllowCellSpacing.Properties.AutoWidth = true;
			this.cbAllowCellSpacing.Properties.Caption = resources.GetString("cbAllowCellSpacing.Properties.Caption");
			this.lblOptions.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			resources.ApplyResources(this.lblOptions, "lblOptions");
			this.lblOptions.LineVisible = true;
			this.lblOptions.Name = "lblOptions";
			resources.ApplyResources(this.chkResizeToFitContent, "chkResizeToFitContent");
			this.chkResizeToFitContent.Name = "chkResizeToFitContent";
			this.chkResizeToFitContent.Properties.AutoWidth = true;
			this.chkResizeToFitContent.Properties.Caption = resources.GetString("chkResizeToFitContent.Properties.Caption");
			resources.ApplyResources(this.spnSpacingBetweenCells, "spnSpacingBetweenCells");
			this.spnSpacingBetweenCells.Name = "spnSpacingBetweenCells";
			this.spnSpacingBetweenCells.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.spnSpacingBetweenCells.Properties.IsValueInPercent = false;
			resources.ApplyResources(this.spnRightMargin, "spnRightMargin");
			this.spnRightMargin.Name = "spnRightMargin";
			this.spnRightMargin.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.spnRightMargin.Properties.IsValueInPercent = false;
			resources.ApplyResources(this.spnLeftMargin, "spnLeftMargin");
			this.spnLeftMargin.Name = "spnLeftMargin";
			this.spnLeftMargin.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.spnLeftMargin.Properties.IsValueInPercent = false;
			resources.ApplyResources(this.spnBottomMargin, "spnBottomMargin");
			this.spnBottomMargin.Name = "spnBottomMargin";
			this.spnBottomMargin.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.spnBottomMargin.Properties.IsValueInPercent = false;
			resources.ApplyResources(this.spnTopMargin, "spnTopMargin");
			this.spnTopMargin.Name = "spnTopMargin";
			this.spnTopMargin.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.spnTopMargin.Properties.IsValueInPercent = false;
			this.AcceptButton = this.btnOk;
			this.AccessibleRole = System.Windows.Forms.AccessibleRole.Window;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.Controls.Add(this.chkResizeToFitContent);
			this.Controls.Add(this.lblOptions);
			this.Controls.Add(this.spnSpacingBetweenCells);
			this.Controls.Add(this.cbAllowCellSpacing);
			this.Controls.Add(this.lblDefaultCellSpacing);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOk);
			this.Controls.Add(this.spnRightMargin);
			this.Controls.Add(this.spnLeftMargin);
			this.Controls.Add(this.spnBottomMargin);
			this.Controls.Add(this.lblRight);
			this.Controls.Add(this.lblLeft);
			this.Controls.Add(this.lblBottom);
			this.Controls.Add(this.lblTop);
			this.Controls.Add(this.lblDefaultCellMargins);
			this.Controls.Add(this.spnTopMargin);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "TableOptionsForm";
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			((System.ComponentModel.ISupportInitialize)(this.cbAllowCellSpacing.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chkResizeToFitContent.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.spnSpacingBetweenCells.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.spnRightMargin.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.spnLeftMargin.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.spnBottomMargin.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.spnTopMargin.Properties)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		protected DevExpress.XtraRichEdit.Design.RichTextIndentEdit spnTopMargin;
		protected DevExpress.XtraEditors.LabelControl lblDefaultCellMargins;
		protected DevExpress.XtraEditors.LabelControl lblTop;
		protected DevExpress.XtraEditors.LabelControl lblBottom;
		protected DevExpress.XtraEditors.LabelControl lblLeft;
		protected DevExpress.XtraEditors.LabelControl lblRight;
		protected DevExpress.XtraRichEdit.Design.RichTextIndentEdit spnBottomMargin;
		protected DevExpress.XtraRichEdit.Design.RichTextIndentEdit spnLeftMargin;
		protected DevExpress.XtraRichEdit.Design.RichTextIndentEdit spnRightMargin;
		protected DevExpress.XtraEditors.SimpleButton btnOk;
		protected DevExpress.XtraEditors.SimpleButton btnCancel;
		protected DevExpress.XtraEditors.LabelControl lblDefaultCellSpacing;
		protected DevExpress.XtraEditors.CheckEdit cbAllowCellSpacing;
		protected DevExpress.XtraRichEdit.Design.RichTextIndentEdit spnSpacingBetweenCells;
		protected DevExpress.XtraEditors.LabelControl lblOptions;
		protected DevExpress.XtraEditors.CheckEdit chkResizeToFitContent;
	}
}
