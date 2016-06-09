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
using System.Windows.Controls;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.Xpf.Core;
using System.Windows;
using DevExpress.Xpf.Editors;
using System.Collections.Generic;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.Xpf.RichEdit.Localization;
using DevExpress.Xpf.RichEdit;
namespace DevExpress.XtraRichEdit.Forms {
	public partial class SplitTableCellsFormControl : UserControl, IDialogContent {
		#region Fields
		readonly SplitTableCellsFormController controller;
		#endregion
		public SplitTableCellsFormControl() {
			InitializeComponent();
		}
		public SplitTableCellsFormControl(SplitTableCellsFormControllerParameters controllerParameters) {
			Guard.ArgumentNotNull(controllerParameters, "controllerParameters");
			this.controller = CreateController(controllerParameters);
			Loaded += OnLoaded;
			InitializeComponent();
			SubscribeControlsEvents();
		}
		#region Properties
		public SplitTableCellsFormController Controller { get { return controller; } }
		internal int SourceRowsCount { get { return Controller.SourceParameters.RowsCount; } }
		#endregion
		protected internal virtual SplitTableCellsFormController CreateController(SplitTableCellsFormControllerParameters controllerParameters) {
			return new SplitTableCellsFormController(controllerParameters);
		}
		void OnLoaded(object sender, RoutedEventArgs e) {
			UpdateForm();
		}
		protected internal virtual void SubscribeControlsEvents() {
			this.spnNumberOfColumns.EditValueChanged += OnSpnNumberOfColumnsEditValueChanged;
			this.spnNumberOfRows.EditValueChanged += OnSpnNumberOfRowsEditValueChanged;
			this.chkMergeBeforeSplit.Checked += OnChkMergeBeforeSplitCheckedChanged;
			this.chkMergeBeforeSplit.Unchecked += OnChkMergeBeforeSplitCheckedChanged;
			this.spnNumberOfRows.Spin += OnSpnNumberOfRowsSpin;
		}
		protected internal virtual void UnsubscribeControlsEvents() {
			this.spnNumberOfColumns.EditValueChanged -= OnSpnNumberOfColumnsEditValueChanged;
			this.spnNumberOfRows.EditValueChanged -= OnSpnNumberOfRowsEditValueChanged;
			this.chkMergeBeforeSplit.Checked -= OnChkMergeBeforeSplitCheckedChanged;
			this.chkMergeBeforeSplit.Unchecked -= OnChkMergeBeforeSplitCheckedChanged;
			this.spnNumberOfRows.Spin -= OnSpnNumberOfRowsSpin;
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
		protected virtual void UpdateFormCore() {
			if (Controller == null)
				return;
			int rowsCountAfterMerge = Controller.RowsCountAfterMerge;
			this.spnNumberOfRows.MaxValue = rowsCountAfterMerge > 1 ? rowsCountAfterMerge : Controller.MaxRowsCount;
			if (Controller.SourceParameters.IsSelectedCellsSquare) {
				this.spnNumberOfRows.Value = SourceRowsCount;
			}
			else {
				Controller.RowsCount = 1;
				this.spnNumberOfRows.Value = 1;
				this.spnNumberOfRows.IsEnabled = false;
			}
			this.spnNumberOfColumns.Value = Controller.ColumnsCount;
			this.spnNumberOfColumns.MaxValue = Controller.MaxColumnsCount;
			this.chkMergeBeforeSplit.IsChecked = Controller.MergeCellsBeforeSplit;
			this.chkMergeBeforeSplit.IsEnabled = Controller.MergeCellsBeforeSplit;
		}
		private void OnSpnNumberOfColumnsEditValueChanged(object sender, EventArgs e) {
			Controller.ColumnsCount = (int)spnNumberOfColumns.Value;
		}
		private void OnSpnNumberOfRowsEditValueChanged(object sender, EventArgs e) {
			Controller.RowsCount = (int)spnNumberOfRows.Value;
		}
		private void OnChkMergeBeforeSplitCheckedChanged(object sender, EventArgs e) {
			bool check = chkMergeBeforeSplit.IsChecked.Value;
			if (check) {
				spnNumberOfRows.IsEnabled = true;
				spnNumberOfColumns.Value = Controller.SourceParameters.ColumnsCount;
				spnNumberOfRows.Value = SourceRowsCount;
			}
			else {
				if (SourceRowsCount > 1) {
					spnNumberOfRows.IsEnabled = false;
				}
				spnNumberOfColumns.Value = 2;
				spnNumberOfRows.Value = 1;
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
					spnNumberOfRows.Value = allowedValues[currentValueIndex + 1];
			}
			else {
				if (currentValue > 1)
					spnNumberOfRows.Value = allowedValues[currentValueIndex - 1];
			}
			e.Handled = true;
		}
		protected internal virtual void ApplyChanges() {
			if (Controller != null)
				Controller.ApplyChanges();
		}
		protected internal void ShowDialog() {
			string errorTextFormat = XtraRichEditLocalizer.GetString(XtraRichEditStringId.Msg_InvalidDivisor);
			string errorMessage = String.Format(errorTextFormat, Controller.RowsCountAfterMerge);
#if SL
			DXDialog dialog = new DXDialog();
			dialog.Title = XpfRichEditLocalizer.GetString(RichEditControlStringId.Msg_Warning);
			dialog.Buttons = DialogButtons.Ok;
			dialog.Content = errorMessage;
			dialog.IsSizable = false;
			dialog.Padding = new Thickness(20);
			dialog.ShowDialog();
#else
			DXMessageBox.Show(errorMessage, System.Windows.Forms.Application.ProductName, MessageBoxButton.OK, MessageBoxImage.Warning);
#endif
		}
		#region IDialogContent Members
		bool IDialogContent.CanCloseWithOKResult() {
			int rowsCountAfterMerge = Controller.RowsCountAfterMerge;
			if (rowsCountAfterMerge > 1 && rowsCountAfterMerge % spnNumberOfRows.Value != 0) {
				ShowDialog();
				return false;
			}
			return true;
		}
		void IDialogContent.OnApply() {
			ApplyChanges();
		}
		void IDialogContent.OnOk() {
			ApplyChanges();
		}
		#endregion
	}
}
