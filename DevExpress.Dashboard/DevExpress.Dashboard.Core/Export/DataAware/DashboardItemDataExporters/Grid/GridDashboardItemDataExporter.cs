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
using System.Linq;
using System.Collections.Generic;
using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Printing;
using DevExpress.DashboardCommon.ViewerData;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.Export;
using DevExpress.Utils;
using DevExpress.XtraExport.Helpers;
namespace DevExpress.DashboardExport {
	public class GridDashboardItemDataExporter : DashboardItemDataExporter<GridExportColumn, GridExportRow> {
		GridDashboardItemViewModel GridViewModel { get { return (GridDashboardItemViewModel)ViewModel; } }
		public GridDashboardItemDataExporter(DashboardItemExportData data, IDataAwareExportOptions options)
			: base(data, options) {
			options.ShowColumnHeaders = GridViewModel.ShowColumnHeaders ? DefaultBoolean.True : DefaultBoolean.False;
		}
		GridExportColumn CreateColumn(GridColumnViewModel viewModel, int index, string caption) {
			GridExportColumn column = new GridExportColumn(viewModel, index);
			if(GridViewModel.ShowColumnHeaders)
				column.Header = caption;
			return column;
		}
		GridExportColumn CreateMeasureColumn(GridColumnViewModel viewModel, int index, string caption) {
			GridExportColumn column = CreateColumn(viewModel, index, caption);
			MeasureDescriptor measure = MDData.GetMeasureDescriptorByID(viewModel.DataId);
			column.FormatSettings = DashboardItemDataFormatter.CreateValueFormatSettings(measure.InternalDescriptor.Format);
			return column;
		}
		protected override IList<GridExportColumn> CreateColumns() {
			List<GridExportColumn> columns = new List<GridExportColumn>();
			GridDashboardItemViewModel grid = GridViewModel;
			bool isSparklineMode = !string.IsNullOrEmpty(grid.SparklineAxisName);
			IList<AxisPoint> sparklinePoints = null;
			if(isSparklineMode)
				sparklinePoints = MDData.GetAxis(DashboardDataAxisNames.SparklineAxis).GetPoints();
			int index = 0;
			for(int i = 0; i < grid.Columns.Count; i++) {
				GridColumnViewModel viewModel = grid.Columns[i];
				if(viewModel.ColumnType == GridColumnType.Dimension) {
					GridExportColumn column = CreateColumn(viewModel, index++, viewModel.Caption);
					DimensionDescriptor dimensionDescriptor = MDData.GetDimensionDescriptorByID(viewModel.DataId);
					column.FormatSettings = DashboardItemDataFormatter.CreateValueFormatSettings(dimensionDescriptor.InternalDescriptor.Format);
					columns.Add(column);
				}
				else {
					if(viewModel.ColumnType == GridColumnType.Sparkline && isSparklineMode) {
						if(sparklinePoints.Count > maxXlsxColumnCount) {
							GridExportColumn column = CreateColumn(viewModel, index++, viewModel.Caption);
							column.IsIncorrectSparklineColumn = true;
							column.FormatSettings = new FormatSettings();
							columns.Add(column);
						}
						else {
							if(viewModel.ShowStartEndValues) {
								GridExportColumn start = CreateMeasureColumn(viewModel, index++, string.Format("{0} StartValue", viewModel.Caption));
								start.SparklinePoints = sparklinePoints;
								start.IsStartSparklineValue = true;
								columns.Add(start);
							}
							GridExportColumn sparkline = CreateColumn(viewModel, index++, viewModel.Caption);
							sparkline.IsSparklineColumn = true;
							sparkline.ColEditType = ColumnEditTypes.Sparkline;
							sparkline.SparklineInfo = new SparklineInfo(viewModel.SparklineOptions);
							sparkline.SparklinePoints = sparklinePoints;
							sparkline.FormatSettings = new FormatSettings();
							columns.Add(sparkline);
							if(viewModel.ShowStartEndValues) {
								GridExportColumn end = CreateMeasureColumn(viewModel, index++, string.Format("{0} EndValue", viewModel.Caption));
								end.SparklinePoints = sparklinePoints;
								end.IsEndSparklineValue = true;
								columns.Add(end);
							}
						}
					}
					else
						columns.Add(CreateMeasureColumn(viewModel, index++, viewModel.Caption));
				}
			}
			return columns;
		}
		protected override IList<GridExportRow> CreateRows() {
			List<GridExportRow> rows = new List<GridExportRow>();
			GridDashboardItemViewModel grid = GridViewModel;
			DataAxis area = MDData.GetAxis(DashboardDataAxisNames.DefaultAxis);
			foreach(AxisPoint point in area.GetPoints())
				rows.Add(new GridExportRow() { 
					IsDataAreaRow = true,
					AxisPoint = point
				});
			for(int i = 0; i < grid.TotalsCount; i++)
				rows.Add(new GridExportRow() { 
					IsFooter = true,
					TotalIndex = i
				});
			return rows;
		}
		protected override object GetRowCellValue(GridExportRow row, GridExportColumn column) {
			int totalIndex = row.TotalIndex;
			IList<GridColumnTotalViewModel> totals = column.ViewModel.Totals;
			if(row.IsFooter) {
				if(totals.Count > totalIndex) {
					GridColumnTotalViewModel totalModel = totals[totalIndex];
					MeasureDescriptor measure = MDData.GetMeasureDescriptorByID(totalModel.DataId);
					return string.Format(totalModel.Caption, MDData.GetValue(measure).DisplayText);
				}
				return null;
			}
			return GetValue(column, row);
		}
		object GetValue(GridExportColumn column, GridExportRow row) {
			if(column.IsIncorrectSparklineColumn)
				return sparklineColumnMessage;
			AxisPoint axisPoint = row.AxisPoint;
			GridColumnViewModel columnViewModel = column.ViewModel;
			if(columnViewModel.ColumnType == GridColumnType.Dimension)
				return GetDimensionValue(axisPoint, columnViewModel.DataId);
			else if(columnViewModel.DeltaDisplayMode == GridDeltaDisplayMode.Delta)
				return MDData.GetDeltaValue(axisPoint, columnViewModel.DataId).DisplayValue.Value;
			else if(columnViewModel.ColumnType == GridColumnType.Sparkline && column.SparklinePoints != null) {
				if(column.IsStartSparklineValue)
					return MDData.GetValue(axisPoint, column.SparklinePoints.FirstOrDefault(), columnViewModel.DataId).Value;
				if(column.IsEndSparklineValue)
					return MDData.GetValue(axisPoint, column.SparklinePoints.LastOrDefault(), columnViewModel.DataId).Value;
				IList<object> values = new List<object>();
				foreach(AxisPoint point in column.SparklinePoints)
					values.Add(MDData.GetValue(axisPoint, point, columnViewModel.DataId).Value);
				return values;
			}
			else
				return MDData.GetValue(axisPoint, columnViewModel.DataId).Value;
		}
		protected override FormatSettings GetRowCellFormatting(GridExportRow row, GridExportColumn column) {
			if(row.IsFooter || column.IsSparklineColumn)
				return null;
			GridColumnViewModel viewModel = column.ViewModel;
			if(viewModel.ColumnType == GridColumnType.Measure) {
				NumericFormatViewModel format = MDData.GetMeasureDescriptorByID(viewModel.DataId).InternalDescriptor.Format.NumericFormat;
				return GetMeasureFormatSettings(row, column, format);
			}
			if(viewModel.ColumnType == GridColumnType.Delta) {
				NumericFormatViewModel format;
				if(viewModel.DeltaDisplayMode == GridDeltaDisplayMode.Delta)
					format = MDData.GetDeltaDescriptorById(viewModel.DataId).InternalDescriptor.DisplayFormat;
				else
					format = MDData.GetMeasureDescriptorByID(viewModel.DataId).InternalDescriptor.Format.NumericFormat;
				return GetMeasureFormatSettings(row, column, format);
			}
			if(viewModel.ColumnType == GridColumnType.Dimension)
				return GetDimensionFormatSettings(row, column, MDData.GetDimensionDescriptorByID(viewModel.DataId).InternalDescriptor.Format);
			return null;
		}
	}
}
