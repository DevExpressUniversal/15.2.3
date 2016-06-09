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
	partial class ProtectSheetForm {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProtectSheetForm));
			this.btnOk = new DevExpress.XtraEditors.SimpleButton();
			this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
			this.chkProtect = new DevExpress.XtraEditors.CheckEdit();
			this.lblPassword = new DevExpress.XtraEditors.LabelControl();
			this.edtPassword = new DevExpress.XtraEditors.TextEdit();
			this.lblPermissions = new DevExpress.XtraEditors.LabelControl();
			this.edtPermissions = new DevExpress.XtraEditors.CheckedListBoxControl();
			((System.ComponentModel.ISupportInitialize)(this.chkProtect.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtPassword.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtPermissions)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.btnOk, "btnOk");
			this.btnOk.Name = "btnOk";
			this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
			resources.ApplyResources(this.btnCancel, "btnCancel");
			this.btnCancel.CausesValidation = false;
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Name = "btnCancel";
			this.chkProtect.AutoSizeInLayoutControl = true;
			resources.ApplyResources(this.chkProtect, "chkProtect");
			this.chkProtect.Name = "chkProtect";
			this.chkProtect.Properties.AccessibleName = resources.GetString("chkProtect.Properties.AccessibleName");
			this.chkProtect.Properties.AccessibleRole = System.Windows.Forms.AccessibleRole.CheckButton;
			this.chkProtect.Properties.AutoWidth = true;
			this.chkProtect.Properties.Caption = resources.GetString("chkProtect.Properties.Caption");
			resources.ApplyResources(this.lblPassword, "lblPassword");
			this.lblPassword.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			this.lblPassword.Name = "lblPassword";
			resources.ApplyResources(this.edtPassword, "edtPassword");
			this.edtPassword.Name = "edtPassword";
			this.edtPassword.Properties.MaxLength = 31;
			this.edtPassword.Properties.UseSystemPasswordChar = true;
			resources.ApplyResources(this.lblPermissions, "lblPermissions");
			this.lblPermissions.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			this.lblPermissions.Name = "lblPermissions";
			resources.ApplyResources(this.edtPermissions, "edtPermissions");
			this.edtPermissions.AccessibleRole = System.Windows.Forms.AccessibleRole.List;
			this.edtPermissions.CheckOnClick = true;
			this.edtPermissions.Name = "edtPermissions";
			this.AcceptButton = this.btnOk;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.Controls.Add(this.edtPermissions);
			this.Controls.Add(this.lblPermissions);
			this.Controls.Add(this.edtPassword);
			this.Controls.Add(this.lblPassword);
			this.Controls.Add(this.chkProtect);
			this.Controls.Add(this.btnOk);
			this.Controls.Add(this.btnCancel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ProtectSheetForm";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			((System.ComponentModel.ISupportInitialize)(this.chkProtect.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtPassword.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtPermissions)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private XtraEditors.SimpleButton btnOk;
		private XtraEditors.SimpleButton btnCancel;
		private XtraEditors.CheckEdit chkProtect;
		private XtraEditors.LabelControl lblPassword;
		private XtraEditors.TextEdit edtPassword;
		private XtraEditors.LabelControl lblPermissions;
		private XtraEditors.CheckedListBoxControl edtPermissions;
	}
}
