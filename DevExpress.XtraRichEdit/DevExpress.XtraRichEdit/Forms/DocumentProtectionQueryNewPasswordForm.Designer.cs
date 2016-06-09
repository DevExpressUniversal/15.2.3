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
	partial class DocumentProtectionQueryNewPasswordForm {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DocumentProtectionQueryNewPasswordForm));
			this.lblPassword = new DevExpress.XtraEditors.LabelControl();
			this.lblRepeatPassword = new DevExpress.XtraEditors.LabelControl();
			this.edtPassword = new DevExpress.XtraEditors.TextEdit();
			this.edtRepeatPassword = new DevExpress.XtraEditors.TextEdit();
			this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
			this.btnOk = new DevExpress.XtraEditors.SimpleButton();
			((System.ComponentModel.ISupportInitialize)(this.edtPassword.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtRepeatPassword.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.lblPassword, "lblPassword");
			this.lblPassword.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			this.lblPassword.Name = "lblPassword";
			resources.ApplyResources(this.lblRepeatPassword, "lblRepeatPassword");
			this.lblRepeatPassword.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			this.lblRepeatPassword.Name = "lblRepeatPassword";
			resources.ApplyResources(this.edtPassword, "edtPassword");
			this.edtPassword.Name = "edtPassword";
			this.edtPassword.Properties.UseSystemPasswordChar = true;
			resources.ApplyResources(this.edtRepeatPassword, "edtRepeatPassword");
			this.edtRepeatPassword.Name = "edtRepeatPassword";
			this.edtRepeatPassword.Properties.UseSystemPasswordChar = true;
			resources.ApplyResources(this.btnCancel, "btnCancel");
			this.btnCancel.CausesValidation = false;
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Name = "btnCancel";
			resources.ApplyResources(this.btnOk, "btnOk");
			this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOk.Name = "btnOk";
			this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
			this.AcceptButton = this.btnOk;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOk);
			this.Controls.Add(this.edtRepeatPassword);
			this.Controls.Add(this.edtPassword);
			this.Controls.Add(this.lblRepeatPassword);
			this.Controls.Add(this.lblPassword);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "DocumentProtectionQueryNewPasswordForm";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			((System.ComponentModel.ISupportInitialize)(this.edtPassword.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtRepeatPassword.Properties)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		protected DevExpress.XtraEditors.LabelControl lblPassword;
		protected DevExpress.XtraEditors.LabelControl lblRepeatPassword;
		protected DevExpress.XtraEditors.TextEdit edtPassword;
		protected DevExpress.XtraEditors.TextEdit edtRepeatPassword;
		protected DevExpress.XtraEditors.SimpleButton btnCancel;
		protected DevExpress.XtraEditors.SimpleButton btnOk;
	}
}
