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
	public abstract class GeoPointMapDashboardItemDataExporterBase : DashboardItemDataExporter<GeoPointMapExportColumn, GeoPointMapExportRow> {
		GeoPointMapDashboardItemViewModelBase MapViewModel { get { return (GeoPointMapDashboardItemViewModelBase)ViewModel; } }
		public GeoPointMapDashboardItemDataExporterBase(DashboardItemExportData data, IDataAwareExportOptions options)
			: base(data, options) {
			options.ShowColumnHeaders = DefaultBoolean.True;
			options.AllowGrouping = DefaultBoolean.True;
		}
		protected void AddDimensionColumn(IList<GeoPointMapExportColumn> columns, string id, bool useDefaultFormat) {
			if(!string.IsNullOrEmpty(id)) {
				DimensionDescriptor dimension = MDData.GetDimensionDescriptorByID(id);
				columns.Add(new GeoPointMapExportColumn() {
					Dimension = dimension,
					Header = dimension.Name,
					UseDefaultSettings = useDefaultFormat,
					FormatSettings = useDefaultFormat ? new FormatSettings() : DashboardItemDataFormatter.CreateValueFormatSettings(dimension.InternalDescriptor.Format)
				});
			}
		}
		protected void AddMeasureColumn(IList<GeoPointMapExportColumn> columns, string id) {
			if(!string.IsNullOrEmpty(id)) {
				MeasureDescriptor measure = MDData.GetMeasureDescriptorByID(id);
				columns.Add(new GeoPointMapExportColumn() {
					Measure = measure,
					Header = id == MapViewModel.PointsCountDataId ? "Count" : measure.Name,
					FormatSettings = DashboardItemDataFormatter.CreateValueFormatSettings(measure.InternalDescriptor.Format)
				});
			}
		}
		protected virtual void AddDimensionColumn(IList<GeoPointMapExportColumn> columns) { }
		protected abstract void AddMeasureColumns(IList<GeoPointMapExportColumn> columns);
		protected override IList<GeoPointMapExportColumn> CreateColumns() {
			List<GeoPointMapExportColumn> columns = new List<GeoPointMapExportColumn>();
			GeoPointMapDashboardItemViewModelBase map = MapViewModel;
			AddDimensionColumn(columns, map.LatitudeDataId, true);
			AddDimensionColumn(columns, map.LongitudeDataId, true);
			if(map.EnableClustering)
				AddMeasureColumn(columns, map.PointsCountDataId);
			AddDimensionColumn(columns);
			foreach(TooltipDataItemViewModel tooltipDimension in map.TooltipDimensions)
				AddDimensionColumn(columns, tooltipDimension.DataId, false);
			AddMeasureColumns(columns);
			foreach(TooltipDataItemViewModel tooltipMeasure in map.TooltipMeasures)
				AddMeasureColumn(columns, tooltipMeasure.DataId);
			return columns;
		}
		protected override IList<GeoPointMapExportRow> CreateRows() {
			List<GeoPointMapExportRow> rows = new List<GeoPointMapExportRow>();
			DataAxis area = MDData.GetAxis(DashboardDataAxisNames.DefaultAxis);
			IList<AxisPoint> points = area.GetPoints();
			GeoPointMapDashboardItemViewModelBase map = MapViewModel;
			if(!string.IsNullOrEmpty(map.LongitudeDataId))
				foreach(AxisPoint longitudePoint in area.GetPointsByDimension(MDData.GetDimensionDescriptorByID(map.LongitudeDataId))) {
					List<AxisPoint> childs = longitudePoint.GetAxisPointsByDimension(points.Count > 0 ? points[0].Dimension : null, false);
					if(childs.Count > 1) {
						GeoPointMapGroupRow groupRow = new GeoPointMapGroupRow() { LongitudeAxisPoint = longitudePoint };
						groupRow.Rows.AddRange(CreateRows(childs, longitudePoint, 1));
						rows.Add(groupRow);
					}
					else
						rows.AddRange(CreateRows(childs, longitudePoint, 0));
				}
			return rows;
		}
		IEnumerable<GeoPointMapExportRow> CreateRows(IList<AxisPoint> axisPoints, AxisPoint longitudeAxisPoint, int level) {
			foreach(AxisPoint point in axisPoints)
				yield return new GeoPointMapExportRow() {
					AxisPoint = point,
					LongitudeAxisPoint = longitudeAxisPoint,
					Level = level
				};
		}
		protected override object GetRowCellValue(GeoPointMapExportRow row, GeoPointMapExportColumn column) {
			if(column.Dimension != null && (!column.UseDefaultSettings || row.Level == 0)) {
				if(row.IsGroupRow)
					return column.UseDefaultSettings ? GetDimensionValue(row.LongitudeAxisPoint, column.Dimension) : null;
				return GetDimensionValue(row.AxisPoint, column.Dimension);
			}
			if(column.Measure != null) {
				if(row.Level == 0 && (row.IsGroupRow || column.Measure.ID == MapViewModel.PointsCountDataId))
					return MDData.GetValue(row.LongitudeAxisPoint, column.Measure).Value;
				return MDData.GetValue(row.AxisPoint, column.Measure).Value;
			}
			return null;
		}
		protected override FormatSettings GetRowCellFormatting(GeoPointMapExportRow row, GeoPointMapExportColumn column) {
			DimensionDescriptor dimension = column.Dimension;
			if(dimension != null && !column.UseDefaultSettings)
				return GetDimensionFormatSettings(row, column, column.Dimension.InternalDescriptor.Format);
			if(column.Measure != null)
				return GetMeasureFormatSettings(row, column, column.Measure.InternalDescriptor.Format.NumericFormat);
			return null;
		}
	}
}
