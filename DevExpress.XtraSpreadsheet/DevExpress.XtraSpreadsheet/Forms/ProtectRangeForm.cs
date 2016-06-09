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

using System;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.XtraSpreadsheet.Forms {
	public partial class ProtectRangeForm : ReferenceEditForm {
		ProtectedRangeViewModel viewModel;
		public ProtectRangeForm() {
			InitializeComponent();
		}
		public ProtectRangeForm(ProtectedRangeViewModel viewModel)
			: base(viewModel) {
			InitializeComponent();
			this.viewModel = viewModel;
			edtReference.SpreadsheetControl = viewModel.Control;
			edtReference.EditValuePrefix = "=";
			edtReference.PositionType = PositionType.Absolute;
			SetBindings();
			SubscribeReferenceEditControlsEvents();
		}
		public new ProtectedRangeViewModel ViewModel { get { return viewModel; } }
		protected internal virtual void SetBindings() {
			if (ViewModel == null)
				return;
			this.DataBindings.Add("Text", ViewModel, "FormText", true, DataSourceUpdateMode.OnPropertyChanged);
			edtTitle.DataBindings.Add("EditValue", ViewModel, "Title", true, DataSourceUpdateMode.OnPropertyChanged);
			edtReference.DataBindings.Add("EditValue", ViewModel, "Reference", true, DataSourceUpdateMode.OnPropertyChanged);
			SetVisibilityBindings();
		}
		void SetVisibilityBindings() {
			if (ViewModel == null)
				return;
			edtPassword.DataBindings.Add("Text", ViewModel, "Password", true, DataSourceUpdateMode.OnPropertyChanged);
			edtPassword.DataBindings.Add("Visible", ViewModel, "AllowToSetNewPassword", true, DataSourceUpdateMode.OnPropertyChanged);
			lblPassword.DataBindings.Add("Visible", ViewModel, "AllowToSetNewPassword", true, DataSourceUpdateMode.OnPropertyChanged);
			btnSetPassword.DataBindings.Add("Visible", ViewModel, "HasPassword", true, DataSourceUpdateMode.OnPropertyChanged);
		}
		void RemoveVisiblityBindings() {
			edtPassword.DataBindings.Clear();
			lblPassword.DataBindings.Clear();
			btnSetPassword.DataBindings.Clear();
		}
		protected override void Collapse(object sender) {
			RemoveVisiblityBindings();
			try {
				base.Collapse(sender);
			}
			finally {
				SetVisibilityBindings();
			}
		}
		protected override void Expand(object sender) {
			RemoveVisiblityBindings();
			try {
				base.Expand(sender);
			}
			finally {
				SetVisibilityBindings();
			}
		}
		protected internal virtual void btnOk_Click(object sender, EventArgs e) {
			this.TopMost = false;
			if (!ViewModel.Validate()) {
				this.TopMost = true;
				return;
			}
			if (String.IsNullOrEmpty(ViewModel.Password) || ViewModel.HasPassword || ConfirmPasswordForm.AskPasswordConfirmation(ViewModel.Control, ViewModel.Password, this)) {
				ViewModel.ApplyChanges();
				this.TopMost = true;
				this.DialogResult = DialogResult.OK;
				Close();
			}
			else
				this.TopMost = true;
		}
		void btnCancel_Click(object sender, EventArgs e) {
			this.DialogResult = DialogResult.Cancel;
			Close();
		}
		void btnSetPassword_Click(object sender, EventArgs e) {
			viewModel.Password = String.Empty;
			viewModel.HasPassword = false;
			viewModel.PasswordChanged = true;
			edtPassword.Focus();
		}
		void btnPermissions_Click(object sender, EventArgs e) {
			this.TopMost = false;
			try {
				viewModel.EditPermissions();
			}
			finally {
				this.TopMost = true;
			}
		}
	}
}
