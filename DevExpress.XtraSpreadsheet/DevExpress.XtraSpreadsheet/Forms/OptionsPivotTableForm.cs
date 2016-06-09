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

using DevExpress.Utils;
using DevExpress.XtraEditors;
using System.Windows.Forms;
namespace DevExpress.XtraSpreadsheet.Forms {
	public partial class OptionsPivotTableForm : XtraForm {
		#region Field
		readonly OptionsPivotTableViewModel viewModel;
		#endregion
		OptionsPivotTableForm() {
			InitializeComponent();
		}
		public OptionsPivotTableForm(OptionsPivotTableViewModel viewModel) {
			Guard.ArgumentNotNull(viewModel, "viewModel");
			this.viewModel = viewModel;
			InitializeComponent();
			SetBindingsForm();
		}
		#region Properties
		protected OptionsPivotTableViewModel ViewModel { get { return viewModel; } }
		#endregion
		void SetBindingsForm() {
			SetBingingName();
			SetBindingsLayoutAndFormat();
			SetBindingsTotalsAndFilters();
			SetBindingsDisplay();
			SetBindingsPrinting();
			SetBindingsData();
			SetBindingsAltText();
		}
		void SetBingingName() {
			this.edtPivotTableName.DataBindings.Add("EditValue", ViewModel, "Name", false, DataSourceUpdateMode.OnValidation);
		}
		void SetBindingsLayoutAndFormat() {
			this.chkMergeAndCenterCells.DataBindings.Add("EditValue", ViewModel, "MergeItem", false, DataSourceUpdateMode.OnPropertyChanged);
			this.edtIndentRowLabels.DataBindings.Add("EditValue", ViewModel, "Indent", false, DataSourceUpdateMode.OnPropertyChanged);
			this.edtFieldsReportFilterArea.Properties.DataSource = ViewModel.FieldsReportFilterList;
			this.edtFieldsReportFilterArea.DataBindings.Add("EditValue", ViewModel, "FieldsReportFilterAreaMode", false, DataSourceUpdateMode.OnPropertyChanged);
			this.lblColumn.DataBindings.Add("Visible", ViewModel, "ColumnTextVisible", false, DataSourceUpdateMode.OnPropertyChanged);
			this.lblRow.DataBindings.Add("Visible", ViewModel, "RowTextVisible", false, DataSourceUpdateMode.OnPropertyChanged);
			this.edtReportFilterFields.DataBindings.Add("EditValue", viewModel, "PageWrap", false, DataSourceUpdateMode.OnPropertyChanged);
			this.chkErrorValuesShow.DataBindings.Add("EditValue", ViewModel, "ShowError", false, DataSourceUpdateMode.OnPropertyChanged);
			this.edtErrorValuesShow.DataBindings.Add("Enabled", ViewModel, "ErrorCaptionEnabled", false, DataSourceUpdateMode.OnPropertyChanged);
			this.edtErrorValuesShow.DataBindings.Add("EditValue", ViewModel, "ErrorCaption", false, DataSourceUpdateMode.OnValidation);
			this.chkEmptyCellsShow.DataBindings.Add("EditValue", ViewModel, "ShowMissing", false, DataSourceUpdateMode.OnPropertyChanged);
			this.edtEmptyCellsShow.DataBindings.Add("Enabled", ViewModel, "MissingCaptionEnabled", false, DataSourceUpdateMode.OnPropertyChanged);
			this.edtEmptyCellsShow.DataBindings.Add("EditValue", ViewModel, "MissingCaption", false, DataSourceUpdateMode.OnValidation);
			this.chkAutofitColumnWidths.DataBindings.Add("EditValue", ViewModel, "UseAutoFormatting", false, DataSourceUpdateMode.OnPropertyChanged);
			this.chkPreserveCellFormatting.DataBindings.Add("EditValue", ViewModel, "PreserveFormatting", false, DataSourceUpdateMode.OnPropertyChanged);
		}
		void SetBindingsTotalsAndFilters() {
			this.chkGrandTotalsRows.DataBindings.Add("EditValue", ViewModel, "RowGrandTotals", false, DataSourceUpdateMode.OnPropertyChanged);
			this.chkGrandTotalsColumns.DataBindings.Add("EditValue", ViewModel, "ColumnGrandTotals", false, DataSourceUpdateMode.OnPropertyChanged);
			this.chkSubtotalFilteredItems.DataBindings.Add("Enabled", ViewModel, "SubtotalHiddenItemsEnabled", false, DataSourceUpdateMode.OnPropertyChanged);
			this.chkSubtotalFilteredItems.DataBindings.Add("EditValue", ViewModel, "SubtotalHiddenItems", false, DataSourceUpdateMode.OnPropertyChanged);
			this.chkMultipleFiltersField.DataBindings.Add("EditValue", ViewModel, "MultipleFieldFilters", false, DataSourceUpdateMode.OnPropertyChanged);
			this.chkUseCustomLists.DataBindings.Add("EditValue", ViewModel, "CustomListSort", false, DataSourceUpdateMode.OnPropertyChanged);
		}
		void SetBindingsDisplay() {
			this.chkShowExpandCollapseButtons.DataBindings.Add("EditValue", ViewModel, "ShowDrill", false, DataSourceUpdateMode.OnPropertyChanged);
			this.chkShowContextualTooltips.DataBindings.Add("EditValue", ViewModel, "ShowDataTips", false, DataSourceUpdateMode.OnPropertyChanged);
			this.chkShowPropertiesInTooltips.DataBindings.Add("Enabled", ViewModel, "ShowMemberPropertyTipsEnabled", false, DataSourceUpdateMode.OnPropertyChanged);
			this.chkShowPropertiesInTooltips.DataBindings.Add("EditValue", ViewModel, "ShowMemberPropertyTips", false, DataSourceUpdateMode.OnPropertyChanged);
			this.chkDisplayFieldAndFilter.DataBindings.Add("EditValue", ViewModel, "ShowHeaders", false, DataSourceUpdateMode.OnPropertyChanged);
			this.chkShowTheValuesRow.DataBindings.Add("Enabled", ViewModel, "ShowValuesRowEnabled", false, DataSourceUpdateMode.OnPropertyChanged);
			this.chkShowTheValuesRow.DataBindings.Add("EditValue", ViewModel, "ShowValuesRow", false, DataSourceUpdateMode.OnPropertyChanged);
			this.chkShowItemsWithNoDataOnRows.DataBindings.Add("Enabled", ViewModel, "ShowEmptyRowEnabled", false, DataSourceUpdateMode.OnPropertyChanged);
			this.chkShowItemsWithNoDataOnRows.DataBindings.Add("EditValue", ViewModel, "ShowEmptyRow", false, DataSourceUpdateMode.OnPropertyChanged);
			this.chkShowItemsWithNoDataOnColumns.DataBindings.Add("Enabled", ViewModel, "ShowEmptyColumnEnabled", false, DataSourceUpdateMode.OnPropertyChanged);
			this.chkShowItemsWithNoDataOnColumns.DataBindings.Add("EditValue", ViewModel, "ShowEmptyColumn", false, DataSourceUpdateMode.OnPropertyChanged);
			this.chkDisplayItemLabels.DataBindings.Add("Enabled", ViewModel, "ShowItemsEnabled", false, DataSourceUpdateMode.OnPropertyChanged);
			this.chkDisplayItemLabels.DataBindings.Add("EditValue", ViewModel, "ShowItems", false, DataSourceUpdateMode.OnPropertyChanged);
			this.rgrpSort.DataBindings.Add("EditValue", ViewModel, "FieldListSortAscending", false, DataSourceUpdateMode.OnPropertyChanged);
		}
		void SetBindingsPrinting() {
			this.chkPrintExpandColapseButtons.DataBindings.Add("EditValue", ViewModel, "PrintDrill", false, DataSourceUpdateMode.OnPropertyChanged);
			this.chkRepeatRowLabels.DataBindings.Add("EditValue", ViewModel, "ItemPrintTitles", false, DataSourceUpdateMode.OnPropertyChanged);
			this.chkSetPrintTitles.DataBindings.Add("EditValue", ViewModel, "FieldPrintTitles", false, DataSourceUpdateMode.OnPropertyChanged);
		}
		void SetBindingsData() {
			this.chkSaveSourceDataWithFile.DataBindings.Add("EditValue", ViewModel, "SaveData", false, DataSourceUpdateMode.OnPropertyChanged);
			this.chkEnableShowDetails.DataBindings.Add("EditValue", ViewModel, "EnableDrill", false, DataSourceUpdateMode.OnPropertyChanged);
			this.chkRerfeshData.DataBindings.Add("EditValue", ViewModel, "RefreshOnLoad", false, DataSourceUpdateMode.OnPropertyChanged);
			this.edtNumberOfItems.Properties.DataSource = ViewModel.MissingItemsLimitList;
			this.edtNumberOfItems.DataBindings.Add("EditValue", ViewModel, "MissingItemsLimitMode", false, DataSourceUpdateMode.OnPropertyChanged);
			this.chkEnableCellEditing.DataBindings.Add("Enabled", ViewModel, "EditDataEnabled", false, DataSourceUpdateMode.OnPropertyChanged);
			this.chkEnableCellEditing.DataBindings.Add("EditValue", ViewModel, "EditData", false, DataSourceUpdateMode.OnPropertyChanged);
		}
		void SetBindingsAltText() {
			this.edtTitle.DataBindings.Add("EditValue", ViewModel, "AltText", false, DataSourceUpdateMode.OnValidation);
			this.edtDecsription.DataBindings.Add("EditValue", ViewModel, "AltTextSummary", false, DataSourceUpdateMode.OnValidation);
		}
		private void btnOk_Click(object sender, System.EventArgs e) {
			this.TopMost = false;
			try {
				if (ViewModel.Validate()) {
					ViewModel.ApplyChanges();
					this.DialogResult = DialogResult.OK;
					Close();
				}
			}
			finally {
				this.TopMost = true;
			}
		}
	}
}
