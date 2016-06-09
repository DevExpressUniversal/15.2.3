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

using System.Collections.Generic;
using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Printing;
using DevExpress.DashboardCommon.ViewerData;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.Export;
using DevExpress.XtraExport.Helpers;
namespace DevExpress.DashboardExport {
	public class ChartDashboardItemDataExporter : DashboardItemDataExporter<ChartExportColumn, ChartExportRow> {
		int columnIndex = 0;
		ChartDashboardItemViewModel ChartViewModel { get { return (ChartDashboardItemViewModel)ViewModel; } }
		public ChartDashboardItemDataExporter(DashboardItemExportData data, IDataAwareExportOptions options)
			: base(data, options) {
		}
		protected override IList<ChartExportColumn> CreateColumns() {
			List<ChartExportColumn> columns = new List<ChartExportColumn>();
			ChartDashboardItemViewModel chart = ChartViewModel;
			if(chart.ActualSeriesDataMembers.Length > 0 && chart.ActualArgumentDataMembers.Length > 0)
				foreach(string series in chart.ActualSeriesDataMembers) {
					DimensionDescriptor dimension = MDData.GetDimensionDescriptorByID(series);
					columns.Add(new ChartExportColumn() {
						IsRowAreaColumn = true,
						Dimension = dimension,
						FormatSettings = new FormatSettings(),
						LogicalPosition = columnIndex++
					});
				}
			List<string> valuesDataMembers = new List<string>();
			foreach(ChartPaneViewModel pane in chart.Panes)
				foreach(ChartSeriesTemplateViewModel seriesTemplate in pane.SeriesTemplates)
					valuesDataMembers.AddRange(seriesTemplate.DataMembers);
			if(valuesDataMembers.Count > 1)
				columns.Add(new ChartExportColumn() {
					IsMeasureCaptionColumn = true,
					FormatSettings = new FormatSettings(),
					LogicalPosition = columnIndex++
				});
			string axisName = DashboardDataAxisNames.ChartArgumentAxis;
			if(chart.ActualSeriesDataMembers.Length > 0 && chart.ActualArgumentDataMembers.Length == 0)
				axisName = DashboardDataAxisNames.ChartSeriesAxis;
			foreach(AxisPoint point in MDData.GetAxis(axisName).GetPoints())
				columns.Add(new ChartExportColumn() {
					AxisPoint = point,
					FormatSettings = new FormatSettings(),
					LogicalPosition = columnIndex++
				});
			if(chart.ActualSeriesDataMembers.Length == 0 && chart.ActualArgumentDataMembers.Length == 0)
				columns.Add(new ChartExportColumn() {
					FormatSettings = new FormatSettings(),
					LogicalPosition = columnIndex++
				});
			return columns;
		}
		void AddRows(IList<ChartExportRow> rows, IEnumerable<string> valueDataMembers, AxisPoint axisPoint) {
			foreach(string id in valueDataMembers) {
				MeasureDescriptor measure = MDData.GetMeasureDescriptorByID(id);
				rows.Add(new ChartExportRow() {
					AxisPoint = axisPoint,
					Measure = measure,
					FormatSettings = DashboardItemDataFormatter.CreateValueFormatSettings(measure.InternalDescriptor.Format)
				});
			}
		}
		protected override IList<ChartExportRow> CreateRows() {
			List<ChartExportRow> rows = new List<ChartExportRow>();
			ChartDashboardItemViewModel chart = ChartViewModel;
			string[] headerDataMembers = chart.ActualArgumentDataMembers;
			string[] rowDataMembers = chart.ActualSeriesDataMembers;
			string axisName = DashboardDataAxisNames.ChartSeriesAxis;
			if(chart.ActualSeriesDataMembers.Length > 0 && chart.ActualArgumentDataMembers.Length == 0) {
				headerDataMembers = chart.ActualSeriesDataMembers;
				rowDataMembers = chart.ActualArgumentDataMembers;
				axisName = DashboardDataAxisNames.ChartArgumentAxis;
			}
			foreach(string id in headerDataMembers)
				rows.Add(new ChartExportRow() {
					IsColumnAreaRow = true,
					Dimension = MDData.GetDimensionDescriptorByID(id)
				});
			List<string> valuesDataMembers = new List<string>();
			foreach(ChartPaneViewModel pane in chart.Panes)
				foreach(ChartSeriesTemplateViewModel seriesTemplate in pane.SeriesTemplates)
					valuesDataMembers.AddRange(seriesTemplate.DataMembers);
			if(rowDataMembers.Length > 0)
				foreach(AxisPoint point in MDData.GetAxisPoints(axisName))
					AddRows(rows, valuesDataMembers, point);
			else
				AddRows(rows, valuesDataMembers, null);
			return rows;
		}
		protected override object GetRowCellValue(ChartExportRow row, ChartExportColumn column) {
			if(row.IsColumnAreaRow && (column.IsRowAreaColumn || column.IsMeasureCaptionColumn))
				return null;
			if(column.IsMeasureCaptionColumn)
				return row.Measure.Name;
			if(row.Dimension != null)
				return GetDimensionValue(column.AxisPoint, row.Dimension);
			if(column.Dimension != null)
				return GetDimensionValue(row.AxisPoint, column.Dimension);
			return MDData.GetValue(column.AxisPoint, row.AxisPoint, row.Measure).Value;
		}
		protected override FormatSettings GetRowCellFormatting(ChartExportRow row, ChartExportColumn column) {
			if(column.IsMeasureCaptionColumn)
				return null;
			if(column.Dimension != null)
				return GetDimensionFormatSettings(row, column, column.Dimension.InternalDescriptor.Format);
			if(row.Dimension != null)
				return GetDimensionFormatSettings(row, column, row.Dimension.InternalDescriptor.Format);
			if(row.Measure != null)
				return GetMeasureFormatSettings(row, column, row.Measure.InternalDescriptor.Format.NumericFormat);
			return null;
		}
	}
}
