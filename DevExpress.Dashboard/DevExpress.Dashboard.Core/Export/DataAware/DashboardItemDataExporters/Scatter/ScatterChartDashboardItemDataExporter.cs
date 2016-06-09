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
using DevExpress.XtraExport.Helpers;
using DevExpress.Export;
using DevExpress.Utils;
namespace DevExpress.DashboardExport {
	public class ScatterChartDashboardItemDataExporter : DashboardItemDataExporter<ScatterChartExportColumn, ScatterChartExportRow> {
		ScatterChartDashboardItemViewModel ChartViewModel { get { return (ScatterChartDashboardItemViewModel)ViewModel; } }
		public ScatterChartDashboardItemDataExporter(DashboardItemExportData data, IDataAwareExportOptions options)
			: base(data, options) {
			options.ShowColumnHeaders = DefaultBoolean.True;
		}
		void AddMeasureColumn(IList<ScatterChartExportColumn> columns, string id) {
			if(!string.IsNullOrEmpty(id)) {
				MeasureDescriptor measure = MDData.GetMeasureDescriptorByID(id);
				columns.Add(new ScatterChartExportColumn() {
					Measure = measure,
					Header = measure.Name,
					FormatSettings = DashboardItemDataFormatter.CreateValueFormatSettings(measure.InternalDescriptor.Format)
				});
			}
		}
		protected override IList<ScatterChartExportColumn> CreateColumns() {
			List<ScatterChartExportColumn> columns = new List<ScatterChartExportColumn>();
			ScatterChartDashboardItemViewModel scatterChart = ChartViewModel;
			foreach(string id in scatterChart.ActualArgumentDataMembers) {
				if(!string.IsNullOrEmpty(id)) {
					DimensionDescriptor dimension = MDData.GetDimensionDescriptorByID(id);
					columns.Add(new ScatterChartExportColumn() {
						Dimension = dimension,
						Header = dimension.Name,
						FormatSettings = DashboardItemDataFormatter.CreateValueFormatSettings(dimension.InternalDescriptor.Format)
					});
				}
			}
			AddMeasureColumn(columns, scatterChart.AxisXDataMember);
			if(scatterChart.Panes.Count > 0) {
				IList<ChartSeriesTemplateViewModel> templates = scatterChart.Panes[0].SeriesTemplates;
				if(templates.Count > 0)
					foreach(string id in templates[0].DataMembers)
						AddMeasureColumn(columns, id);
			}
			return columns;
		}
		protected override IList<ScatterChartExportRow> CreateRows() {
			List<ScatterChartExportRow> rows = new List<ScatterChartExportRow>();
			DataAxis area = MDData.GetAxis(DashboardDataAxisNames.ChartArgumentAxis);
			foreach(AxisPoint point in area.GetPoints())
				rows.Add(new ScatterChartExportRow { AxisPoint = point });
			return rows;
		}
		protected override object GetRowCellValue(ScatterChartExportRow row, ScatterChartExportColumn column) {
			if(column.Dimension != null)
				return GetDimensionValue(row.AxisPoint, column.Dimension);
			if(column.Measure != null)
				return MDData.GetValue(row.AxisPoint, column.Measure).Value;
			return null;
		}
		protected override FormatSettings GetRowCellFormatting(ScatterChartExportRow row, ScatterChartExportColumn column) {
			if(column.Dimension != null)
				return GetDimensionFormatSettings(row, column, column.Dimension.InternalDescriptor.Format);
			if(column.Measure != null)
				return GetMeasureFormatSettings(row, column, column.Measure.InternalDescriptor.Format.NumericFormat);
			return null;
		}
	}
}
