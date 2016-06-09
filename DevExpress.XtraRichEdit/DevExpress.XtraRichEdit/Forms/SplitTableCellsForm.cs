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
using DevExpress.XtraEditors;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.Utils;
using System.Windows.Forms;
using System.Collections.Generic;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraEditors.Repository;
#region FxCop suppressions
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.SplitTableCellsForm.lblNumberOfColumns")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.SplitTableCellsForm.lblNumberOfRows")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.SplitTableCellsForm.spnNumberOfColumns")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.SplitTableCellsForm.spnNumberOfRows")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.SplitTableCellsForm.chkMergeBeforeSplit")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.SplitTableCellsForm.btnCancel")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.SplitTableCellsForm.btnOk")]
#endregion
namespace DevExpress.XtraRichEdit.Forms {
	#region SplitTableCellsForm
	public partial class SplitTableCellsForm : XtraForm {
		#region Fields
		SplitTableCellsFormController controller;
		DXValidationProvider validationProviderBetween;
		DXValidationProvider validationProviderEquals;
		bool isRowsCountValid;
		bool isColumnsCountValid;
		#endregion
		SplitTableCellsForm() {
			InitializeComponent();
		}
		public SplitTableCellsForm(SplitTableCellsFormControllerParameters controllerParameters) {
			Guard.ArgumentNotNull(controllerParameters, "controllerParameters");
			InitializeComponent();
			this.controller = CreateController(controllerParameters);
			this.isRowsCountValid = true;
			this.isColumnsCountValid = true;
			InitializeValidationProviders();
			UpdateForm();
		}
		#region Properties
		protected internal SplitTableCellsFormController Controller { get { return controller; } }
		internal int SourceRowsCount { get { return Controller.SourceParameters.RowsCount; } }
		#endregion
		protected internal virtual SplitTableCellsFormController CreateController(SplitTableCellsFormControllerParameters controllerParameters) {
			return new SplitTableCellsFormController(controllerParameters);
		}
		protected internal void InitializeValidationProviders() {
			validationProviderBetween = new DXValidationProvider();
			ConditionValidationRule rowsBetweenValidationRule = new ConditionValidationRule();
			ConditionValidationRule columnsBetweenValidationRule = new ConditionValidationRule();
			RepositoryItemSpinEdit spnRowsProperties = spnNumberOfRows.Properties;
			InitializeValidationRule(rowsBetweenValidationRule, spnRowsProperties.MinValue, Controller.MaxRowsCount);
			RepositoryItemSpinEdit spnColumnsProperties = spnNumberOfColumns.Properties;
			InitializeValidationRule(columnsBetweenValidationRule, spnColumnsProperties.MinValue, Controller.MaxColumnsCount);
			validationProviderBetween.SetValidationRule(spnNumberOfRows, rowsBetweenValidationRule);
			validationProviderBetween.SetValidationRule(spnNumberOfColumns, columnsBetweenValidationRule);
			validationProviderEquals = new DXValidationProvider();
			ConditionValidationRule rowsEqualsValidationRule = new ConditionValidationRule();
			rowsEqualsValidationRule.ConditionOperator = ConditionOperator.Equals;
			rowsEqualsValidationRule.Value1 = 0;
			string errorTextFormat = XtraRichEditLocalizer.GetString(XtraRichEditStringId.Msg_InvalidDivisor);
			rowsEqualsValidationRule.ErrorText = String.Format(errorTextFormat, Controller.RowsCountAfterMerge);
			validationProviderEquals.SetValidationRule(spnNumberOfRows, rowsEqualsValidationRule);
		}
		protected internal virtual void InitializeValidationRule(ConditionValidationRule validationRule, decimal minValue, decimal maxValue) {
			validationRule.Value1 = minValue;
			validationRule.Value2 = maxValue;
			validationRule.ConditionOperator = ConditionOperator.Between;
			string errorTextFormat = XtraRichEditLocalizer.GetString(XtraRichEditStringId.Msg_InvalidFontSize);
			validationRule.ErrorText = String.Format(errorTextFormat, minValue, maxValue);
		}
		protected internal virtual void UpdateForm() {
			UnsubscribeControlsEvents();
			try {
				UpdateFormCore();
			}
			finally {
				SubscribeControlsEvents();
			}
		}
		protected internal virtual void UnsubscribeControlsEvents() {
			this.chkMergeBeforeSplit.CheckedChanged -= OnChkMergeBeforeSplitCheckedChanged;
			this.spnNumberOfRows.TextChanged -= OnSpnNumberOfRowsTextChanged;
			this.spnNumberOfColumns.TextChanged -= OnSpnNumberOfColumnsTextChanged;
		}
		protected internal virtual void SubscribeControlsEvents() {
			this.chkMergeBeforeSplit.CheckedChanged += OnChkMergeBeforeSplitCheckedChanged;
			this.spnNumberOfRows.TextChanged += OnSpnNumberOfRowsTextChanged;
			this.spnNumberOfColumns.TextChanged += OnSpnNumberOfColumnsTextChanged;
		}
		protected internal virtual void UpdateFormCore() {
			int rowsCountAfterMerge = Controller.RowsCountAfterMerge;
			this.spnNumberOfRows.Properties.MaxValue = rowsCountAfterMerge > 1 ? rowsCountAfterMerge : Controller.MaxRowsCount;
			if (Controller.SourceParameters.IsSelectedCellsSquare) {
				this.spnNumberOfRows.EditValue = SourceRowsCount;
				ValidateRowsCount();
			}
			else {
				Controller.RowsCount = 1;
				this.spnNumberOfRows.EditValue = 1;
				this.spnNumberOfRows.Enabled = false;
			}
			this.spnNumberOfColumns.EditValue = Controller.ColumnsCount;
			this.spnNumberOfColumns.Properties.MaxValue = Controller.MaxColumnsCount;
			bool isMergeCellsBeforeSplit = Controller.MergeCellsBeforeSplit;
			this.chkMergeBeforeSplit.Checked = isMergeCellsBeforeSplit;
			this.chkMergeBeforeSplit.Enabled = isMergeCellsBeforeSplit;
		}
		private void OnChkMergeBeforeSplitCheckedChanged(object sender, EventArgs e) {
			bool check = chkMergeBeforeSplit.Checked;
			if (check) {
				spnNumberOfRows.Enabled = true;
				lblNumberOfRows.Enabled = true;
				spnNumberOfColumns.EditValue = Controller.SourceParameters.ColumnsCount;
				spnNumberOfRows.EditValue = SourceRowsCount;
			}
			else {
				if (SourceRowsCount > 1) {
					spnNumberOfRows.Enabled = false;
					lblNumberOfRows.Enabled = false;
				}
				spnNumberOfColumns.EditValue = 2;
				spnNumberOfRows.EditValue = 1;
			}
			Controller.MergeCellsBeforeSplit = check;
		}
		private void OnSpnNumberOfRowsSpin(object sender, SpinEventArgs e) {
			int currentValue = (int)spnNumberOfRows.Value;
			List<int> allowedValues = Controller.AllowedRowsCount;
			if (allowedValues == null || currentValue < 1 || currentValue > Controller.RowsCountAfterMerge)
				return;
			int currentValueIndex = allowedValues.IndexOf(currentValue);
			if (currentValueIndex == -1) {
				spnNumberOfRows.Value = allowedValues[0];
				e.Handled = true;
				return;
			}
			if (e.IsSpinUp) {
				if (currentValue < Controller.RowsCountAfterMerge)
					spnNumberOfRows.EditValue = allowedValues[currentValueIndex + 1];
			}
			else {
				if (currentValue > 1)
					spnNumberOfRows.EditValue = allowedValues[currentValueIndex - 1];
			}
			e.Handled = true;
		}
		private void OnSpnNumberOfRowsTextChanged(object sender, EventArgs e) {
			isRowsCountValid = validationProviderBetween.Validate(spnNumberOfRows);
			Controller.RowsCount = (int)spnNumberOfRows.Value;
			if(!isRowsCountValid || Controller.AllowedRowsCount == null)
				return;
			UnsubscribeControlsEvents();
			ValidateRowsCount();
			SubscribeControlsEvents();
		}
		protected internal virtual void ValidateRowsCount() {
			decimal oldValue = spnNumberOfRows.Value;
			int currentValue = (int)spnNumberOfRows.Value;
			spnNumberOfRows.Value = Controller.RowsCountAfterMerge % currentValue;
			isRowsCountValid = validationProviderEquals.Validate(spnNumberOfRows);
			List<int> allowedValues = Controller.AllowedRowsCount;
			if (allowedValues != null && !allowedValues.Contains(currentValue))
				isRowsCountValid = false;
			spnNumberOfRows.Value = oldValue;
		}
		private void OnSpnNumberOfColumnsTextChanged(object sender, EventArgs e) {
			isColumnsCountValid = validationProviderBetween.Validate(spnNumberOfColumns);
			Controller.ColumnsCount = (int)spnNumberOfColumns.Value;
		}
		void OnBtnOkClick(object sender, EventArgs e) {
			if (isRowsCountValid && isColumnsCountValid) {
				Controller.ApplyChanges();
				this.DialogResult = DialogResult.OK;
			}
		}
	}
	#endregion
}
