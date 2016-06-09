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
using System.ComponentModel;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Tables.Native;
using DevExpress.XtraRichEdit.Design.Internal;
using System.Windows.Forms;
using DevExpress.Office;
using DevExpress.Utils.Internal;
#region FxCop suppressions
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TablePropertiesForm.tabControl")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TablePropertiesForm.tableTab")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TablePropertiesForm.rowTab")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TablePropertiesForm.columnTab")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TablePropertiesForm.btnOk")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TablePropertiesForm.btnCancel")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TablePropertiesForm.lblTableSize")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TablePropertiesForm.btnTableOptions")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TablePropertiesForm.lblTableAlignment")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TablePropertiesForm.spnIndentFromLeft")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TablePropertiesForm.lblIndentFromLeft")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TablePropertiesForm.lblRowOptions")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TablePropertiesForm.lblRowNumber")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TablePropertiesForm.lblRowSize")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TablePropertiesForm.chkHeader")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TablePropertiesForm.chkCantSplit")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TablePropertiesForm.btnPreviousRow")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TablePropertiesForm.btnNextRow")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TablePropertiesForm.lblColumn")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TablePropertiesForm.lblColumnSize")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TablePropertiesForm.btnNextColumn")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TablePropertiesForm.btnPreviousColumn")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TablePropertiesForm.cellTab")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TablePropertiesForm.btnCellOptions")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TablePropertiesForm.lblCellVerticalAlighment")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TablePropertiesForm.lblCellSize")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TablePropertiesForm.rgTableAlignment")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TablePropertiesForm.rgCellVerticalAlignment")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TablePropertiesForm.tableSizeControl")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TablePropertiesForm.tableRowHeightControl")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TablePropertiesForm.columnSizeControl")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TablePropertiesForm.cellSizeControl")]
#endregion
namespace DevExpress.XtraRichEdit.Forms {
	#region TablePropertiesForm
	[DXToolboxItem(false)]
	public partial class TablePropertiesForm : XtraForm {
		#region Fields
		readonly IRichEditControl control;
		readonly TablePropertiesFormController controller;
		#endregion
		TablePropertiesForm() {
			InitializeComponent();
		}
		public TablePropertiesForm(TablePropertiesFormControllerParameters controllerParameters) {
			Guard.ArgumentNotNull(controllerParameters, "controllerParameters");
			this.control = controllerParameters.Control;
			this.controller = CreateController(controllerParameters);
			InitializeComponent();
			InitializeForm();
			SubscribeControlsEvents();
			UpdateForm();
		}
		#region Properties
		public IRichEditControl Control { get { return control; } }
		public TablePropertiesFormController Controller { get { return controller; } }
		#endregion
		protected internal virtual TablePropertiesFormController CreateController(TablePropertiesFormControllerParameters controllerParameters) {
			return new TablePropertiesFormController(controllerParameters);
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
			this.spnIndentFromLeft.Properties.DefaultUnitType = uIUnit;
			this.spnIndentFromLeft.ValueUnitConverter = unitConverter;
			this.spnIndentFromLeft.Properties.MinValue = TablePropertiesFormDefaults.MinTableIndentByDefault;
			this.spnIndentFromLeft.Properties.MaxValue = TablePropertiesFormDefaults.MaxTableIndentByDefault;
		}
		protected internal virtual void SubscribeControlsEvents() {
			tableSizeControl.TableSizeControlChanged += OnTableSizeControlChanged;
			spnIndentFromLeft.ValueChanged += OnSpnIndentFromLeftValueChanged;
			rgTableAlignment.SelectedIndexChanged += OnTableAlignmentSelectedIndexChanged;
			tableRowHeightControl.TableRowHeightControlChanged += OnTableRowHeightControlChanged;
			chkCantSplit.CheckStateChanged += OnChkCantSplitCheckStateChanged;
			chkHeader.CheckStateChanged += OnChkHeaderCheckStateChanged;
			columnSizeControl.TableSizeControlChanged += OnColumnSizeControlChanged;
			cellSizeControl.TableSizeControlChanged += OnCellSizeControlChanged;
			rgCellVerticalAlignment.SelectedIndexChanged += OnCellVerticalAlignmentSelectedIndexChanged;
		}
		protected internal virtual void UnsubscribeControlsEvents() {
			tableSizeControl.TableSizeControlChanged -= OnTableSizeControlChanged;
			spnIndentFromLeft.ValueChanged -= OnSpnIndentFromLeftValueChanged;
			rgTableAlignment.SelectedIndexChanged -= OnTableAlignmentSelectedIndexChanged;
			tableRowHeightControl.TableRowHeightControlChanged -= OnTableRowHeightControlChanged;
			chkCantSplit.CheckStateChanged -= OnChkCantSplitCheckStateChanged;
			chkHeader.CheckStateChanged -= OnChkHeaderCheckStateChanged;
			columnSizeControl.TableSizeControlChanged -= OnColumnSizeControlChanged;
			cellSizeControl.TableSizeControlChanged -= OnCellSizeControlChanged;
			rgCellVerticalAlignment.SelectedIndexChanged -= OnCellVerticalAlignmentSelectedIndexChanged;
		}
		void OnTableSizeControlChanged(object sender, EventArgs e) {
			TableSizeProperties properties = tableSizeControl.Properties;
			Controller.UseDefaultTableWidth = properties.UseDefaultValue.Value;
			Controller.TableWidth = properties.Width.Value;
			Controller.TableWidthUnitType = properties.WidthType.Value;
			properties.MaxValue = Controller.GetTableWidthMaxValueConsiderWidthUnitType();
		}
		void OnSpnIndentFromLeftValueChanged(object sender, EventArgs e) {
			Controller.TableIndent = spnIndentFromLeft.Value.Value;
		}
		void OnTableAlignmentSelectedIndexChanged(object sender, EventArgs e) {
			Controller.TableAlignment = GetTableAlignment(rgTableAlignment.SelectedIndex);
			UpdateEnabledControls();
		}
		TableRowAlignment? GetTableAlignment(int selectedIndex) {
			switch (selectedIndex) {
				case 0:
					return TableRowAlignment.Left;
				case 1:
					return TableRowAlignment.Center;
				case 2:
					return TableRowAlignment.Right;
				default:
					return null;
			}
		}
		void OnTableRowHeightControlChanged(object sender, EventArgs e) {
			TableRowHeightProperties properties = tableRowHeightControl.Properties;
			Controller.UseDefaultRowHeight = properties.UseDefaultValue;
			Controller.RowHeight = properties.Height.Value;
			Controller.RowHeightType = properties.HeightType.Value;
		}
		void OnChkCantSplitCheckStateChanged(object sender, EventArgs e) {
			Controller.RowCantSplit = !CheckStateToNullableBool(chkCantSplit.CheckState);
		}
		void OnChkHeaderCheckStateChanged(object sender, EventArgs e) {
			Controller.RowHeader = CheckStateToNullableBool(chkHeader.CheckState);
		}
		bool? CheckStateToNullableBool(CheckState checkState) {
			switch (checkState) {
				case CheckState.Checked:
					return true;
				case CheckState.Unchecked:
					return false;
				case CheckState.Indeterminate:
					return null;
			}
			return null;
		}
		void OnColumnSizeControlChanged(object sender, EventArgs e) {
			TableSizeProperties properties = columnSizeControl.Properties;
			Controller.UseDefaultColumnWidth = properties.UseDefaultValue;
			Controller.ColumnWidth = properties.Width.Value;
			Controller.ColumnWidthUnitType = properties.WidthType.Value;
			properties.MaxValue = Controller.GetColumnWidthMaxValueConsiderWidthUnitType();
		}
		void OnCellSizeControlChanged(object sender, EventArgs e) {
			TableSizeProperties properties = cellSizeControl.Properties;
			Controller.UseDefaultCellWidth = properties.UseDefaultValue;
			Controller.CellWidth = properties.Width.Value;
			Controller.CellWidthUnitType = properties.WidthType.Value;
		}
		void OnCellVerticalAlignmentSelectedIndexChanged(object sender, EventArgs e) {
			Controller.CellVerticalAlignment = GetCellVerticalAlignment(rgCellVerticalAlignment.SelectedIndex);
		}
		VerticalAlignment? GetCellVerticalAlignment(int selectedIndex) {
			switch (selectedIndex) {
				case 0:
					return VerticalAlignment.Top;
				case 1:
					return VerticalAlignment.Center;
				case 2:
					return VerticalAlignment.Bottom;
				default:
					return null;
			}
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
			this.rgTableAlignment.SelectedIndex = CalculteTableAlignmentIndex(Controller.TableAlignment);
		}
		void UpdateEnabledControls() {
			TableRowAlignment? tableAlignment = Controller.TableAlignment;
			this.spnIndentFromLeft.Enabled = tableAlignment.HasValue && tableAlignment.Value == TableRowAlignment.Left;
		}
		int CalculteTableAlignmentIndex(TableRowAlignment? alignment) {
			if (!alignment.HasValue)
				return -1;
			switch (alignment.Value) {
				case TableRowAlignment.Left:
					return 0;
				case TableRowAlignment.Center:
					return 1;
				case TableRowAlignment.Right:
					return 2;
				default:
					return -1;
			}
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
			chkHeader.Enabled = Controller.IsSelectedFirstRowInTable();
		}
		void UpdateCheckEdit(CheckEdit checkEdit, bool? check) {
			if (check == null) {
				checkEdit.Properties.AllowGrayed = true;
				checkEdit.CheckState = CheckState.Indeterminate;
				return;
			}
			if (check.Value)
				checkEdit.CheckState = CheckState.Checked;
			else
				checkEdit.CheckState = CheckState.Unchecked;
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
			rgCellVerticalAlignment.SelectedIndex = CalculteCellVerticalAlignmentIndex(Controller.CellVerticalAlignment);
		}
		int CalculteCellVerticalAlignmentIndex(VerticalAlignment? alignment) {
			if (!alignment.HasValue)
				return -1;
			switch (alignment.Value) {
				case VerticalAlignment.Top:
					return 0;
				case VerticalAlignment.Center:
					return 1;
				case VerticalAlignment.Bottom:
					return 2;
				default:
					return -1;
			}
		}
		void OnBtnTableOptionsClick(object sender, EventArgs e) {
			ShowTableOptionsForm();
		}
		void ShowTableOptionsForm() {
			((RichEditControl)Control).ShowTableOptionsForm(Controller.Table, this); 
		}
		void OnBtnCellOptionsClick(object sender, EventArgs e) {
			ShowTableCellOptionsForm();
		}
		void ShowTableCellOptionsForm() {
			using (TableCellOptionsForm form = new TableCellOptionsForm(Controller.SelectedCells, (RichEditControl)Control)) {
				form.LookAndFeel.ParentLookAndFeel = LookAndFeel;
				FormTouchUIAdapter.ShowDialog(form, this);
			}
		}
		void OnBtnOkClick(object sender, EventArgs e) {
			Controller.ApplyChanges();
			DialogResult = DialogResult.OK;
		}
		private void ShowBorderShadingForm(BorderShadingForm form, SelectedCellsCollection selectedCells) {
			BorderShadingFormController controller = new BorderShadingFormController(Control, this.controller.Table.DocumentModel, selectedCells);
			form.Initialize(controller);
			FormTouchUIAdapter.ShowDialog(form, this);
		}
		private void btnBorder_Click(object sender, EventArgs e) {
			using (BorderShadingForm form = new BorderShadingForm()) {
				form.LookAndFeel.ParentLookAndFeel = LookAndFeel;
				Table controllerTable = this.controller.Table;
				DocumentModel documentModel = controllerTable.DocumentModel;
				if (documentModel.Selection.Start == documentModel.Selection.End) {
					DocumentLogPosition oldSelection = documentModel.Selection.Start;
					documentModel.BeginUpdate();
					try {
						TableCell firstCell = controllerTable.FirstRow.FirstCell;
						TableCell lastCell = controllerTable.LastRow.LastCell;
						documentModel.Selection.ManualySetTableSelectionStructureAndChangeSelection(firstCell, lastCell);
						SelectedCellsCollection selectedCells = documentModel.Selection.SelectedCells as SelectedCellsCollection;
						ShowBorderShadingForm(form, selectedCells);
						documentModel.Selection.ClearSelectionInTable();
						documentModel.Selection.SetInterval(oldSelection, oldSelection);
					}
					finally {
						documentModel.EndUpdate();
					}
				}
				else
					ShowBorderShadingForm(form, this.controller.SourceSelectedCells);
			}
		}
	}
	#endregion
}
