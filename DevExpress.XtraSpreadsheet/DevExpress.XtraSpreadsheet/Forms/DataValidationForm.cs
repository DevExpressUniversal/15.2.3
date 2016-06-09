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
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.Utils;
using DevExpress.XtraTab;
namespace DevExpress.XtraSpreadsheet.Forms {
	public partial class DataValidationForm : ReferenceEditForm {
		readonly DataValidationViewModel viewModel;
		public DataValidationForm() {
			InitializeComponent();
		}
		public DataValidationForm(DataValidationViewModel viewModel) 
			: base(viewModel) {
			Guard.ArgumentNotNull(viewModel, "viewModel");
			this.viewModel = viewModel;
			InitializeComponent();
			InitializeReferenceEditControls();
			viewModel.PrepareDateTimeFormulas();
			SetBindings();
		}
		protected new DataValidationViewModel ViewModel { get { return viewModel; } }
		void InitializeReferenceEditControls() {
			edtFormula1.SpreadsheetControl = viewModel.Control;
			edtFormula1.EditValuePrefix = "=";
			edtFormula1.PositionType = Model.PositionType.Absolute;
			edtFormula2.SpreadsheetControl = viewModel.Control;
			edtFormula2.EditValuePrefix = "=";
			edtFormula2.PositionType = Model.PositionType.Absolute;
			SubscribeReferenceEditControlsEvents();
		}
		#region SetBindings
		void SetBindings() {
			SetSettingsTabBindings();
			SetInputMessageTabBindings();
			SetErrorAlertTabBindings();
		}
		void SetVisibilityBindings() {
			if (ViewModel == null)
				return;
			int index = xtraTabControl.SelectedTabPageIndex;
			if (index == 0)
				SetSettingsTabVisibility();
			else if (index == 2)
				SetErrorAlertTabVisibility();
		}
		void ClearVisibilityBindings() {
			if (ViewModel == null)
				return;
			foreach (Control control in xtraTabControl.SelectedTabPage.Controls) {
				ControlBindingsCollection bindings = control.DataBindings;
				int count = bindings.Count;
				for (int i = count - 1; i >= 0; i--) {
					Binding binding = bindings[i];
					if (binding.PropertyName == "Visible")
						bindings.Remove(binding);
				}
			}
		}
		void SetSettingsTabBindings() {
			this.chkApplyToAllCells.DataBindings.Add("Enabled", ViewModel, "CanApplyChangesToAllCells", true, DataSourceUpdateMode.Never);
			this.chkIgnoreBlank.DataBindings.Add("Checked", ViewModel, "IgnoreBlank", true, DataSourceUpdateMode.OnPropertyChanged);
			this.chkIgnoreBlank.DataBindings.Add("Enabled", ViewModel, "Formula1Visible", true, DataSourceUpdateMode.OnPropertyChanged);
			this.chkDropdown.DataBindings.Add("Checked", ViewModel, "InCellDropDown", true, DataSourceUpdateMode.OnPropertyChanged);
			this.edtType.Properties.DataSource = ViewModel.TypeDataSource;
			this.edtType.DataBindings.Add("EditValue", ViewModel, "TypeValue", true, DataSourceUpdateMode.OnPropertyChanged);
			this.lblData.DataBindings.Add("Enabled", ViewModel, "CanChangeOperator", true, DataSourceUpdateMode.OnPropertyChanged);
			this.edtData.Properties.DataSource = ViewModel.OperatorDataSource;
			this.edtData.DataBindings.Add("EditValue", ViewModel, "OperatorValue", true, DataSourceUpdateMode.OnPropertyChanged);
			this.edtData.DataBindings.Add("Enabled", ViewModel, "CanChangeOperator", true, DataSourceUpdateMode.OnPropertyChanged);
			this.lblFormula1.DataBindings.Add("Text", ViewModel, "Formula1Caption", true, DataSourceUpdateMode.OnPropertyChanged);
			this.lblFormula2.DataBindings.Add("Text", ViewModel, "Formula2Caption", true, DataSourceUpdateMode.OnPropertyChanged);
			this.edtFormula1.DataBindings.Add("EditValue", ViewModel, "Formula1", true, DataSourceUpdateMode.OnPropertyChanged);
			this.edtFormula2.DataBindings.Add("EditValue", ViewModel, "Formula2", true, DataSourceUpdateMode.OnPropertyChanged);
			SetSettingsTabVisibility();
		}
		void SetInputMessageTabBindings() {
			this.chkShowMessage.DataBindings.Add("Checked", ViewModel, "ShowMessage", true, DataSourceUpdateMode.OnPropertyChanged);
			this.lblMessageTitle.DataBindings.Add("Enabled", ViewModel, "ShowMessage", true, DataSourceUpdateMode.OnPropertyChanged);
			this.lblMessage.DataBindings.Add("Enabled", ViewModel, "ShowMessage", true, DataSourceUpdateMode.OnPropertyChanged);
			this.edtMessageTitle.DataBindings.Add("EditValue", ViewModel, "MessageTitle", true, DataSourceUpdateMode.OnPropertyChanged);
			this.edtMessageTitle.DataBindings.Add("Enabled", ViewModel, "ShowMessage", true, DataSourceUpdateMode.OnPropertyChanged);
			this.edtMessage.DataBindings.Add("EditValue", ViewModel, "Message", true, DataSourceUpdateMode.OnPropertyChanged);
			this.edtMessage.DataBindings.Add("Enabled", ViewModel, "ShowMessage", true, DataSourceUpdateMode.OnPropertyChanged);
		}
		void SetErrorAlertTabBindings() {
			this.chkShowError.DataBindings.Add("Checked", ViewModel, "ShowErrorMessage", true, DataSourceUpdateMode.OnPropertyChanged);
			this.lblErrorTitle.DataBindings.Add("Enabled", ViewModel, "ShowErrorMessage", true, DataSourceUpdateMode.OnPropertyChanged);
			this.lblErrorMessage.DataBindings.Add("Enabled", ViewModel, "ShowErrorMessage", true, DataSourceUpdateMode.OnPropertyChanged);
			this.lblErrorStyle.DataBindings.Add("Enabled", ViewModel, "ShowErrorMessage", true, DataSourceUpdateMode.OnPropertyChanged);
			this.edtErrorTitle.DataBindings.Add("EditValue", ViewModel, "ErrorTitle", true, DataSourceUpdateMode.OnPropertyChanged);
			this.edtErrorTitle.DataBindings.Add("Enabled", ViewModel, "ShowErrorMessage", true, DataSourceUpdateMode.OnPropertyChanged);
			this.edtErrorMessage.DataBindings.Add("EditValue", ViewModel, "ErrorMessage", true, DataSourceUpdateMode.OnPropertyChanged);
			this.edtErrorMessage.DataBindings.Add("Enabled", ViewModel, "ShowErrorMessage", true, DataSourceUpdateMode.OnPropertyChanged);
			this.edtErrorStyle.Properties.DataSource = ViewModel.ErrorStyleDataSource;
			this.edtErrorStyle.DataBindings.Add("EditValue", ViewModel, "ErrorStyleValue", true, DataSourceUpdateMode.OnPropertyChanged);
			this.edtErrorStyle.DataBindings.Add("Enabled", ViewModel, "ShowErrorMessage", true, DataSourceUpdateMode.OnPropertyChanged);
		}
		void SetSettingsTabVisibility() {
			this.chkDropdown.DataBindings.Add("Visible", ViewModel, "InCellDropDownVisible", true, DataSourceUpdateMode.OnPropertyChanged);
			this.lblFormula1.DataBindings.Add("Visible", ViewModel, "Formula1Visible", true, DataSourceUpdateMode.OnPropertyChanged);
			this.lblFormula2.DataBindings.Add("Visible", ViewModel, "Formula2Visible", true, DataSourceUpdateMode.OnPropertyChanged);
			this.edtFormula1.DataBindings.Add("Visible", ViewModel, "Formula1Visible", true, DataSourceUpdateMode.OnPropertyChanged);
			this.edtFormula2.DataBindings.Add("Visible", ViewModel, "Formula2Visible", true, DataSourceUpdateMode.OnPropertyChanged);
		}
		void SetErrorAlertTabVisibility() {
			this.iconInfo.DataBindings.Add("Visible", ViewModel, "InfoIconVisible", true, DataSourceUpdateMode.OnPropertyChanged);
			this.iconStop.DataBindings.Add("Visible", ViewModel, "StopIconVisible", true, DataSourceUpdateMode.OnPropertyChanged);
			this.iconWarning.DataBindings.Add("Visible", ViewModel, "WarningIconVisible", true, DataSourceUpdateMode.OnPropertyChanged);
		}
		#endregion
		protected override int ForEachControlResursiveCore(Control control, Action<Control> action) {
			int count = base.ForEachControlResursiveCore(control, action);
			XtraTabControl tabControl = control as XtraTabControl;
			if (tabControl != null)
				count += ForEachControlResursive(tabControl, action);
			return count;
		}
		protected override void Expand(object sender) {
			UnsubscribePageChangingEvents();
			try {
				base.Expand(sender);
				xtraTabControl.SelectedTabPageIndex = 0;
				viewModel.Refresh();
			}
			finally {
				SubscribePageChangingEvents();
			}
		}
		protected override void Collapse(object sender) {
			base.Collapse(sender);
			bool dataValidationSheetActive = Object.ReferenceEquals(DocumentModel.ActiveSheet, viewModel.Worksheet);
			edtFormula1.IncludeSheetName = !dataValidationSheetActive;
			edtFormula2.IncludeSheetName = !dataValidationSheetActive;
		}
		void SubscribePageChangingEvents() {
			xtraTabControl.SelectedPageChanged += new TabPageChangedEventHandler(xtraTabControl_SelectedPageChanged);
			xtraTabControl.SelectedPageChanging += new TabPageChangingEventHandler(xtraTabControl_SelectedPageChanging);
		}
		void UnsubscribePageChangingEvents() {
			xtraTabControl.SelectedPageChanged -= new TabPageChangedEventHandler(xtraTabControl_SelectedPageChanged);
			xtraTabControl.SelectedPageChanging -= new TabPageChangingEventHandler(xtraTabControl_SelectedPageChanging);
		}
		void btnOk_Click(object sender, EventArgs e) {
			TopMost = false;
			try {
				if (ViewModel.Validate()) {
					this.DialogResult = DialogResult.OK;
					Close();
				}
			}
			finally {
				TopMost = true;
			}
		}
		void btnCancel_Click(object sender, EventArgs e) {
			this.DialogResult = DialogResult.Cancel;
			Close();
		}
		void chkApplyToAllCells_CheckedChanged(object sender, EventArgs e) {
			viewModel.SetSelection(((CheckEdit)sender).Checked);
		}
		void btnClearAll_Click(object sender, EventArgs e) {
			viewModel.ResetToDefaults();
		}
		void xtraTabControl_SelectedPageChanging(object sender, TabPageChangingEventArgs e) {
			ClearVisibilityBindings();
		}
		void xtraTabControl_SelectedPageChanged(object sender, TabPageChangedEventArgs e) {
			SetVisibilityBindings();
		}
	}
}
