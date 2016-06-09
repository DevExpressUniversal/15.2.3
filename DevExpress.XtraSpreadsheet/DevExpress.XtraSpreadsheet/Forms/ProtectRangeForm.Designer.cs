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
	partial class ProtectRangeForm {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProtectRangeForm));
			this.btnOk = new DevExpress.XtraEditors.SimpleButton();
			this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
			this.lblPassword = new DevExpress.XtraEditors.LabelControl();
			this.edtPassword = new DevExpress.XtraEditors.TextEdit();
			this.edtReference = new DevExpress.XtraSpreadsheet.ReferenceEditControl();
			this.lblReference = new DevExpress.XtraEditors.LabelControl();
			this.edtTitle = new DevExpress.XtraEditors.TextEdit();
			this.lblTitle = new DevExpress.XtraEditors.LabelControl();
			this.btnSetPassword = new DevExpress.XtraEditors.SimpleButton();
			this.btnPermissions = new DevExpress.XtraEditors.SimpleButton();
			((System.ComponentModel.ISupportInitialize)(this.edtPassword.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtReference.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtTitle.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.btnOk, "btnOk");
			this.btnOk.Name = "btnOk";
			this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
			resources.ApplyResources(this.btnCancel, "btnCancel");
			this.btnCancel.CausesValidation = false;
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			resources.ApplyResources(this.lblPassword, "lblPassword");
			this.lblPassword.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			this.lblPassword.Name = "lblPassword";
			resources.ApplyResources(this.edtPassword, "edtPassword");
			this.edtPassword.Name = "edtPassword";
			this.edtPassword.Properties.MaxLength = 31;
			this.edtPassword.Properties.UseSystemPasswordChar = true;
			this.edtReference.Activated = false;
			resources.ApplyResources(this.edtReference, "edtReference");
			this.edtReference.EditValuePrefix = null;
			this.edtReference.Name = "edtReference";
			this.edtReference.PositionType = DevExpress.XtraSpreadsheet.Model.PositionType.Relative;
			this.edtReference.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.edtReference.SpreadsheetControl = null;
			resources.ApplyResources(this.lblReference, "lblReference");
			this.lblReference.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			this.lblReference.Name = "lblReference";
			resources.ApplyResources(this.edtTitle, "edtTitle");
			this.edtTitle.Name = "edtTitle";
			this.edtTitle.Properties.MaxLength = 31;
			resources.ApplyResources(this.lblTitle, "lblTitle");
			this.lblTitle.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			this.lblTitle.Name = "lblTitle";
			resources.ApplyResources(this.btnSetPassword, "btnSetPassword");
			this.btnSetPassword.CausesValidation = false;
			this.btnSetPassword.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnSetPassword.Name = "btnSetPassword";
			this.btnSetPassword.Click += new System.EventHandler(this.btnSetPassword_Click);
			resources.ApplyResources(this.btnPermissions, "btnPermissions");
			this.btnPermissions.CausesValidation = false;
			this.btnPermissions.Name = "btnPermissions";
			this.btnPermissions.Click += new System.EventHandler(this.btnPermissions_Click);
			this.AcceptButton = this.btnOk;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.Controls.Add(this.btnPermissions);
			this.Controls.Add(this.btnSetPassword);
			this.Controls.Add(this.lblTitle);
			this.Controls.Add(this.edtTitle);
			this.Controls.Add(this.lblReference);
			this.Controls.Add(this.edtReference);
			this.Controls.Add(this.edtPassword);
			this.Controls.Add(this.lblPassword);
			this.Controls.Add(this.btnOk);
			this.Controls.Add(this.btnCancel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "ProtectRangeForm";
			((System.ComponentModel.ISupportInitialize)(this.edtPassword.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtReference.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtTitle.Properties)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private XtraEditors.SimpleButton btnOk;
		private XtraEditors.SimpleButton btnCancel;
		private XtraEditors.LabelControl lblPassword;
		private XtraEditors.TextEdit edtPassword;
		private ReferenceEditControl edtReference;
		private XtraEditors.LabelControl lblReference;
		private XtraEditors.TextEdit edtTitle;
		private XtraEditors.LabelControl lblTitle;
		private XtraEditors.SimpleButton btnSetPassword;
		private XtraEditors.SimpleButton btnPermissions;
	}
}
