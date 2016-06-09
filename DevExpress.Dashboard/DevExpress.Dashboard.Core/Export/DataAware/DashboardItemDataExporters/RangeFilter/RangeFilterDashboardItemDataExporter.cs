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
using DevExpress.DashboardCommon.Printing;
using DevExpress.DashboardCommon.ViewerData;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.Export;
using DevExpress.XtraExport.Helpers;
using DevExpress.Utils;
namespace DevExpress.DashboardExport {
	public class RangeFilterDashboardItemDataExporter : DashboardItemDataExporter<RangeFilterExportColumn, RangeFilterExportRow> {
		RangeFilterDashboardItemViewModel RangeFilterViewModel { get { return (RangeFilterDashboardItemViewModel)ViewModel; } }
		public RangeFilterDashboardItemDataExporter(DashboardItemExportData data, IDataAwareExportOptions options)
			: base(data, options) {
			options.ShowColumnHeaders = DefaultBoolean.True;
		}
		protected override IList<RangeFilterExportColumn> CreateColumns() {
			List<RangeFilterExportColumn> columns = new List<RangeFilterExportColumn>();
			RangeFilterDashboardItemViewModel rangeFilter = RangeFilterViewModel;
			foreach(DimensionDescriptor dimension in MDData.GetDimensions(DashboardDataAxisNames.ChartSeriesAxis)) {
				columns.Add(new RangeFilterExportColumn() {
					Dimension = dimension,
					Header = dimension.Name,
					FormatSettings = DashboardItemDataFormatter.CreateValueFormatSettings(dimension.InternalDescriptor.Format)
				});
			}
			IList<AxisPoint> argumentPoints = MDData.GetAxisPointsByDimensionId(rangeFilter.Argument.SummaryArgumentMember);
			if(argumentPoints != null)
				foreach(AxisPoint argumentPoint in argumentPoints)
					foreach(ChartSeriesTemplateViewModel series in rangeFilter.SeriesTemplates) {
						MeasureDescriptor measure = MDData.GetMeasureDescriptorByID(series.DataMembers[0]);
						columns.Add(new RangeFilterExportColumn() {
							Measure = measure,
							AxisPoint = argumentPoint,
							Header = argumentPoint.DisplayText,
							FormatSettings = DashboardItemDataFormatter.CreateValueFormatSettings(measure.InternalDescriptor.Format)
						});
					}
			return columns;
		}
		protected override IList<RangeFilterExportRow> CreateRows() {
			List<RangeFilterExportRow> rows = new List<RangeFilterExportRow>();
			RangeFilterDashboardItemViewModel rangeFilter = RangeFilterViewModel;
			if(!string.IsNullOrEmpty(rangeFilter.Argument.SummaryArgumentMember) && rangeFilter.SeriesTemplates.Count > 1)
				rows.Add(new RangeFilterExportRow() { IsMeasureHeader = true });
			IList<AxisPoint> points = MDData.GetAxisPointsByDimensionId(rangeFilter.SummarySeriesMember);
			if(points != null)
				foreach(AxisPoint point in points)
					rows.Add(new RangeFilterExportRow() { AxisPoint = point });
			else
				rows.Add(new RangeFilterExportRow());
			return rows;
		}
		protected override object GetRowCellValue(RangeFilterExportRow row, RangeFilterExportColumn column) {
			if(row.IsMeasureHeader)
				return column.Measure != null ? column.Measure.Name : null;
			if(column.Dimension != null)
				return GetDimensionValue(row.AxisPoint, column.Dimension);
			if(column.Measure != null)
				return MDData.GetValue(row.AxisPoint, column.AxisPoint, column.Measure).Value;
			return MDData.GetValue(column.AxisPoint, column.Measure).Value;
		}
		protected override FormatSettings GetRowCellFormatting(RangeFilterExportRow row, RangeFilterExportColumn column) {
			if(row.IsMeasureHeader)
				return null;
			if(column.Dimension != null)
				return GetDimensionFormatSettings(row, column, column.Dimension.InternalDescriptor.Format);
			if(column.Measure != null)
				return GetMeasureFormatSettings(row, column, column.Measure.InternalDescriptor.Format.NumericFormat);
			return null;
		}
	}
}
