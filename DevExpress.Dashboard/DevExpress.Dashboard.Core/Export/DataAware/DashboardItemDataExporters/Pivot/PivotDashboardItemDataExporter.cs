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
using DevExpress.DashboardCommon.Localization;
using DevExpress.DashboardCommon.Printing;
using DevExpress.DashboardCommon.ViewerData;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.Export;
using DevExpress.XtraExport.Helpers;
namespace DevExpress.DashboardExport {
	public class PivotDashboardItemDataExporter : DashboardItemDataExporter<PivotExportColumn, PivotExportRow> {
		static string GetPivotTotalText(string displayText) {
			return string.Format(DashboardLocalizer.GetString(DashboardStringId.PivotGridTotal), displayText);
		}
		static string GetPivotGrandTotalText() {
			return DashboardLocalizer.GetString(DashboardStringId.PivotGridGrandTotal);
		}
		readonly PivotExportColumnCreator columnCreator;
		readonly PivotExportRowCreator rowCreator;
		PivotDashboardItemViewModel PivotViewModel { get { return (PivotDashboardItemViewModel)ViewModel; } }
		public PivotDashboardItemDataExporter(DashboardItemExportData data, IDataAwareExportOptions options)
			: base(data, options) {
			columnCreator = new PivotExportColumnCreator(MDData, PivotViewModel);
			rowCreator = new PivotExportRowCreator(MDData, PivotViewModel);
		}
		protected override IList<PivotExportColumn> CreateColumns() {
			return columnCreator.Items;
		}
		protected override IList<PivotExportRow> CreateRows() {
			return rowCreator.Items;
		}
		object GetAreaValue(AxisPoint point, DimensionDescriptor dimension, int index, bool isTotal) {
			int axisPointLevel = point.Level.LevelNumber;
			if(isTotal && axisPointLevel <= index)
				return GetPivotTotalText(point.DisplayText);
			return axisPointLevel < index ? point.Value : GetDimensionValue(point, dimension);
		}
		protected override object GetRowCellValue(PivotExportRow row, PivotExportColumn column) {
			if(row.IsColumnAreaRow && column.IsRowAreaColumn)
				return null;
			AxisPoint columnAxisPoint = column.AxisPoint;
			AxisPoint rowAxisPoint = row.AxisPoint;
			if(row.IsColumnAreaRow) {
				if(row.IsColumnDataAreaRow) {
					string name = column.Measure.Name;
					return row.IsSingleTotalRow ? GetPivotTotalText(name) : name;
				}
				return column.IsGrandTotalColumn ? GetPivotGrandTotalText() : GetAreaValue(columnAxisPoint, row.Dimension, row.LogicalPosition, column.IsTotalColumn);
			}
			if(column.IsRowAreaColumn) {
				if(column.IsSingleTotalColumn)
					return GetPivotTotalText(column.Measure.Name);
				return row.IsGrandTotalRow ? GetPivotGrandTotalText() : GetAreaValue(rowAxisPoint, column.Dimension, column.LogicalPosition, row.IsTotalRow);
			}
			return MDData.GetValue(columnAxisPoint, rowAxisPoint, column.Measure).Value;
		}
		protected override FormatSettings GetRowCellFormatting(PivotExportRow row, PivotExportColumn column) {
			if(row.IsDataRow && column.IsDataColumn)
				return GetMeasureFormatSettings(row, column, column.Measure.InternalDescriptor.Format.NumericFormat);
			if(row.IsGrandTotalRow || row.IsTotalRow || column.IsGrandTotalColumn || column.IsTotalColumn)
				return null;
			if(row.IsColumnAreaRow && !row.IsColumnDataAreaRow)
				return GetDimensionFormatSettings(row, column, row.Dimension.InternalDescriptor.Format);
			if(column.IsRowAreaColumn && !column.IsSingleTotalColumn)
				return GetDimensionFormatSettings(row, column, column.Dimension.InternalDescriptor.Format);
			return null;
		}
	}
}
