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
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using DevExpress.XtraSpreadsheet.Forms;
using DevExpress.Xpf.Spreadsheet.Internal;
using DevExpress.Xpf.Spreadsheet.Localization;
using DevExpress.Xpf.Editors;
namespace DevExpress.Xpf.Spreadsheet.Forms {
	public partial class DataValidationControl : UserControl {
		DataValidationViewModel viewModel;
		public DataValidationControl(DataValidationViewModel viewModel) {
			InitializeComponent();
			DataContext = viewModel;
			this.viewModel = viewModel;
			viewModel.PrepareDateTimeFormulas();
		}
		void chkApplyToAllCells_OnCheckedChanged(object sender, EventArgs e) {
			bool? isChecked = ((CheckEdit)sender).IsChecked;
			if (isChecked.HasValue)
				viewModel.SetSelection(isChecked.Value);
		}
	}
	public class DataValidationDXDialog : CustomDXDialog {
		DataValidationViewModel viewModel;
		public DataValidationDXDialog(string title, DataValidationViewModel viewModel)
			: base(title) {
			this.viewModel = viewModel;
		}
		Button ClearAllButton { get { return YesButton; } }
		#region ClearAllButtonClick
		EventHandler onClearAllButtonClick;
		public event EventHandler OnClearAllButtonClick { add { onClearAllButtonClick += value; } remove { onClearAllButtonClick -= value; } }
		void RaiseClearAllButtonClick() {
			if (onClearAllButtonClick != null) {
				onClearAllButtonClick(this, EventArgs.Empty);
			}
		}
		#endregion
		protected override void ApplyDialogButtonProperty() {
			base.ApplyDialogButtonProperty();
			SetButtonVisibilities(true, true, true, false, false);
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			SetupCheckAllButton(ClearAllButton);
		}
		protected override void OnButtonClick(bool? result, MessageBoxResult messageBoxResult) {
			if (messageBoxResult == MessageBoxResult.Yes && ClearAllButton != null) {
				ClearAllButton.Focus();
				RaiseClearAllButtonClick();
				return;
			}
			base.OnButtonClick(result, messageBoxResult);
		}
		void SetupCheckAllButton(Button button) {
			if (button == null)
				return;
			button.Content = XpfSpreadsheetLocalizer.GetString(SpreadsheetControlStringId.Caption_DataValidationFormClearAllBtn);
			button.IsDefault = false;
			button.IsCancel = false;
		}
	}
}
