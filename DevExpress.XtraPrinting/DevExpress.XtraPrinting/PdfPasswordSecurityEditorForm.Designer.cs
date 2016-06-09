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

namespace DevExpress.XtraPrinting.Native.WinControls {
	partial class PdfPasswordSecurityEditorForm {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PdfPasswordSecurityEditorForm));
			this.chbOpenPassword = new DevExpress.XtraEditors.CheckEdit();
			this.teOpenPassword = new DevExpress.XtraEditors.TextEdit();
			this.lbOpenPassword = new DevExpress.XtraEditors.LabelControl();
			this.grpBoxPermissions = new DevExpress.XtraEditors.GroupControl();
			this.chbEnableScreenReaders = new DevExpress.XtraEditors.CheckEdit();
			this.chbEnableCoping = new DevExpress.XtraEditors.CheckEdit();
			this.lbChangesAllowed = new DevExpress.XtraEditors.LabelControl();
			this.lbPrintingAllowed = new DevExpress.XtraEditors.LabelControl();
			this.lkpChangesAllowed = new DevExpress.XtraEditors.LookUpEdit();
			this.lkpPrintingAllowed = new DevExpress.XtraEditors.LookUpEdit();
			this.lbPermissionsPassword = new DevExpress.XtraEditors.LabelControl();
			this.tePermissionsPassword = new DevExpress.XtraEditors.TextEdit();
			this.chbRestrict = new DevExpress.XtraEditors.CheckEdit();
			this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
			this.btnOK = new DevExpress.XtraEditors.SimpleButton();
			((System.ComponentModel.ISupportInitialize)(this.chbOpenPassword.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.teOpenPassword.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.grpBoxPermissions)).BeginInit();
			this.grpBoxPermissions.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.chbEnableScreenReaders.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chbEnableCoping.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lkpChangesAllowed.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lkpPrintingAllowed.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.tePermissionsPassword.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chbRestrict.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.chbOpenPassword, "chbOpenPassword");
			this.chbOpenPassword.Name = "chbOpenPassword";
			this.chbOpenPassword.Properties.AutoWidth = true;
			this.chbOpenPassword.Properties.Caption = resources.GetString("chbOpenPassword.Properties.Caption");
			this.chbOpenPassword.CheckedChanged += new System.EventHandler(this.chbOpenPassword_CheckedChanged);
			resources.ApplyResources(this.teOpenPassword, "teOpenPassword");
			this.teOpenPassword.Name = "teOpenPassword";
			this.teOpenPassword.Properties.PasswordChar = '*';
			this.teOpenPassword.Tag = "";
			resources.ApplyResources(this.lbOpenPassword, "lbOpenPassword");
			this.lbOpenPassword.Name = "lbOpenPassword";
			this.lbOpenPassword.Tag = "";
			resources.ApplyResources(this.grpBoxPermissions, "grpBoxPermissions");
			this.grpBoxPermissions.Controls.Add(this.chbEnableScreenReaders);
			this.grpBoxPermissions.Controls.Add(this.chbEnableCoping);
			this.grpBoxPermissions.Controls.Add(this.lbChangesAllowed);
			this.grpBoxPermissions.Controls.Add(this.lbPrintingAllowed);
			this.grpBoxPermissions.Controls.Add(this.lkpChangesAllowed);
			this.grpBoxPermissions.Controls.Add(this.lkpPrintingAllowed);
			this.grpBoxPermissions.Controls.Add(this.lbPermissionsPassword);
			this.grpBoxPermissions.Controls.Add(this.tePermissionsPassword);
			this.grpBoxPermissions.Controls.Add(this.chbRestrict);
			this.grpBoxPermissions.Name = "grpBoxPermissions";
			resources.ApplyResources(this.chbEnableScreenReaders, "chbEnableScreenReaders");
			this.chbEnableScreenReaders.Name = "chbEnableScreenReaders";
			this.chbEnableScreenReaders.Properties.AutoWidth = true;
			this.chbEnableScreenReaders.Properties.Caption = resources.GetString("chbEnableScreenReaders.Properties.Caption");
			resources.ApplyResources(this.chbEnableCoping, "chbEnableCoping");
			this.chbEnableCoping.Name = "chbEnableCoping";
			this.chbEnableCoping.Properties.AutoWidth = true;
			this.chbEnableCoping.Properties.Caption = resources.GetString("chbEnableCoping.Properties.Caption");
			resources.ApplyResources(this.lbChangesAllowed, "lbChangesAllowed");
			this.lbChangesAllowed.Name = "lbChangesAllowed";
			this.lbChangesAllowed.Tag = "";
			resources.ApplyResources(this.lbPrintingAllowed, "lbPrintingAllowed");
			this.lbPrintingAllowed.Name = "lbPrintingAllowed";
			this.lbPrintingAllowed.Tag = "";
			resources.ApplyResources(this.lkpChangesAllowed, "lkpChangesAllowed");
			this.lkpChangesAllowed.Name = "lkpChangesAllowed";
			this.lkpChangesAllowed.PopupCloseMode = DevExpress.XtraEditors.PopupCloseMode.Normal;
			this.lkpChangesAllowed.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("lkpChangesAllowed.Properties.Buttons"))))});
			this.lkpChangesAllowed.Properties.ShowFooter = false;
			this.lkpChangesAllowed.Properties.ShowHeader = false;
			this.lkpChangesAllowed.Properties.ShowLines = false;
			this.lkpChangesAllowed.Tag = "";
			resources.ApplyResources(this.lkpPrintingAllowed, "lkpPrintingAllowed");
			this.lkpPrintingAllowed.Name = "lkpPrintingAllowed";
			this.lkpPrintingAllowed.PopupCloseMode = DevExpress.XtraEditors.PopupCloseMode.Normal;
			this.lkpPrintingAllowed.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("lkpPrintingAllowed.Properties.Buttons"))))});
			this.lkpPrintingAllowed.Properties.ShowFooter = false;
			this.lkpPrintingAllowed.Properties.ShowHeader = false;
			this.lkpPrintingAllowed.Properties.ShowLines = false;
			this.lkpPrintingAllowed.Tag = "";
			resources.ApplyResources(this.lbPermissionsPassword, "lbPermissionsPassword");
			this.lbPermissionsPassword.Name = "lbPermissionsPassword";
			this.lbPermissionsPassword.Tag = "";
			resources.ApplyResources(this.tePermissionsPassword, "tePermissionsPassword");
			this.tePermissionsPassword.Name = "tePermissionsPassword";
			this.tePermissionsPassword.Properties.PasswordChar = '*';
			this.tePermissionsPassword.Tag = "";
			resources.ApplyResources(this.chbRestrict, "chbRestrict");
			this.chbRestrict.Name = "chbRestrict";
			this.chbRestrict.Properties.AutoWidth = true;
			this.chbRestrict.Properties.Caption = resources.GetString("chbRestrict.Properties.Caption");
			this.chbRestrict.CheckedChanged += new System.EventHandler(this.chbRestrict_CheckedChanged);
			resources.ApplyResources(this.btnCancel, "btnCancel");
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Name = "btnCancel";
			resources.ApplyResources(this.btnOK, "btnOK");
			this.btnOK.Name = "btnOK";
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.grpBoxPermissions);
			this.Controls.Add(this.lbOpenPassword);
			this.Controls.Add(this.teOpenPassword);
			this.Controls.Add(this.chbOpenPassword);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "PdfPasswordSecurityEditorForm";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			((System.ComponentModel.ISupportInitialize)(this.chbOpenPassword.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.teOpenPassword.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.grpBoxPermissions)).EndInit();
			this.grpBoxPermissions.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.chbEnableScreenReaders.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chbEnableCoping.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lkpChangesAllowed.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lkpPrintingAllowed.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.tePermissionsPassword.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chbRestrict.Properties)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		private DevExpress.XtraEditors.CheckEdit chbOpenPassword;
		private DevExpress.XtraEditors.TextEdit teOpenPassword;
		private DevExpress.XtraEditors.LabelControl lbOpenPassword;
		private DevExpress.XtraEditors.GroupControl grpBoxPermissions;
		private DevExpress.XtraEditors.LabelControl lbPermissionsPassword;
		private DevExpress.XtraEditors.TextEdit tePermissionsPassword;
		private DevExpress.XtraEditors.CheckEdit chbRestrict;
		private DevExpress.XtraEditors.CheckEdit chbEnableScreenReaders;
		private DevExpress.XtraEditors.CheckEdit chbEnableCoping;
		private DevExpress.XtraEditors.LabelControl lbChangesAllowed;
		private DevExpress.XtraEditors.LabelControl lbPrintingAllowed;
		private DevExpress.XtraEditors.LookUpEdit lkpChangesAllowed;
		private DevExpress.XtraEditors.LookUpEdit lkpPrintingAllowed;
		private DevExpress.XtraEditors.SimpleButton btnCancel;
		private DevExpress.XtraEditors.SimpleButton btnOK;
	}
}
