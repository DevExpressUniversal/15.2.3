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
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Native;
using DevExpress.XtraSpreadsheet.Localization;
namespace DevExpress.XtraSpreadsheet.Forms {
	public partial class ConfirmPasswordForm : XtraForm {
		ConfirmPasswordViewModel viewModel;
		public ConfirmPasswordForm() {
			InitializeComponent();
		}
		public ConfirmPasswordForm(ConfirmPasswordViewModel viewModel) {
			InitializeComponent();
			this.ViewModel = viewModel;
		}
		public ConfirmPasswordViewModel ViewModel {
			get { return viewModel; }
			set {
				if (value == viewModel)
					return;
				Guard.ArgumentNotNull(value, "viewModel");
				this.viewModel = value;
				SetBindings();
			}
		}
		protected internal virtual void SetBindings() {
			if (ViewModel == null)
				return;
			edtPassword.DataBindings.Add("Text", ViewModel, "Password", true, DataSourceUpdateMode.OnPropertyChanged);
		}
		public static bool AskPasswordConfirmation(ISpreadsheetControl control, string password, IWin32Window owner) {
			SpreadsheetControl spreadsheetControl = control as SpreadsheetControl;
			if (spreadsheetControl == null)
				return false;
			ConfirmPasswordViewModel viewModel = new ConfirmPasswordViewModel();
			using (ConfirmPasswordForm form = new ConfirmPasswordForm(viewModel)) {
				if (spreadsheetControl.ShowModalForm(form, owner) == DialogResult.OK) {
					if (viewModel.Password == password)
						return true;
					else {
						ShowPasswordNotConfirmedMessage(control);
						return false;
					}
				}
			}
			return false;
		}
		static void ShowPasswordNotConfirmedMessage(ISpreadsheetControl control) {
			control.ShowWarningMessage(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_PasswordNotConfirmed));
		}
	}
}
