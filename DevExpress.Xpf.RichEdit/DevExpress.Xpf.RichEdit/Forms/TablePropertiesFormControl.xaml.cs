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
using DevExpress.Xpf.Core;
using DevExpress.Xpf.RichEdit;
using DevExpress.XtraRichEdit.Tables.Native;
using DevExpress.Utils;
using System.Windows;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Xpf.Editors;
using DevExpress.XtraRichEdit.Design.Internal;
using DevExpress.Office;
namespace DevExpress.XtraRichEdit.Forms {
	public partial class TablePropertiesFormControl : UserControl, IDialogContent {
		#region Fields
		readonly IRichEditControl control;
		readonly TablePropertiesFormController controller;
		#endregion
		public TablePropertiesFormControl() {
			InitializeComponent();
		}
		public TablePropertiesFormControl(TablePropertiesFormControllerParameters controllerParameters) {
			Guard.ArgumentNotNull(controllerParameters, "controllerParameters");
			this.control = controllerParameters.Control;
			this.controller = CreateController(controllerParameters);
			InitializeComponent();
			Loaded += OnLoaded;
			InitializeForm();
			SubscribeControlsEvents();
		}
		#region Properties
		public IRichEditControl Control { get { return control; } }
		public TablePropertiesFormController Controller { get { return controller; } }
		#endregion
		protected internal virtual TablePropertiesFormController CreateController(TablePropertiesFormControllerParameters controllerParameters) {
			return new TablePropertiesFormController(controllerParameters);
		}
		void OnLoaded(object sender, RoutedEventArgs e) {
			UpdateForm();
		}
		void InitializeForm() {
			DocumentUnit uIUnit = Control.InnerControl.UIUnit;
			DocumentModelUnitConverter unitConverter = Control.InnerControl.DocumentModel.UnitConverter;
			tableSizeControl.BeginUpdate();
			try {
				TableSizeProperties properties = tableSizeControl.Properties;
				properties.UnitType = uIUnit;
				properties.ValueUnitConverter = unitConverter;
				properties.MinValue = TablePropertiesFormDefaults.MinTableWidthByDefault;
			}
			finally {
				tableSizeControl.EndUpdate();
			}
			tableRowHeightControl.BeginUpdate();
			try {
				TableRowHeightProperties properties = tableRowHeightControl.Properties;
				properties.UnitType = uIUnit;
				properties.ValueUnitConverter = unitConverter;
				properties.MinValue = TablePropertiesFormDefaults.MinRowHeightByDefault;
				properties.MaxValue = TablePropertiesFormDefaults.MaxRowHeightByDefault;
			}
			finally {
				tableRowHeightControl.EndUpdate();
			}
			columnSizeControl.BeginUpdate();
			try {
				TableSizeProperties properties = this.columnSizeControl.Properties;
				properties.UnitType = uIUnit;
				properties.ValueUnitConverter = unitConverter;
				properties.MinValue = TablePropertiesFormDefaults.MinColumnWidthByDefault;
			}
			finally {
				columnSizeControl.EndUpdate();
			}
			cellSizeControl.BeginUpdate();
			try {
				TableSizeProperties properties = this.cellSizeControl.Properties;
				properties.UnitType = uIUnit;
				properties.ValueUnitConverter = unitConverter;
				properties.MinValue = TablePropertiesFormDefaults.MinCellWidthByDefault;
			}
			finally {
				cellSizeControl.EndUpdate();
			}
			this.spnIndentFromLeft.DefaultUnitType = uIUnit;
			this.spnIndentFromLeft.ValueUnitConverter = unitConverter;
			this.spnIndentFromLeft.MinValue = TablePropertiesFormDefaults.MinTableIndentByDefault;
			this.spnIndentFromLeft.MaxValue = TablePropertiesFormDefaults.MaxTableIndentByDefault;
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
		protected internal virtual void UpdateFormCore() {
			UpdateTableTab();
			UpdateRowTab();
			UpdateColumnTab();
			UpdateCellTab();
		}
		void UpdateTableTab() {
			tableSizeControl.BeginUpdate();
			try {
				TableSizeProperties properties = tableSizeControl.Properties;
				properties.UseDefaultValue = Controller.UseDefaultTableWidth;
				properties.Width = Controller.TableWidth;
				properties.WidthType = Controller.TableWidthUnitType;
				properties.ValueForPercent = Controller.PageFirstColumnWidth;
				properties.MaxValue = Controller.GetTableWidthMaxValueConsiderWidthUnitType();
			}
			finally {
				tableSizeControl.EndUpdate();
			}
			UpdateEnabledControls();
			this.spnIndentFromLeft.Value = Controller.TableIndent;
			UpdateTableAlignmentGroup(Controller.TableAlignment);
		}
		void UpdateEnabledControls() {
			TableRowAlignment? tableAlignment = Controller.TableAlignment;
			this.spnIndentFromLeft.IsEnabled = tableAlignment.HasValue && tableAlignment.Value == TableRowAlignment.Left;
		}
		void UpdateTableAlignmentGroup(TableRowAlignment? alignment) {
			bool hasValue = alignment.HasValue;
			rbTableAlignmentLeft.IsChecked = hasValue && alignment == TableRowAlignment.Left;
			rbTableAlignmentCenter.IsChecked = hasValue && alignment == TableRowAlignment.Center;
			rbTableAlignmentRight.IsChecked = hasValue && alignment == TableRowAlignment.Right;
		}
		void UpdateRowTab() {
			tableRowHeightControl.BeginUpdate();
			try {
				TableRowHeightProperties properties = tableRowHeightControl.Properties;
				properties.UseDefaultValue = Controller.UseDefaultRowHeight;
				properties.Height = Controller.RowHeight;
				properties.HeightType = Controller.RowHeightType;
			}
			finally {
				tableRowHeightControl.EndUpdate();
			}
			UpdateCheckEdit(chkCantSplit, !Controller.RowCantSplit);
			UpdateCheckEdit(chkHeader, Controller.RowHeader);
			chkHeader.IsEnabled = Controller.IsSelectedFirstRowInTable();
		}
		void UpdateCheckEdit(CheckEdit checkEdit, bool? check) {
			if (check == null)
				checkEdit.IsThreeState = true;
			checkEdit.IsChecked = check;
		}
		void UpdateColumnTab() {
			columnSizeControl.BeginUpdate();
			try {
				TableSizeProperties properties = columnSizeControl.Properties;
				properties.UseDefaultValue = Controller.UseDefaultColumnWidth;
				properties.Width = Controller.ColumnWidth;
				properties.WidthType = Controller.ColumnWidthUnitType;
				properties.ValueForPercent = Controller.PageFirstColumnWidth;
				properties.MaxValue = Controller.GetColumnWidthMaxValueConsiderWidthUnitType();
			}
			finally {
				columnSizeControl.EndUpdate();
			}
		}
		void UpdateCellTab() {
			cellSizeControl.BeginUpdate();
			try {
				TableSizeProperties properties = cellSizeControl.Properties;
				properties.UseDefaultValue = Controller.UseDefaultCellWidth;
				properties.Width = Controller.CellWidth;
				properties.WidthType = Controller.CellWidthUnitType;
				properties.ValueForPercent = Controller.PageFirstColumnWidth;
				properties.MaxValue = Controller.GetCellWidthMaxValueConsiderWidthUnitType();
			}
			finally {
				cellSizeControl.EndUpdate();
			}
			UpdateCellVerticalAlignmentGroup(Controller.CellVerticalAlignment);
		}
		void UpdateCellVerticalAlignmentGroup(Model.VerticalAlignment? alignment) {
			bool hasValue = alignment.HasValue;
			rbCellVerticalAlignmentTop.IsChecked = hasValue && alignment == Model.VerticalAlignment.Top;
			rbCellVerticalAlignmentCenter.IsChecked = hasValue && alignment == Model.VerticalAlignment.Center;
			rbCellVerticalAlignmentBottom.IsChecked = hasValue && alignment == Model.VerticalAlignment.Bottom;
		}
		protected internal virtual void SubscribeControlsEvents() {
			tableSizeControl.TableSizeControlChanged += OnTableSizeControlChanged;
			spnIndentFromLeft.ValueChanged += OnSpnIndentFromLeftValueChanged;
			rbTableAlignmentLeft.Checked += OnTableAlignmentChanged;
			rbTableAlignmentLeft.Unchecked += OnTableAlignmentChanged;
			rbTableAlignmentCenter.Checked += OnTableAlignmentChanged;
			rbTableAlignmentCenter.Unchecked += OnTableAlignmentChanged;
			rbTableAlignmentRight.Checked += OnTableAlignmentChanged;
			rbTableAlignmentRight.Unchecked += OnTableAlignmentChanged;
			tableRowHeightControl.TableRowHeightControlChanged += OnTableRowHeightControlChanged;
			chkCantSplit.EditValueChanged += OnChkCantSplitEditValueChanged;
			chkHeader.EditValueChanged += OnChkHeaderEditValueChanged;
			columnSizeControl.TableSizeControlChanged += OnColumnSizeControlChanged;
			cellSizeControl.TableSizeControlChanged += OnCellSizeControlChanged;
			rbCellVerticalAlignmentTop.Checked += OnCellVerticalAlignmentChanged;
			rbCellVerticalAlignmentTop.Unchecked += OnCellVerticalAlignmentChanged;
			rbCellVerticalAlignmentCenter.Checked += OnCellVerticalAlignmentChanged;
			rbCellVerticalAlignmentCenter.Unchecked += OnCellVerticalAlignmentChanged;
			rbCellVerticalAlignmentBottom.Checked += OnCellVerticalAlignmentChanged;
			rbCellVerticalAlignmentBottom.Unchecked += OnCellVerticalAlignmentChanged;
		}
		protected internal virtual void UnsubscribeControlsEvents() {
			tableSizeControl.TableSizeControlChanged -= OnTableSizeControlChanged;
			spnIndentFromLeft.ValueChanged -= OnSpnIndentFromLeftValueChanged;
			rbTableAlignmentLeft.Checked -= OnTableAlignmentChanged;
			rbTableAlignmentLeft.Unchecked -= OnTableAlignmentChanged;
			rbTableAlignmentCenter.Checked -= OnTableAlignmentChanged;
			rbTableAlignmentCenter.Unchecked -= OnTableAlignmentChanged;
			rbTableAlignmentRight.Checked -= OnTableAlignmentChanged;
			rbTableAlignmentRight.Unchecked -= OnTableAlignmentChanged;
			tableRowHeightControl.TableRowHeightControlChanged -= OnTableRowHeightControlChanged;
			chkCantSplit.EditValueChanged -= OnChkCantSplitEditValueChanged;
			chkHeader.EditValueChanged -= OnChkHeaderEditValueChanged;
			columnSizeControl.TableSizeControlChanged -= OnColumnSizeControlChanged;
			cellSizeControl.TableSizeControlChanged -= OnCellSizeControlChanged;
			rbCellVerticalAlignmentTop.Checked -= OnCellVerticalAlignmentChanged;
			rbCellVerticalAlignmentTop.Unchecked -= OnCellVerticalAlignmentChanged;
			rbCellVerticalAlignmentCenter.Checked -= OnCellVerticalAlignmentChanged;
			rbCellVerticalAlignmentCenter.Unchecked -= OnCellVerticalAlignmentChanged;
			rbCellVerticalAlignmentBottom.Checked -= OnCellVerticalAlignmentChanged;
			rbCellVerticalAlignmentBottom.Unchecked -= OnCellVerticalAlignmentChanged;
		}
		void OnTableSizeControlChanged(object sender, EventArgs e) {
			TableSizeProperties properties = tableSizeControl.Properties;
			if (properties.UseDefaultValue.HasValue)
				Controller.UseDefaultTableWidth = properties.UseDefaultValue.Value;
			if (properties.Width.HasValue)
				Controller.TableWidth = properties.Width.Value;
			if (properties.WidthType.HasValue)
				Controller.TableWidthUnitType = properties.WidthType.Value;
			properties.MaxValue = Controller.GetTableWidthMaxValueConsiderWidthUnitType();
		}
		void OnSpnIndentFromLeftValueChanged(object sender, EventArgs e) {
			Controller.TableIndent = spnIndentFromLeft.Value.Value;
		}
		void OnTableAlignmentChanged(object sender, RoutedEventArgs e) {
			Controller.TableAlignment = GetTableAlignment();
			UpdateEnabledControls();
		}
		TableRowAlignment? GetTableAlignment() {
			if (rbTableAlignmentLeft.IsChecked.HasValue && rbTableAlignmentLeft.IsChecked.Value)
				return TableRowAlignment.Left;
			if (rbTableAlignmentCenter.IsChecked.HasValue && rbTableAlignmentCenter.IsChecked.Value)
				return TableRowAlignment.Center;
			if (rbTableAlignmentRight.IsChecked.HasValue && rbTableAlignmentRight.IsChecked.Value)
				return TableRowAlignment.Right;
			return null;
		}
		void OnTableRowHeightControlChanged(object sender, EventArgs e) {
			TableRowHeightProperties properties = tableRowHeightControl.Properties;
			Controller.UseDefaultRowHeight = properties.UseDefaultValue;
			if (properties.Height.HasValue)
				Controller.RowHeight = properties.Height.Value;
			if (properties.HeightType.HasValue)
				Controller.RowHeightType = properties.HeightType.Value;
		}
		void OnChkCantSplitEditValueChanged(object sender, EventArgs e) {
			Controller.RowCantSplit = !chkCantSplit.IsChecked;
		}
		void OnChkHeaderEditValueChanged(object sender, EventArgs e) {
			Controller.RowHeader = chkHeader.IsChecked;
		}
		void OnColumnSizeControlChanged(object sender, EventArgs e) {
			TableSizeProperties properties = columnSizeControl.Properties;
			Controller.UseDefaultColumnWidth = properties.UseDefaultValue;
			if (properties.Width.HasValue)
				Controller.ColumnWidth = properties.Width.Value;
			if (properties.WidthType.HasValue)
				Controller.ColumnWidthUnitType = properties.WidthType.Value;
		}
		void OnCellSizeControlChanged(object sender, EventArgs e) {
			TableSizeProperties properties = cellSizeControl.Properties;
			Controller.UseDefaultCellWidth = properties.UseDefaultValue;
			if (properties.Width.HasValue)
				Controller.CellWidth = properties.Width.Value;
			if (properties.WidthType.HasValue)
				Controller.CellWidthUnitType = properties.WidthType.Value;
		}
		void OnCellVerticalAlignmentChanged(object sender, RoutedEventArgs e) {
			Controller.CellVerticalAlignment = GetCellVerticalAlignment();
		}
		Model.VerticalAlignment? GetCellVerticalAlignment() {
			if (rbCellVerticalAlignmentTop.IsChecked.HasValue && rbCellVerticalAlignmentTop.IsChecked.Value)
				return Model.VerticalAlignment.Top;
			if (rbCellVerticalAlignmentCenter.IsChecked.HasValue && rbCellVerticalAlignmentCenter.IsChecked.Value)
				return Model.VerticalAlignment.Center;
			if (rbCellVerticalAlignmentBottom.IsChecked.HasValue && rbCellVerticalAlignmentBottom.IsChecked.Value)
				return Model.VerticalAlignment.Bottom;
			return null;
		}
		void OnBtnTableOptionsClick(object sender, RoutedEventArgs e) {
			ShowTableOptionsForm();
		}
		void ShowTableOptionsForm() {
			((RichEditControl)Control).ShowTableOptionsForm(Controller.Table, this);
		}
		void OnBtnBordersClick(object sender, RoutedEventArgs e) {
			ShowBorderShadingForm();
		}
		void ShowBorderShadingForm() {
			((RichEditControl)Control).ShowBorderShadingForm(Controller.SourceSelectedCells);
		}
		void OnBtnCellOptionsClick(object sender, RoutedEventArgs e) {
			ShowTableCellOptionsForm();
		}
		void ShowTableCellOptionsForm() {
			((RichEditControl)Control).ShowTableCellOptionsForm(Controller.SelectedCells, this);
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
		private void tableSizeControl_Loaded(object sender, RoutedEventArgs e) {
		}
	}
}
