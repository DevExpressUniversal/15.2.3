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

using DevExpress.XtraRichEdit.Design;
namespace DevExpress.XtraRichEdit.Forms {
	partial class ParagraphForm {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ParagraphForm));
			this.xtraTabControl = new DevExpress.XtraTab.XtraTabControl();
			this.indentsAndSpacing = new RichEditTabPage();
			this.chkContextualSpacing = new DevExpress.XtraEditors.CheckEdit();
			this.edtOutlineLevel = new DevExpress.XtraEditors.ComboBoxEdit();
			this.lblOutlineLevel = new DevExpress.XtraEditors.LabelControl();
			this.edtAlignment = new DevExpress.XtraRichEdit.Design.AlignmentEdit();
			this.lblAlignment = new DevExpress.XtraEditors.LabelControl();
			this.paragraphSpacingControl = new DevExpress.XtraRichEdit.Design.ParagraphSpacingControl();
			this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
			this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
			this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
			this.paragraphIndentationControl = new DevExpress.XtraRichEdit.Design.ParagraphIndentationControl();
			this.lineAndPageBreaks = new RichEditTabPage();
			this.chkPageBreakBefore = new DevExpress.XtraEditors.CheckEdit();
			this.chkKeepLinesTogether = new DevExpress.XtraEditors.CheckEdit();
			this.labelControl4 = new DevExpress.XtraEditors.LabelControl();
			this.btnOk = new DevExpress.XtraEditors.SimpleButton();
			this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
			this.btnTabs = new DevExpress.XtraEditors.SimpleButton();
			((System.ComponentModel.ISupportInitialize)(this.xtraTabControl)).BeginInit();
			this.xtraTabControl.SuspendLayout();
			this.indentsAndSpacing.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.chkContextualSpacing.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtOutlineLevel.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtAlignment.Properties)).BeginInit();
			this.lineAndPageBreaks.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.chkPageBreakBefore.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chkKeepLinesTogether.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.xtraTabControl, "xtraTabControl");
			this.xtraTabControl.Name = "xtraTabControl";
			this.xtraTabControl.SelectedTabPage = this.indentsAndSpacing;
			this.xtraTabControl.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
			this.indentsAndSpacing,
			this.lineAndPageBreaks});
			this.indentsAndSpacing.Controls.Add(this.chkContextualSpacing);
			this.indentsAndSpacing.Controls.Add(this.edtOutlineLevel);
			this.indentsAndSpacing.Controls.Add(this.lblOutlineLevel);
			this.indentsAndSpacing.Controls.Add(this.edtAlignment);
			this.indentsAndSpacing.Controls.Add(this.lblAlignment);
			this.indentsAndSpacing.Controls.Add(this.paragraphSpacingControl);
			this.indentsAndSpacing.Controls.Add(this.labelControl3);
			this.indentsAndSpacing.Controls.Add(this.labelControl2);
			this.indentsAndSpacing.Controls.Add(this.labelControl1);
			this.indentsAndSpacing.Controls.Add(this.paragraphIndentationControl);
			this.indentsAndSpacing.Name = "indentsAndSpacing";
			resources.ApplyResources(this.indentsAndSpacing, "indentsAndSpacing");
			resources.ApplyResources(this.chkContextualSpacing, "chkContextualSpacing");
			this.chkContextualSpacing.Name = "chkContextualSpacing";
			this.chkContextualSpacing.Properties.AccessibleName = resources.GetString("chkContextualSpacing.Properties.AccessibleName");
			this.chkContextualSpacing.Properties.AutoWidth = true;
			this.chkContextualSpacing.Properties.Caption = resources.GetString("chkContextualSpacing.Properties.Caption");
			resources.ApplyResources(this.edtOutlineLevel, "edtOutlineLevel");
			this.edtOutlineLevel.Name = "edtOutlineLevel";
			this.edtOutlineLevel.Properties.AccessibleName = resources.GetString("edtOutlineLevel.Properties.AccessibleName");
			this.edtOutlineLevel.Properties.AccessibleRole = System.Windows.Forms.AccessibleRole.ComboBox;
			this.edtOutlineLevel.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("edtOutlineLevel.Properties.Buttons"))))});
			this.edtOutlineLevel.Properties.DropDownRows = 9;
			this.edtOutlineLevel.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			resources.ApplyResources(this.lblOutlineLevel, "lblOutlineLevel");
			this.lblOutlineLevel.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			this.lblOutlineLevel.Name = "lblOutlineLevel";
			resources.ApplyResources(this.edtAlignment, "edtAlignment");
			this.edtAlignment.Name = "edtAlignment";
			this.edtAlignment.Properties.AccessibleName = resources.GetString("edtAlignment.Properties.AccessibleName");
			this.edtAlignment.Properties.AccessibleRole = System.Windows.Forms.AccessibleRole.ComboBox;
			this.edtAlignment.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("edtAlignment.Properties.Buttons"))))});
			resources.ApplyResources(this.lblAlignment, "lblAlignment");
			this.lblAlignment.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			this.lblAlignment.Name = "lblAlignment";
			resources.ApplyResources(this.paragraphSpacingControl, "paragraphSpacingControl");
			this.paragraphSpacingControl.Name = "paragraphSpacingControl";
			resources.ApplyResources(this.labelControl3, "labelControl3");
			this.labelControl3.LineVisible = true;
			this.labelControl3.Name = "labelControl3";
			resources.ApplyResources(this.labelControl2, "labelControl2");
			this.labelControl2.LineVisible = true;
			this.labelControl2.Name = "labelControl2";
			resources.ApplyResources(this.labelControl1, "labelControl1");
			this.labelControl1.LineVisible = true;
			this.labelControl1.Name = "labelControl1";
			resources.ApplyResources(this.paragraphIndentationControl, "paragraphIndentationControl");
			this.paragraphIndentationControl.Name = "paragraphIndentationControl";
			this.lineAndPageBreaks.Controls.Add(this.chkPageBreakBefore);
			this.lineAndPageBreaks.Controls.Add(this.chkKeepLinesTogether);
			this.lineAndPageBreaks.Controls.Add(this.labelControl4);
			this.lineAndPageBreaks.Name = "lineAndPageBreaks";
			resources.ApplyResources(this.lineAndPageBreaks, "lineAndPageBreaks");
			resources.ApplyResources(this.chkPageBreakBefore, "chkPageBreakBefore");
			this.chkPageBreakBefore.Name = "chkPageBreakBefore";
			this.chkPageBreakBefore.Properties.AccessibleName = resources.GetString("chkPageBreakBefore.Properties.AccessibleName");
			this.chkPageBreakBefore.Properties.AutoWidth = true;
			this.chkPageBreakBefore.Properties.Caption = resources.GetString("chkPageBreakBefore.Properties.Caption");
			resources.ApplyResources(this.chkKeepLinesTogether, "chkKeepLinesTogether");
			this.chkKeepLinesTogether.Name = "chkKeepLinesTogether";
			this.chkKeepLinesTogether.Properties.AccessibleName = resources.GetString("chkKeepLinesTogether.Properties.AccessibleName");
			this.chkKeepLinesTogether.Properties.AutoWidth = true;
			this.chkKeepLinesTogether.Properties.Caption = resources.GetString("chkKeepLinesTogether.Properties.Caption");
			resources.ApplyResources(this.labelControl4, "labelControl4");
			this.labelControl4.LineVisible = true;
			this.labelControl4.Name = "labelControl4";
			resources.ApplyResources(this.btnOk, "btnOk");
			this.btnOk.Name = "btnOk";
			this.btnOk.Click += new System.EventHandler(this.OnBtnOkClick);
			resources.ApplyResources(this.btnCancel, "btnCancel");
			this.btnCancel.CausesValidation = false;
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Name = "btnCancel";
			resources.ApplyResources(this.btnTabs, "btnTabs");
			this.btnTabs.Name = "btnTabs";
			this.btnTabs.Click += new System.EventHandler(this.OnTabsClick);
			this.AcceptButton = this.btnOk;
			this.AccessibleRole = System.Windows.Forms.AccessibleRole.Window;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.Controls.Add(this.btnTabs);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.xtraTabControl);
			this.Controls.Add(this.btnOk);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ParagraphForm";
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			((System.ComponentModel.ISupportInitialize)(this.xtraTabControl)).EndInit();
			this.xtraTabControl.ResumeLayout(false);
			this.indentsAndSpacing.ResumeLayout(false);
			this.indentsAndSpacing.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.chkContextualSpacing.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtOutlineLevel.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtAlignment.Properties)).EndInit();
			this.lineAndPageBreaks.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.chkPageBreakBefore.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chkKeepLinesTogether.Properties)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		protected DevExpress.XtraTab.XtraTabControl xtraTabControl;
		protected DevExpress.XtraTab.XtraTabPage indentsAndSpacing;
		protected DevExpress.XtraEditors.LabelControl labelControl2;
		protected DevExpress.XtraEditors.LabelControl labelControl1;
		protected DevExpress.XtraEditors.LabelControl labelControl3;
		protected ParagraphSpacingControl paragraphSpacingControl;
		protected DevExpress.XtraEditors.SimpleButton btnOk;
		protected DevExpress.XtraEditors.SimpleButton btnCancel;
		protected DevExpress.XtraEditors.SimpleButton btnTabs;
		protected DevExpress.XtraEditors.LabelControl lblAlignment;
		protected AlignmentEdit edtAlignment;
		protected ParagraphIndentationControl paragraphIndentationControl;
		protected DevExpress.XtraEditors.ComboBoxEdit edtOutlineLevel;
		protected DevExpress.XtraEditors.LabelControl lblOutlineLevel;
		protected DevExpress.XtraTab.XtraTabPage lineAndPageBreaks;
		protected DevExpress.XtraEditors.LabelControl labelControl4;
		protected DevExpress.XtraEditors.CheckEdit chkPageBreakBefore;
		protected DevExpress.XtraEditors.CheckEdit chkKeepLinesTogether;
		protected DevExpress.XtraEditors.CheckEdit chkContextualSpacing;
	}
}
