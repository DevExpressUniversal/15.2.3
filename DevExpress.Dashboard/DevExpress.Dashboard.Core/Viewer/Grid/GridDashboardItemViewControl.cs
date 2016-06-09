#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.Drawing;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.ViewerData;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.DataAccess.Native.Data;
using System.Linq;
namespace DevExpress.DashboardCommon.Viewer {
	public interface IGridColumn {
		string FieldName { get; set; }
		bool Visible { get; set; }
		string Caption { get; set; }
		int VisibleIndex { get; set; }
		int ActualIndex { get; set; }
		bool AllowGroup { get; set; }
		bool AllowMove { get; set; }
		bool AllowCellMerge { get; set; }
		GridColumnFilterMode FilterMode { get; set; }
		double Weight { get; set; }
		double FixedWidth { get; set; }
		GridColumnFixedWidthType WidthType { get; set; }
		GridColumnDisplayMode DisplayMode { get; set; }
		double DefaultBestCharacterCount { get; set; }
		bool IgnoreDeltaIndication { get; set; }
		int MaxIconStyleImageWidth { get; set; }
		bool TextIsHidden { get; set; }
	}
	public interface IGridControl {
		bool AllowIncrementalSearch { get; set; }
		bool AllowCellMerge { get; set; }
		bool EnableAppearanceEvenRow { get; set; }
		bool ShowHorizontalLines { get; set; }
		bool ShowVerticalLines { get; set; }
		bool ShowColumnHeaders { get; set; }
		bool AllColumnsFixed { get; set; }
		GridColumnWidthMode ColumnWidthMode { get; set; }
		bool WordWrap { get; set; }
		bool ShowFooter { get; set; }
		IList<IGridColumn> Columns { get; }
		int ColumnsCount { get; }
		event EventHandler<CustomColumnDisplayTextEventArgsBase> GridViewCustomColumnDisplayText;
		event EventHandler<CustomGridDisplayTextEventArgs> CustomGridDisplayText;
		event EventHandler<GridCustomDrawCellEventArgsBase> CustomDrawCell;
		void ClearColumns();
		IGridColumn AddColumn(GridColumnViewModel column);
		IGridColumn AddColumn(GridColumnViewModel column, ValueFormatViewModel exportFormat, GridViewerDataController dataController);
		void SetData(ReadOnlyTypedList data);
		void BeginUpdate();
		void EndUpdate();
		void ResetClientState();
	}
	public abstract class CustomColumnDisplayTextEventArgsBase : EventArgs {
		public abstract string ColumnFieldName { get; }
		public abstract int Index { get; }
		public abstract void SetDisplayText(string displayText);
	}
	public class CustomGridDisplayTextEventArgs : EventArgs {
		public object Value { get; private set; }
		public string ColumnId { get; private set; }
		public string DisplayText { get; set; }
		public CustomGridDisplayTextEventArgs(object value, string columnId) {
			Value = value;
			ColumnId = columnId;
		}
	}
	public abstract class GridCustomDrawCellEventArgsBase : CustomDrawCellEventArgsBase {
		readonly string columnId;
		readonly int rowIndex;
		public string ColumnId { get { return columnId; } }
		public int RowIndex { get { return rowIndex; } }
		protected GridCustomDrawCellEventArgsBase(string columnId, int rowIndex, bool isDarkSkin, bool ignoreColorAndBackColor, Color defaultBackColor)
			: base(isDarkSkin, ignoreColorAndBackColor, defaultBackColor) {
			this.columnId = columnId;
			this.rowIndex = rowIndex;
		}
	}
	public class GridDashboardItemViewControl {
		IGridControl gridControl;
		readonly Dictionary<string, GridColumnViewModel> gridColumnsCache = new Dictionary<string, GridColumnViewModel>();
		GridViewerDataController dataController;
		public GridViewerDataController DataController { get { return dataController; } }
		public GridDashboardItemViewControl(IGridControl gridControl) {
			this.gridControl = gridControl;
			gridControl.GridViewCustomColumnDisplayText += OnGridViewCustomColumnDisplayText;
			gridControl.CustomGridDisplayText += OnGridControlCustomGridDisplayText;
			gridControl.CustomDrawCell += OnGridControlCustomDrawCell;
		}
		public void Update(GridDashboardItemViewModel viewModel, ConditionalFormattingModel cfModel, MultiDimensionalData data) {
			dataController = new GridViewerDataController(viewModel, cfModel, data);
			gridControl.BeginUpdate();
			UpdateData(null);
			try {
				ApplyViewModelToGridControl(viewModel, cfModel);
			}
			finally {
				UpdateData(dataController.GetDataSource());
				gridControl.EndUpdate();
			}
		}
		public GridColumnViewModel GetColumnViewModel(string fieldName) {
			return gridColumnsCache[fieldName];
		}
		public decimal NormalizeBarValue(string columnId, object value) {
			return dataController.NormalizeValue(columnId, value);
		}
		public decimal GetBarZeroPosition(string columnId) {
			return dataController.GetZeroPosition(columnId);
		}
		public Dictionary<object, string> GetDisplayTexts(string columnId) {
			return dataController.GetDisplayTexts(columnId);
		}
		void OnGridControlCustomDrawCell(object sender, GridCustomDrawCellEventArgsBase e) {
			dataController.FillStyleSettings(e);
		}
		public IEnumerable<object> GetUniqueValues(int dataSourceRowIndex, IList<string> columnIds){
			return dataController.GetUniqueValues(dataSourceRowIndex, columnIds);
		}
		public IEnumerable<int> GetDataSourceIndices(IEnumerable<AxisPointTuple> tuples, string targetAxis) {
			return dataController.GetDataSourceIndices(tuples, targetAxis);
		}
		void ApplyViewModelToGridControl(GridDashboardItemViewModel gridViewModel, ConditionalFormattingModel cfModel) {
			ApplyViewModelToGridOptions(gridViewModel);
			ApplyViewModelToGridColumns(gridViewModel, cfModel);
		}
		void ApplyViewModelToGridColumns(GridDashboardItemViewModel gridViewModel, ConditionalFormattingModel cfModel) {
			gridControl.ClearColumns();
			gridColumnsCache.Clear();
			Dictionary<FormatRuleModelBase, List<StyleSettingsModel>> iconStyleRules = GetIconStyleRules(cfModel);
			IList<GridColumnViewModel> visibleColumns = gridViewModel.Columns;
			gridControl.GridViewCustomColumnDisplayText -= OnGridViewCustomColumnDisplayText;
			try {
				foreach(GridColumnViewModel columnViewModel in gridViewModel.Columns) {
					IGridColumn column = gridControl.AddColumn(columnViewModel, dataController.GetColumnFormat(columnViewModel.DataId), dataController);
					string fieldName = columnViewModel.DataId;
					gridColumnsCache.Add(fieldName, columnViewModel);
					column.FieldName = fieldName;
					column.AllowGroup = false;
					column.VisibleIndex = visibleColumns.IndexOf(columnViewModel);
					column.Caption = columnViewModel.Caption;
					column.FilterMode = columnViewModel.GridColumnFilterMode;
					column.AllowMove = false;
					column.Visible = true;
					column.AllowCellMerge = columnViewModel.AllowCellMerge;
					column.ActualIndex = columnViewModel.ActualIndex;
					column.Weight = columnViewModel.Weight;
					column.FixedWidth = columnViewModel.FixedWidth;
					column.WidthType = columnViewModel.WidthType;
					column.DisplayMode = columnViewModel.DisplayMode;
					column.DefaultBestCharacterCount = columnViewModel.DefaultBestCharacterCount;
					column.IgnoreDeltaIndication = columnViewModel.IgnoreDeltaIndication;
					column.MaxIconStyleImageWidth = GetMaxIconStyleImageWidth(iconStyleRules, columnViewModel.DataId);
					column.TextIsHidden = GetTextIsHidden(cfModel.RuleModels, columnViewModel.DataId);
				}
			}
			finally {
				gridControl.GridViewCustomColumnDisplayText += OnGridViewCustomColumnDisplayText;
			}
		}
		bool GetTextIsHidden(IList<FormatRuleModelBase> ruleModels, string dataId) {
			IEnumerable<GridFormatRuleModel> rules = ruleModels.Cast<GridFormatRuleModel>().Where(rule => rule.ApplyToDataId == dataId);
			bool textIsHidden = false;
			foreach(GridFormatRuleModel rule in rules) {
				BarConditionModel barModel = rule.ConditionModel as BarConditionModel;
				if(barModel != null)
					textIsHidden = barModel.BarOptions.ShowBarOnly;
			}
			return textIsHidden;
		}
		int GetMaxIconStyleImageWidth(Dictionary<FormatRuleModelBase, List<StyleSettingsModel>> iconStyleRules, string dataId) {
			int maxImageWidth = 0;
			FormatConditionImageProviderBase provider = new FormatConditionImageProviderBase();
			foreach(GridFormatRuleModel rule in iconStyleRules.Keys) {
				if(rule.ApplyToDataId == dataId || rule.ApplyToRow) {
					List<StyleSettingsModel> styles = iconStyleRules[rule];
					foreach(StyleSettingsModel style in styles) {
						Image image = provider.GetImage(style.IconType, style.Image, FormatConditionColorScheme.Light);
						if(image != null)
							maxImageWidth = Math.Max(maxImageWidth, image.Width);
					}
				}
			}
			return maxImageWidth;
		}
		Dictionary<FormatRuleModelBase, List<StyleSettingsModel>> GetIconStyleRules(ConditionalFormattingModel cfModel) {
			Dictionary<FormatRuleModelBase, List<StyleSettingsModel>> iconStyleRules = new Dictionary<FormatRuleModelBase, List<StyleSettingsModel>>();
			if(cfModel != null && cfModel.FormatConditionStyleSettings != null)
				foreach(StyleSettingsModel style in cfModel.FormatConditionStyleSettings)
					if(style.IconType != FormatConditionIconType.None || style.Image != null) {
						FormatRuleModelBase rule = cfModel.RuleModels[style.RuleIndex];
						if(!iconStyleRules.ContainsKey(rule))
							iconStyleRules[rule] = new List<StyleSettingsModel>();
						iconStyleRules[rule].Add(style);
					}
			return iconStyleRules;
		}
		void ApplyViewModelToGridOptions(GridDashboardItemViewModel viewModel) {
			gridControl.AllowIncrementalSearch = true;
			gridControl.AllowCellMerge = viewModel.AllowCellMerge;
			gridControl.EnableAppearanceEvenRow = viewModel.EnableBandedRows;
			gridControl.ShowHorizontalLines = viewModel.ShowHorizontalLines;
			gridControl.ShowVerticalLines = viewModel.ShowVerticalLines;
			gridControl.ShowColumnHeaders = viewModel.ShowColumnHeaders;
			gridControl.ColumnWidthMode = viewModel.ColumnWidthMode;
			gridControl.WordWrap = viewModel.WordWrap;
			gridControl.AllColumnsFixed = viewModel.AllColumnsFixed;
			gridControl.ShowFooter = viewModel.ShowFooter;
		}
		void UpdateData(GridMultiDimensionalDataSource data) {
			gridControl.SetData(data);
		}
		void OnGridViewCustomColumnDisplayText(object sender, CustomColumnDisplayTextEventArgsBase e) {
			GridColumnViewModel columnModel = gridColumnsCache[e.ColumnFieldName];
			if(columnModel.ColumnType == GridColumnType.Sparkline || e.Index < 0)
				return;
			e.SetDisplayText(dataController.GetDisplayText(e.ColumnFieldName, e.Index));
		}
		void OnGridControlCustomGridDisplayText(object sender, CustomGridDisplayTextEventArgs e) {
			e.DisplayText = dataController.Format(e.Value, e.ColumnId);
		}
		internal void ResetGridWidthOptions(GridDashboardItemViewModel viewModel) {
			gridControl.BeginUpdate();
			try {
				gridControl.ResetClientState();
				gridControl.ColumnWidthMode = viewModel.ColumnWidthMode;
				gridControl.WordWrap = viewModel.WordWrap;
				for(int i = 0; i < gridControl.ColumnsCount; i++) {
					IGridColumn column = gridControl.Columns[i];
					GridColumnViewModel viewModelColumn = viewModel.Columns[i];
					column.ActualIndex = viewModelColumn.ActualIndex;
					column.Weight = viewModelColumn.Weight;
					column.FixedWidth = viewModelColumn.FixedWidth;
					column.WidthType = viewModelColumn.WidthType;
					column.DisplayMode = viewModelColumn.DisplayMode;
					column.DefaultBestCharacterCount = viewModelColumn.DefaultBestCharacterCount;
					column.IgnoreDeltaIndication = viewModelColumn.IgnoreDeltaIndication;
				}
			}
			finally {
				gridControl.EndUpdate();
			}
		}
	}
}
