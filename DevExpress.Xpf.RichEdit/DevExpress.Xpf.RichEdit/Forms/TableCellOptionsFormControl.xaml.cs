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
using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.RichEdit;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Xpf.Editors;
using DevExpress.Office;
namespace DevExpress.XtraRichEdit.Forms {
	#region TableCellOptionsForm
	public partial class TableCellOptionsFormControl : UserControl, IDialogContent {
		#region Fields
		readonly RichEditControl control;
		TableCellOptionsFormController controller;
		#endregion
		public TableCellOptionsFormControl() {
			InitializeComponent();
		}
		public TableCellOptionsFormControl(List<TableCell> tableCells, RichEditControl control) {
			Guard.ArgumentNotNull(tableCells, "tableCells");
			Guard.ArgumentPositive(tableCells.Count, "tableCellsCount");
			Guard.ArgumentNotNull(control, "control");
			this.control = control;
			this.controller = CreateController(tableCells);
			InitializeComponent();
			Loaded += OnLoaded;
			SubscribeControlsEvents();
		}
		#region Properties
		public RichEditControl Control { get { return control; } }
		public TableCellOptionsFormController Controller { get { return controller; } }
		#endregion
		protected internal virtual TableCellOptionsFormController CreateController(List<TableCell> tableCells) {
			return new TableCellOptionsFormController(tableCells);
		}
		void InitializeForm() {
			DocumentUnit UIUnit = Control.InnerControl.UIUnit;
			this.spnTopMargin.DefaultUnitType = UIUnit;
			this.spnRightMargin.DefaultUnitType = UIUnit;
			this.spnBottomMargin.DefaultUnitType = UIUnit;
			this.spnLeftMargin.DefaultUnitType = UIUnit;
			DocumentModelUnitConverter unitConverter = Control.DocumentModel.UnitConverter;
			this.spnTopMargin.ValueUnitConverter = unitConverter;
			this.spnRightMargin.ValueUnitConverter = unitConverter;
			this.spnBottomMargin.ValueUnitConverter = unitConverter;
			this.spnLeftMargin.ValueUnitConverter = unitConverter;
			this.spnTopMargin.MaxValue = TableCellOptionsFormDefaults.MaxTopMarginByDefault;
			this.spnTopMargin.MinValue = TableCellOptionsFormDefaults.MinTopMarginByDefault;
			this.spnRightMargin.MaxValue = TableCellOptionsFormDefaults.MaxRightMarginByDefault;
			this.spnRightMargin.MinValue = TableCellOptionsFormDefaults.MinRightMarginByDefault;
			this.spnBottomMargin.MaxValue = TableCellOptionsFormDefaults.MaxBottomMarginByDefault;
			this.spnBottomMargin.MinValue = TableCellOptionsFormDefaults.MinBottomMarginByDefault;
			this.spnLeftMargin.MaxValue = TableCellOptionsFormDefaults.MaxLeftMarginByDefault;
			this.spnLeftMargin.MinValue = TableCellOptionsFormDefaults.MinLeftMarginByDefault;
		}
		protected internal virtual void SubscribeControlsEvents() {
			spnTopMargin.ValueChanged += OnSpnTopMarginValueChanged;
			spnRightMargin.ValueChanged += OnSpnRightMarginValueChanged;
			spnBottomMargin.ValueChanged += OnSpnBottomMarginValueChanged;
			spnLeftMargin.ValueChanged += OnSpnLeftMarginValueChanged;
			chkSameAsWholeTable.EditValueChanged += OnSameAsWholeTableChanged;
			chkWrapText.EditValueChanged += OnkWrapTextChanged;
			chkFitText.EditValueChanged += OnFitTextChanged;
		}
		protected internal virtual void UnsubscribeControlsEvents() {
			spnTopMargin.ValueChanged -= OnSpnTopMarginValueChanged;
			spnRightMargin.ValueChanged -= OnSpnRightMarginValueChanged;
			spnBottomMargin.ValueChanged -= OnSpnBottomMarginValueChanged;
			spnLeftMargin.ValueChanged -= OnSpnLeftMarginValueChanged;
			chkSameAsWholeTable.EditValueChanged -= OnSameAsWholeTableChanged;
			chkWrapText.EditValueChanged -= OnkWrapTextChanged;
			chkFitText.EditValueChanged -= OnFitTextChanged;
		}
		void OnLoaded(object sender, RoutedEventArgs e) {
			InitializeForm();
			UpdateForm();
		}
		protected internal virtual void UpdateForm() {
			UnsubscribeControlsEvents();
			try {
				UpdateFormCore();
			} finally {
				SubscribeControlsEvents();
			}
		}
		protected internal virtual void UpdateFormCore() {
			spnTopMargin.Value = Controller.TopMargin;
			spnRightMargin.Value = Controller.RightMargin;
			spnBottomMargin.Value = Controller.BottomMargin;
			spnLeftMargin.Value = Controller.LeftMargin;
			bool cellMarginsSameAsTable = Controller.CellMarginsSameAsTable;
			chkSameAsWholeTable.IsChecked = cellMarginsSameAsTable;
			SetControlsEnabled(!cellMarginsSameAsTable);
			UpdateCheckEdit(chkWrapText, !Controller.NoWrap);
			UpdateCheckEdit(chkFitText, Controller.FitText);
		}
		protected internal virtual void UpdateCheckEdit(CheckEdit checkEdit, bool? check) {
			if (check == null)
				checkEdit.IsThreeState = true;
			checkEdit.IsChecked = check;
		}
		void OnSpnTopMarginValueChanged(object sender, EventArgs e) {
			Controller.TopMargin = spnTopMargin.Value;
		}
		void OnSpnRightMarginValueChanged(object sender, EventArgs e) {
			Controller.RightMargin = spnRightMargin.Value;
		}
		void OnSpnBottomMarginValueChanged(object sender, EventArgs e) {
			Controller.BottomMargin = spnBottomMargin.Value;
		}
		void OnSpnLeftMarginValueChanged(object sender, EventArgs e) {
			Controller.LeftMargin = spnLeftMargin.Value;
		}
		void OnSameAsWholeTableChanged(object sender, EventArgs e) {
			bool check = chkSameAsWholeTable.IsChecked.Value;
			Controller.CellMarginsSameAsTable = check;
			SetControlsEnabled(!check);
		}
		void SetControlsEnabled(bool enabled) {
			spnTopMargin.IsEnabled = enabled;
			spnRightMargin.IsEnabled = enabled;
			spnBottomMargin.IsEnabled = enabled;
			spnLeftMargin.IsEnabled = enabled;
		}
		void OnkWrapTextChanged(object sender, EventArgs e) {
			Controller.NoWrap = !chkWrapText.IsChecked;
		}
		void OnFitTextChanged(object sender, EventArgs e) {
			Controller.FitText = chkFitText.IsChecked;
		}
		protected internal virtual void ApplyChanges() {
			if (Controller != null)
				Controller.ApplyChanges();
		}
		#region IDialogContent Members
		bool IDialogContent.CanCloseWithOKResult() {
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
	#endregion
}
