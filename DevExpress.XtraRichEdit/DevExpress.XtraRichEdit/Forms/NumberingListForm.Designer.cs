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
	partial class NumberingListForm {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NumberingListForm));
			this.btnOk = new DevExpress.XtraEditors.SimpleButton();
			this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
			this.tabControl = new DevExpress.XtraTab.XtraTabControl();
			this.tabBulleted = new DevExpress.XtraTab.XtraTabPage();
			this.bulletedListBox = new DevExpress.XtraRichEdit.Design.NumberingListBox();
			this.tabNumbered = new DevExpress.XtraTab.XtraTabPage();
			this.numberedListBox = new DevExpress.XtraRichEdit.Design.NumberingListBox();
			this.tabOutlineNumbered = new DevExpress.XtraTab.XtraTabPage();
			this.outlineNumberedListBox = new DevExpress.XtraRichEdit.Design.NumberingListBox();
			this.btnCustomize = new DevExpress.XtraEditors.SimpleButton();
			this.restartNumberingCheckEdit = new DevExpress.XtraEditors.CheckEdit();
			this.continuePreviousListCheckEdit = new DevExpress.XtraEditors.CheckEdit();
			((System.ComponentModel.ISupportInitialize)(this.tabControl)).BeginInit();
			this.tabControl.SuspendLayout();
			this.tabBulleted.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.bulletedListBox)).BeginInit();
			this.tabNumbered.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.numberedListBox)).BeginInit();
			this.tabOutlineNumbered.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.outlineNumberedListBox)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.restartNumberingCheckEdit.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.continuePreviousListCheckEdit.Properties)).BeginInit();
			this.SuspendLayout();
			this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			resources.ApplyResources(this.btnOk, "btnOk");
			this.btnOk.Name = "btnOk";
			this.btnOk.Click += new System.EventHandler(this.OnOkClick);
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			resources.ApplyResources(this.btnCancel, "btnCancel");
			this.btnCancel.Name = "btnCancel";
			resources.ApplyResources(this.tabControl, "tabControl");
			this.tabControl.Name = "tabControl";
			this.tabControl.SelectedTabPage = this.tabBulleted;
			this.tabControl.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
			this.tabBulleted,
			this.tabNumbered,
			this.tabOutlineNumbered});
			this.tabBulleted.Controls.Add(this.bulletedListBox);
			this.tabBulleted.Name = "tabBulleted";
			resources.ApplyResources(this.tabBulleted, "tabBulleted");
			resources.ApplyResources(this.bulletedListBox, "bulletedListBox");
			this.bulletedListBox.ItemHeight = 120;
			this.bulletedListBox.Name = "bulletedListBox";
			this.tabNumbered.Controls.Add(this.numberedListBox);
			this.tabNumbered.Name = "tabNumbered";
			resources.ApplyResources(this.tabNumbered, "tabNumbered");
			resources.ApplyResources(this.numberedListBox, "numberedListBox");
			this.numberedListBox.ItemHeight = 120;
			this.numberedListBox.Name = "numberedListBox";
			this.tabOutlineNumbered.Controls.Add(this.outlineNumberedListBox);
			this.tabOutlineNumbered.Name = "tabOutlineNumbered";
			resources.ApplyResources(this.tabOutlineNumbered, "tabOutlineNumbered");
			resources.ApplyResources(this.outlineNumberedListBox, "outlineNumberedListBox");
			this.outlineNumberedListBox.ItemHeight = 120;
			this.outlineNumberedListBox.Name = "outlineNumberedListBox";
			resources.ApplyResources(this.btnCustomize, "btnCustomize");
			this.btnCustomize.Name = "btnCustomize";
			this.btnCustomize.Click += new System.EventHandler(this.OnCustomizeClick);
			resources.ApplyResources(this.restartNumberingCheckEdit, "restartNumberingCheckEdit");
			this.restartNumberingCheckEdit.Name = "restartNumberingCheckEdit";
			this.restartNumberingCheckEdit.Properties.AutoWidth = true;
			this.restartNumberingCheckEdit.Properties.Caption = resources.GetString("restartNumberingCheckEdit.Properties.Caption");
			this.restartNumberingCheckEdit.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
			this.restartNumberingCheckEdit.Properties.RadioGroupIndex = 1;
			resources.ApplyResources(this.continuePreviousListCheckEdit, "continuePreviousListCheckEdit");
			this.continuePreviousListCheckEdit.Name = "continuePreviousListCheckEdit";
			this.continuePreviousListCheckEdit.Properties.AutoWidth = true;
			this.continuePreviousListCheckEdit.Properties.Caption = resources.GetString("continuePreviousListCheckEdit.Properties.Caption");
			this.continuePreviousListCheckEdit.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
			this.continuePreviousListCheckEdit.Properties.RadioGroupIndex = 1;
			this.AcceptButton = this.btnOk;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.Controls.Add(this.continuePreviousListCheckEdit);
			this.Controls.Add(this.restartNumberingCheckEdit);
			this.Controls.Add(this.btnCustomize);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.tabControl);
			this.Controls.Add(this.btnOk);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "NumberingListForm";
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			((System.ComponentModel.ISupportInitialize)(this.tabControl)).EndInit();
			this.tabControl.ResumeLayout(false);
			this.tabBulleted.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.bulletedListBox)).EndInit();
			this.tabNumbered.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.numberedListBox)).EndInit();
			this.tabOutlineNumbered.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.outlineNumberedListBox)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.restartNumberingCheckEdit.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.continuePreviousListCheckEdit.Properties)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		protected DevExpress.XtraEditors.SimpleButton btnOk;
		protected DevExpress.XtraEditors.SimpleButton btnCancel;
		protected DevExpress.XtraTab.XtraTabControl tabControl;
		protected DevExpress.XtraTab.XtraTabPage tabBulleted;
		protected DevExpress.XtraTab.XtraTabPage tabNumbered;
		protected DevExpress.XtraTab.XtraTabPage tabOutlineNumbered;
		protected DevExpress.XtraEditors.SimpleButton btnCustomize;
		protected DevExpress.XtraRichEdit.Design.NumberingListBox bulletedListBox;
		protected DevExpress.XtraRichEdit.Design.NumberingListBox numberedListBox;
		protected DevExpress.XtraRichEdit.Design.NumberingListBox outlineNumberedListBox;
		protected DevExpress.XtraEditors.CheckEdit restartNumberingCheckEdit;
		protected DevExpress.XtraEditors.CheckEdit continuePreviousListCheckEdit;
	}
}
