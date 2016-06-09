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

namespace DevExpress.XtraSpreadsheet.Forms {
	partial class FindReplaceForm {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FindReplaceForm));
			this.btnClose = new DevExpress.XtraEditors.SimpleButton();
			this.tabControl = new DevExpress.XtraTab.XtraTabControl();
			this.pageFind = new DevExpress.XtraTab.XtraTabPage();
			this.panel = new DevExpress.XtraEditors.PanelControl();
			this.lblFindWhat = new DevExpress.XtraEditors.LabelControl();
			this.chkMatchEntireCellContents = new DevExpress.XtraEditors.CheckEdit();
			this.lblSearch = new DevExpress.XtraEditors.LabelControl();
			this.chkMatchCase = new DevExpress.XtraEditors.CheckEdit();
			this.lblLookIn = new DevExpress.XtraEditors.LabelControl();
			this.edtSearchIn = new DevExpress.XtraEditors.LookUpEdit();
			this.edtFindWhat = new DevExpress.XtraEditors.TextEdit();
			this.edtSearchBy = new DevExpress.XtraEditors.LookUpEdit();
			this.lblReplaceWith = new DevExpress.XtraEditors.LabelControl();
			this.edtReplaceWith = new DevExpress.XtraEditors.TextEdit();
			this.pageReplace = new DevExpress.XtraTab.XtraTabPage();
			this.btnFindNext = new DevExpress.XtraEditors.SimpleButton();
			this.btnReplace = new DevExpress.XtraEditors.SimpleButton();
			this.btnReplaceAll = new DevExpress.XtraEditors.SimpleButton();
			((System.ComponentModel.ISupportInitialize)(this.tabControl)).BeginInit();
			this.tabControl.SuspendLayout();
			this.pageFind.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.panel)).BeginInit();
			this.panel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.chkMatchEntireCellContents.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chkMatchCase.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtSearchIn.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtFindWhat.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtSearchBy.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtReplaceWith.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.btnClose, "btnClose");
			this.btnClose.CausesValidation = false;
			this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnClose.Name = "btnClose";
			this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
			resources.ApplyResources(this.tabControl, "tabControl");
			this.tabControl.Name = "tabControl";
			this.tabControl.SelectedTabPage = this.pageFind;
			this.tabControl.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
			this.pageFind,
			this.pageReplace});
			this.tabControl.SelectedPageChanging += new DevExpress.XtraTab.TabPageChangingEventHandler(this.tabControl_SelectedPageChanging);
			this.pageFind.Controls.Add(this.panel);
			this.pageFind.Name = "pageFind";
			resources.ApplyResources(this.pageFind, "pageFind");
			this.panel.Appearance.BackColor = ((System.Drawing.Color)(resources.GetObject("panel.Appearance.BackColor")));
			this.panel.Appearance.Options.UseBackColor = true;
			this.panel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.panel.Controls.Add(this.lblFindWhat);
			this.panel.Controls.Add(this.chkMatchEntireCellContents);
			this.panel.Controls.Add(this.lblSearch);
			this.panel.Controls.Add(this.chkMatchCase);
			this.panel.Controls.Add(this.lblLookIn);
			this.panel.Controls.Add(this.edtSearchIn);
			this.panel.Controls.Add(this.edtFindWhat);
			this.panel.Controls.Add(this.edtSearchBy);
			this.panel.Controls.Add(this.lblReplaceWith);
			this.panel.Controls.Add(this.edtReplaceWith);
			resources.ApplyResources(this.panel, "panel");
			this.panel.Name = "panel";
			resources.ApplyResources(this.lblFindWhat, "lblFindWhat");
			this.lblFindWhat.Name = "lblFindWhat";
			resources.ApplyResources(this.chkMatchEntireCellContents, "chkMatchEntireCellContents");
			this.chkMatchEntireCellContents.Name = "chkMatchEntireCellContents";
			this.chkMatchEntireCellContents.Properties.AutoWidth = true;
			this.chkMatchEntireCellContents.Properties.Caption = resources.GetString("chkMatchEntireCellContents.Properties.Caption");
			resources.ApplyResources(this.lblSearch, "lblSearch");
			this.lblSearch.Name = "lblSearch";
			resources.ApplyResources(this.chkMatchCase, "chkMatchCase");
			this.chkMatchCase.Name = "chkMatchCase";
			this.chkMatchCase.Properties.AutoWidth = true;
			this.chkMatchCase.Properties.Caption = resources.GetString("chkMatchCase.Properties.Caption");
			resources.ApplyResources(this.lblLookIn, "lblLookIn");
			this.lblLookIn.Name = "lblLookIn";
			resources.ApplyResources(this.edtSearchIn, "edtSearchIn");
			this.edtSearchIn.Name = "edtSearchIn";
			this.edtSearchIn.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("edtSearchIn.Properties.Buttons"))))});
			this.edtSearchIn.Properties.ShowFooter = false;
			this.edtSearchIn.Properties.ShowHeader = false;
			resources.ApplyResources(this.edtFindWhat, "edtFindWhat");
			this.edtFindWhat.Name = "edtFindWhat";
			resources.ApplyResources(this.edtSearchBy, "edtSearchBy");
			this.edtSearchBy.Name = "edtSearchBy";
			this.edtSearchBy.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("edtSearchBy.Properties.Buttons"))))});
			this.edtSearchBy.Properties.ShowFooter = false;
			this.edtSearchBy.Properties.ShowHeader = false;
			resources.ApplyResources(this.lblReplaceWith, "lblReplaceWith");
			this.lblReplaceWith.Name = "lblReplaceWith";
			resources.ApplyResources(this.edtReplaceWith, "edtReplaceWith");
			this.edtReplaceWith.Name = "edtReplaceWith";
			this.pageReplace.Name = "pageReplace";
			resources.ApplyResources(this.pageReplace, "pageReplace");
			resources.ApplyResources(this.btnFindNext, "btnFindNext");
			this.btnFindNext.CausesValidation = false;
			this.btnFindNext.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnFindNext.Name = "btnFindNext";
			this.btnFindNext.Click += new System.EventHandler(this.btnFindNext_Click);
			resources.ApplyResources(this.btnReplace, "btnReplace");
			this.btnReplace.CausesValidation = false;
			this.btnReplace.Name = "btnReplace";
			this.btnReplace.Click += new System.EventHandler(this.btnReplace_Click);
			resources.ApplyResources(this.btnReplaceAll, "btnReplaceAll");
			this.btnReplaceAll.CausesValidation = false;
			this.btnReplaceAll.Name = "btnReplaceAll";
			this.btnReplaceAll.Click += new System.EventHandler(this.btnReplaceAll_Click);
			this.AcceptButton = this.btnFindNext;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnClose;
			this.Controls.Add(this.btnReplaceAll);
			this.Controls.Add(this.btnReplace);
			this.Controls.Add(this.btnFindNext);
			this.Controls.Add(this.tabControl);
			this.Controls.Add(this.btnClose);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FindReplaceForm";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			((System.ComponentModel.ISupportInitialize)(this.tabControl)).EndInit();
			this.tabControl.ResumeLayout(false);
			this.pageFind.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.panel)).EndInit();
			this.panel.ResumeLayout(false);
			this.panel.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.chkMatchEntireCellContents.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chkMatchCase.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtSearchIn.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtFindWhat.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtSearchBy.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtReplaceWith.Properties)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		private XtraEditors.SimpleButton btnClose;
		private XtraTab.XtraTabControl tabControl;
		private XtraTab.XtraTabPage pageFind;
		private XtraEditors.LookUpEdit edtSearchIn;
		private XtraEditors.LookUpEdit edtSearchBy;
		private XtraEditors.TextEdit edtReplaceWith;
		private XtraEditors.LabelControl lblReplaceWith;
		private XtraEditors.TextEdit edtFindWhat;
		private XtraEditors.LabelControl lblLookIn;
		private XtraEditors.LabelControl lblSearch;
		private XtraEditors.LabelControl lblFindWhat;
		private XtraEditors.CheckEdit chkMatchEntireCellContents;
		private XtraEditors.CheckEdit chkMatchCase;
		private XtraEditors.SimpleButton btnFindNext;
		private XtraEditors.SimpleButton btnReplace;
		private XtraEditors.PanelControl panel;
		private XtraTab.XtraTabPage pageReplace;
		private XtraEditors.SimpleButton btnReplaceAll;
	}
}
