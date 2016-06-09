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
namespace DevExpress.DashboardExport {
	public class PieDashboardItemDataExporter : DashboardItemDataExporter<PieExportColumn, PieExportRow> {
		PieDashboardItemViewModel PieViewModel { get { return (PieDashboardItemViewModel)ViewModel; } }
		public PieDashboardItemDataExporter(DashboardItemExportData data, IDataAwareExportOptions options)
			: base(data, options) {
			PieDashboardItemViewModel pie = PieViewModel;
			if(pie.ActualArgumentDataMembers.Length != 0 && pie.ActualSeriesDataMembers.Length != 0)
				options.ShowColumnHeaders = Utils.DefaultBoolean.False;
			else
				options.ShowColumnHeaders = Utils.DefaultBoolean.True;
		}
		void AddColumns(IList<PieExportColumn> columns, string[] dataMembers) {
			foreach(string id in dataMembers) {
				DimensionDescriptor argument = MDData.GetDimensionDescriptorByID(id);
				columns.Add(new PieExportColumn() {
					Dimension = argument,
					Header = argument.Name,
					FormatSettings = DashboardItemDataFormatter.CreateValueFormatSettings(argument.InternalDescriptor.Format)
				});
			}
		}
		protected override IList<PieExportColumn> CreateColumns() {
			List<PieExportColumn> columns = new List<PieExportColumn>();
			PieDashboardItemViewModel pie = PieViewModel;
			if(pie.ActualArgumentDataMembers.Length != 0 && pie.ActualSeriesDataMembers.Length != 0) {
				foreach(string id in pie.ActualSeriesDataMembers)
					columns.Add(new PieExportColumn() {
						IsRowAreaColumn = true,
						Dimension = MDData.GetDimensionDescriptorByID(id),
						FormatSettings = new FormatSettings()
					});
				MeasureDescriptor measure = MDData.GetMeasureDescriptorByID(pie.ValueDataMembers[0]);
				foreach(AxisPoint point in MDData.GetAxis(DashboardDataAxisNames.ChartArgumentAxis).GetPoints())
					columns.Add(new PieExportColumn() { 
						AxisPoint = point,
						Measure = measure,
						FormatSettings = DashboardItemDataFormatter.CreateValueFormatSettings(measure.InternalDescriptor.Format)
					});
			}
			else {
				if(pie.ActualArgumentDataMembers.Length != 0)
					AddColumns(columns, pie.ActualArgumentDataMembers);
				if(pie.ActualSeriesDataMembers.Length != 0)
					AddColumns(columns, pie.ActualSeriesDataMembers);
				foreach(string valueId in pie.ValueDataMembers) {
					MeasureDescriptor measure = MDData.GetMeasureDescriptorByID(valueId);
					columns.Add(new PieExportColumn() {
						Measure = measure,
						Header = measure.Name,
						FormatSettings = DashboardItemDataFormatter.CreateValueFormatSettings(measure.InternalDescriptor.Format)
					});
				}
			}
			return columns;
		}
		void AddRows(List<PieExportRow> rows, string axisName) {
			foreach(AxisPoint point in MDData.GetAxis(axisName).GetPoints())
				rows.Add(new PieExportRow() { AxisPoint = point });
		}
		protected override IList<PieExportRow> CreateRows() {
			List<PieExportRow> rows = new List<PieExportRow>();
			PieDashboardItemViewModel pie = PieViewModel;
			if(pie.ActualArgumentDataMembers.Length != 0 && pie.ActualSeriesDataMembers.Length != 0) {
				foreach(string id in pie.ActualArgumentDataMembers)
					rows.Add(new PieExportRow() {
						IsColumnAreaRow = true,
						Dimension = MDData.GetDimensionDescriptorByID(id)
					});
				AddRows(rows, DashboardDataAxisNames.ChartSeriesAxis);
			}
			else if(pie.ActualArgumentDataMembers.Length != 0)
				AddRows(rows, DashboardDataAxisNames.ChartArgumentAxis);
			else if(pie.ActualSeriesDataMembers.Length != 0)
				AddRows(rows, DashboardDataAxisNames.ChartSeriesAxis);
			else
				rows.Add(new PieExportRow());
			return rows;
		}
		protected override object GetRowCellValue(PieExportRow row, PieExportColumn column) {
			if(row.IsColumnAreaRow && column.IsRowAreaColumn)
				return null;
			if(row.Dimension != null)
				return GetDimensionValue(column.AxisPoint, row.Dimension);
			if(column.Dimension != null)
				return GetDimensionValue(row.AxisPoint, column.Dimension);
			return MDData.GetValue(column.AxisPoint, row.AxisPoint, column.Measure).Value;
		}
		protected override FormatSettings GetRowCellFormatting(PieExportRow row, PieExportColumn column) {
			if(row.IsColumnAreaRow || column.IsRowAreaColumn)
				return null;
			if(column.Dimension != null)
				return GetDimensionFormatSettings(row, column, column.Dimension.InternalDescriptor.Format);
			if(column.Measure != null)
				return GetMeasureFormatSettings(row, column, column.Measure.InternalDescriptor.Format.NumericFormat);
			return null;
		}
	}
}
