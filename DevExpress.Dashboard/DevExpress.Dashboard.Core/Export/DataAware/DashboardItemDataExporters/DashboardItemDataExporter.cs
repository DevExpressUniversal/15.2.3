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
using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.Printing;
using DevExpress.DashboardCommon.ViewerData;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.Export;
using DevExpress.Export.Xl;
using DevExpress.XtraExport.Helpers;
namespace DevExpress.DashboardExport {
	public abstract class DashboardItemDataExporter<TColumn, TRow> : IGridView<TColumn, TRow> where TColumn : DashboardColumnImplementer where TRow : DashboardRowImplementer {
		protected const int maxXlsxColumnCount = 16384;
		protected const string sparklineColumnMessage = "Spaklines cannot be exported because the number of arguments exceeds 16384";
		readonly DashboardItemExportData data;
		readonly IDataAwareExportOptions options;
		readonly MultiDimensionalData mdData;
		IList<TColumn> columns;
		IList<TRow> rows;
		string IGridView<TColumn, TRow>.FilterString { get { return string.Empty; } }
		string IGridView<TColumn, TRow>.GetViewCaption { get { return ViewModel.Caption; } }
		bool IGridView<TColumn, TRow>.ReadOnly { get { return false; } }
		int IGridView<TColumn, TRow>.RowHeight { get { return 0; } }
		int IGridView<TColumn, TRow>.FixedRowsCount { get { return 0; } }
		bool IGridView<TColumn, TRow>.ShowFooter { get { return true; } }
		bool IGridView<TColumn, TRow>.ShowGroupedColumns { get { return false; } }
		IAdditionalSheetInfo IGridView<TColumn, TRow>.AdditionalSheetInfo {
			get {
				string sheetName = string.Format("{0}_Sparkline", ViewModel.Caption);
				if(sheetName.Length > 30)
					sheetName = sheetName.Substring(0, 30);
				return new SparklineSheetInfo(sheetName);
			}
		}
		bool IGridView<TColumn, TRow>.ShowGroupFooter { get { return false; } }
		bool IGridView<TColumn, TRow>.IsCancelPending { get { return false; } }
		long IGridView<TColumn, TRow>.RowCount { get { return Rows.Count; } }
		XlCellFormatting IGridView<TColumn, TRow>.AppearanceGroupRow { get { return null; } }
		XlCellFormatting IGridView<TColumn, TRow>.AppearanceEvenRow { get { return new XlCellFormatting(); } }
		XlCellFormatting IGridView<TColumn, TRow>.AppearanceOddRow { get { return new XlCellFormatting(); } }
		XlCellFormatting IGridView<TColumn, TRow>.AppearanceGroupFooter { get { return new XlCellFormatting(); } }
		XlCellFormatting IGridView<TColumn, TRow>.AppearanceHeader { get { return new XlCellFormatting(); } }
		XlCellFormatting IGridView<TColumn, TRow>.AppearanceFooter { get { return new XlCellFormatting(); } }
		XlCellFormatting IGridView<TColumn, TRow>.AppearanceRow { get { return new XlCellFormatting(); } }
		IGridViewAppearancePrint IGridView<TColumn, TRow>.AppearancePrint { get { return null; } }
		IEnumerable<FormatConditionObject> IGridView<TColumn, TRow>.FormatConditionsCollection { get { return null; } }
		IEnumerable<IFormatRuleBase> IGridView<TColumn, TRow>.FormatRulesCollection { get { return new List<IFormatRuleBase>(); } }
		IEnumerable<ISummaryItemEx> IGridView<TColumn, TRow>.GridGroupSummaryItemCollection { get { return new List<ISummaryItemEx>(); } }
		IEnumerable<ISummaryItemEx> IGridView<TColumn, TRow>.GridTotalSummaryItemCollection { get { return new List<ISummaryItemEx>(); } }
		IEnumerable<ISummaryItemEx> IGridView<TColumn, TRow>.GroupHeaderSummaryItems { get { return new List<ISummaryItemEx>(); } }
		IEnumerable<ISummaryItemEx> IGridView<TColumn, TRow>.FixedSummaryItems { get { return new List<ISummaryItemEx>(); } }
		bool IGridViewBase<TColumn, TRow, TColumn, TRow>.GetAllowMerge(TColumn col) { return false; }
		string IGridViewBase<TColumn, TRow, TColumn, TRow>.GetRowCellHyperlink(TRow row, TColumn col) { return null; }
		int IGridViewBase<TColumn, TRow, TColumn, TRow>.RaiseMergeEvent(int startRow, int rowLogicalPosition, TColumn col) { return 0; }
		IEnumerable<TColumn> IGridView<TColumn, TRow>.GetGroupedColumns() { return new List<TColumn>(); }
		IEnumerable<TColumn> IGridView<TColumn, TRow>.GetAllColumns() { return Columns; }
		IEnumerable<TRow> IGridView<TColumn, TRow>.GetAllRows() { return Rows; }
		FormatSettings IGridViewBase<TColumn, TRow, TColumn, TRow>.GetRowCellFormatting(TRow irow, TColumn icol) { return GetRowCellFormatting(irow, icol); }
		object IGridViewBase<TColumn, TRow, TColumn, TRow>.GetRowCellValue(TRow irow, TColumn icol) { return GetRowCellValue(irow, icol); }
		IList<TColumn> Columns {
			get {
				if(columns == null)
					columns = CreateColumns();
				return columns;
			}
		}
		IList<TRow> Rows {
			get {
				if(rows == null)
					rows = CreateRows();
				return rows;
			}
		}
		DashboardItemExportData Data { get { return data; } }
		protected DashboardItemServerData ServerData { get { return Data.ServerData; } }
		protected MultiDimensionalData MDData { get { return mdData; } }
		protected DashboardItemViewModel ViewModel { get { return ServerData.ViewModel; } }
		protected DashboardItemDataExporter(DashboardItemExportData data, IDataAwareExportOptions options) {
			this.data = data;
			this.options = options;
			mdData = CreateMultiDimensionalData();
		}
		MultiDimensionalData CreateMultiDimensionalData() {
			HierarchicalMetadata hMetaData = ServerData.Metadata;
			if(hMetaData == null)
				return null;
			MultidimensionalDataDTO multiDataDTO = ServerData.MultiDimensionalData;
			ClientHierarchicalMetadata metaData = new ClientHierarchicalMetadata(hMetaData);
			return new MultiDimensionalData(multiDataDTO.HierarchicalDataParams, metaData);
		}
		protected abstract IList<TColumn> CreateColumns();
		protected abstract IList<TRow> CreateRows();
		protected abstract object GetRowCellValue(TRow irow, TColumn icol);
		protected virtual FormatSettings GetRowCellFormatting(TRow irow, TColumn icol) {
			return null;
		}
		protected FormatSettings GetMeasureFormatSettings(TRow row, TColumn column, NumericFormatViewModel format) {
			if(format != null)
				if(format.FormatType == NumericFormatType.Number || format.FormatType == NumericFormatType.Currency || format.FormatType == NumericFormatType.Percent) {
					object cellValue = GetRowCellValue(row, column);
					string pattern = NumericFormatter.CreateInstance(format).GetFormatPattern(cellValue);
					Type actualDataType = cellValue != null ? cellValue.GetType() : typeof(object);
					return new FormatSettings() { FormatString = pattern, ActualDataType = actualDataType };
				}
			return null;
		}
		protected FormatSettings GetDateTimeDimensionFormatSettings(TRow row, TColumn column, DateTimeFormatViewModel format) {
			if(format != null) {
				object cellValue = GetRowCellValue(row, column);
				string pattern = DateTimeFormatter.CreateInstance(format).FormatPattern;
				Type actualDataType = cellValue != null ? cellValue.GetType() : typeof(object);
				return new FormatSettings() { FormatString = pattern, ActualDataType = actualDataType };
			}
			return null;
		}
		protected FormatSettings GetDimensionFormatSettings(TRow row, TColumn column, ValueFormatViewModel format) {
			if(format != null) {
				if(format.NumericFormat != null)
					return GetMeasureFormatSettings(row, column, format.NumericFormat);
				if(format.DateTimeFormat != null)
					return GetDateTimeDimensionFormatSettings(row, column, format.DateTimeFormat);
			}
			return null;
		}
		protected object GetDimensionValue(AxisPoint axisPoint, string dataId) {
			DimensionDescriptor dimension = mdData.GetDimensionDescriptorByID(dataId);
			return GetDimensionValue(axisPoint, dimension);
		}
		protected object GetDimensionValue(AxisPoint axisPoint, DimensionDescriptor dimension) {
			object value = axisPoint.GetDimensionValue(dimension).Value;
			string specialValueString;
			if(DashboardSpecialValuesInternal.TryGetDisplayText(value, out specialValueString))
				return specialValueString;
			DateTimeFormatViewModel dateTimeFormat = dimension.InternalDescriptor.Format.DateTimeFormat;
			if(dateTimeFormat != null) {
				if(dateTimeFormat.GroupInterval == DateTimeGroupInterval.Hour) {
					DateTime dateTimeValue;
					if(DateTimeHourFormatter.TryConvertIntegerToDateTime(value, out dateTimeValue))
						return dateTimeValue;
				}
				if(!DashboardItemDataFormatter.SupportFormatSettings(dateTimeFormat))
					return DateTimeFormatter.CreateInstance(dateTimeFormat).Format(value);
			}
			return value;
		}
	}
}
