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
	partial class ProtectWorkbookForm {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProtectWorkbookForm));
			this.btnOk = new DevExpress.XtraEditors.SimpleButton();
			this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
			this.chkStructure = new DevExpress.XtraEditors.CheckEdit();
			this.lblPassword = new DevExpress.XtraEditors.LabelControl();
			this.edtPassword = new DevExpress.XtraEditors.TextEdit();
			this.lblPermissions = new DevExpress.XtraEditors.LabelControl();
			this.chkWindows = new DevExpress.XtraEditors.CheckEdit();
			((System.ComponentModel.ISupportInitialize)(this.chkStructure.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtPassword.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chkWindows.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.btnOk, "btnOk");
			this.btnOk.Name = "btnOk";
			this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
			resources.ApplyResources(this.btnCancel, "btnCancel");
			this.btnCancel.CausesValidation = false;
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Name = "btnCancel";
			this.chkStructure.AutoSizeInLayoutControl = true;
			resources.ApplyResources(this.chkStructure, "chkStructure");
			this.chkStructure.Name = "chkStructure";
			this.chkStructure.Properties.AccessibleName = resources.GetString("chkStructure.Properties.AccessibleName");
			this.chkStructure.Properties.AccessibleRole = System.Windows.Forms.AccessibleRole.CheckButton;
			this.chkStructure.Properties.AutoWidth = true;
			this.chkStructure.Properties.Caption = resources.GetString("chkStructure.Properties.Caption");
			resources.ApplyResources(this.lblPassword, "lblPassword");
			this.lblPassword.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			this.lblPassword.Name = "lblPassword";
			resources.ApplyResources(this.edtPassword, "edtPassword");
			this.edtPassword.Name = "edtPassword";
			this.edtPassword.Properties.MaxLength = 31;
			this.edtPassword.Properties.UseSystemPasswordChar = true;
			resources.ApplyResources(this.lblPermissions, "lblPermissions");
			this.lblPermissions.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			this.lblPermissions.LineVisible = true;
			this.lblPermissions.Name = "lblPermissions";
			this.chkWindows.AutoSizeInLayoutControl = true;
			resources.ApplyResources(this.chkWindows, "chkWindows");
			this.chkWindows.Name = "chkWindows";
			this.chkWindows.Properties.AccessibleName = resources.GetString("chkWindows.Properties.AccessibleName");
			this.chkWindows.Properties.AccessibleRole = System.Windows.Forms.AccessibleRole.CheckButton;
			this.chkWindows.Properties.AutoWidth = true;
			this.chkWindows.Properties.Caption = resources.GetString("chkWindows.Properties.Caption");
			this.AcceptButton = this.btnOk;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.Controls.Add(this.chkWindows);
			this.Controls.Add(this.lblPermissions);
			this.Controls.Add(this.edtPassword);
			this.Controls.Add(this.lblPassword);
			this.Controls.Add(this.chkStructure);
			this.Controls.Add(this.btnOk);
			this.Controls.Add(this.btnCancel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ProtectWorkbookForm";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			((System.ComponentModel.ISupportInitialize)(this.chkStructure.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtPassword.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chkWindows.Properties)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private XtraEditors.SimpleButton btnOk;
		private XtraEditors.SimpleButton btnCancel;
		private XtraEditors.CheckEdit chkStructure;
		private XtraEditors.LabelControl lblPassword;
		private XtraEditors.TextEdit edtPassword;
		private XtraEditors.LabelControl lblPermissions;
		private XtraEditors.CheckEdit chkWindows;
	}
}
