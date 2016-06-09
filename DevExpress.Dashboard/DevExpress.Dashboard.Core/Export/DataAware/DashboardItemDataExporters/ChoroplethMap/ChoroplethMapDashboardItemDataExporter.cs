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
	public class ChoroplethMapDashboardItemDataExporter : DashboardItemDataExporter<ChoroplethMapExportColumn, ChoroplethMapExportRow> {
		ChoroplethMapDashboardItemViewModel MapViewModel { get { return (ChoroplethMapDashboardItemViewModel)ViewModel; } }
		public ChoroplethMapDashboardItemDataExporter(DashboardItemExportData data, IDataAwareExportOptions options)
			: base(data, options) {
			options.ShowColumnHeaders = DefaultBoolean.True;
		}
		protected override IList<ChoroplethMapExportColumn> CreateColumns() {
			List<ChoroplethMapExportColumn> columns = new List<ChoroplethMapExportColumn>();
			string attributeId = MapViewModel.AttributeDimensionId;
			if(!string.IsNullOrEmpty(attributeId)) {
				DimensionDescriptor dimension = MDData.GetDimensionDescriptorByID(attributeId);
				columns.Add(new ChoroplethMapExportColumn() {
					Dimension = dimension,
					Header = dimension.Name,
					FormatSettings = DashboardItemDataFormatter.CreateValueFormatSettings(dimension.InternalDescriptor.Format),
				});
			}
			MeasureDescriptorCollection measureDescriptors = MDData.GetMeasures();
			List<MeasureDescriptor> tooltipMeasures = new List<MeasureDescriptor>();
			for(int i = 0; i < measureDescriptors.Count; i++) {
				MeasureDescriptor measure = measureDescriptors[i];
				bool isTooltip = false;
				foreach(TooltipDataItemViewModel tooltipMeasure in MapViewModel.TooltipMeasures) {
					if(measure.ID == tooltipMeasure.DataId) {
						tooltipMeasures.Add(measure);
						isTooltip = true;
						break;
					}
				}
				if(!isTooltip)
					columns.Add(CreateColumn(measure));
			}
			DeltaDescriptorCollection deltaDescriptors = MDData.GetDeltas();
			for(int i = 0; i < deltaDescriptors.Count; i++) {
				DeltaDescriptor delta = deltaDescriptors[i];
				columns.Add(new ChoroplethMapExportColumn() {
					Delta = delta,
					Header = delta.Name,
					FormatSettings = DashboardItemDataFormatter.CreateNumericFormatSettings(delta.InternalDescriptor.DisplayFormat)
				});
			}
			foreach(MeasureDescriptor measure in tooltipMeasures)
				columns.Add(CreateColumn(measure));
			return columns;
		}
		ChoroplethMapExportColumn CreateColumn(MeasureDescriptor measure) {
			return new ChoroplethMapExportColumn() {
				Measure = measure,
				Header = measure.Name,
				FormatSettings = DashboardItemDataFormatter.CreateValueFormatSettings(measure.InternalDescriptor.Format)
			};
		}
		protected override IList<ChoroplethMapExportRow> CreateRows() {
			List<ChoroplethMapExportRow> rows = new List<ChoroplethMapExportRow>();
			if(!string.IsNullOrEmpty(MapViewModel.AttributeDimensionId)) {
				DataAxis area = MDData.GetAxis(DashboardDataAxisNames.DefaultAxis);
				foreach(AxisPoint point in area.GetPoints())
					rows.Add(new ChoroplethMapExportRow() { AxisPoint = point });
			}
			return rows;
		}
		protected override object GetRowCellValue(ChoroplethMapExportRow row, ChoroplethMapExportColumn column) {
			if(column.Dimension != null)
				return row.AxisPoint.GetDimensionValue(column.Dimension).Value;
			if(column.Delta != null)
				return MDData.GetDeltaValue(row.AxisPoint, column.Delta).DisplayValue.Value;
			return MDData.GetValue(row.AxisPoint, column.Measure).Value;
		}
		protected override FormatSettings GetRowCellFormatting(ChoroplethMapExportRow row, ChoroplethMapExportColumn column) {
			if(column.Dimension != null)
				return null;
			NumericFormatViewModel format;
			if(column.Measure != null) {
				format = MDData.GetMeasureDescriptorByID(column.Measure.ID).InternalDescriptor.Format.NumericFormat;
				return GetMeasureFormatSettings(row, column, format);
			}
			if(column.Delta != null) {
				format = MDData.GetDeltaDescriptorById(column.Delta.ID).InternalDescriptor.DisplayFormat;
				return GetMeasureFormatSettings(row, column, format);
			}
			return null;
		}
	}
}
