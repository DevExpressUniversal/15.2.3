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
using DevExpress.Utils;
using DevExpress.XtraExport.Helpers;
namespace DevExpress.DashboardExport {
	public abstract class KpiDashboardItemDataExporter<TColumn, TRow, TElement> : DashboardItemDataExporter<TColumn, TRow>
		where TColumn : KpiExportColumn, new()
		where TRow : KpiExportRow, new()
		where TElement : KpiElementViewModel {
		static TColumn CreateDeltaColumn(DeltaDescriptor delta, DeltaValueType valueType) {
			return new TColumn() {
				DeltaValueType = valueType,
				Header = string.Format("{0} {1}", delta.Name, valueType),
				Delta = delta,
				FormatSettings = DashboardItemDataFormatter.CreateDeltaFormatSettings(delta, valueType)
			};
		}
		KpiDashboardItemViewModel KpiViewModel { get { return (KpiDashboardItemViewModel)ViewModel; } }
		bool UseMeasuresAsArguments { get { return string.IsNullOrEmpty(KpiViewModel.SeriesAxisName); } }
		protected KpiDashboardItemDataExporter(DashboardItemExportData data, IDataAwareExportOptions options)
			: base(data, options) {
			options.ShowColumnHeaders = DefaultBoolean.True;
		}
		void AddColumn(IList<TColumn> columns, string id) {
			if(!string.IsNullOrEmpty(id)) {
				MeasureDescriptor measure = MDData.GetMeasureDescriptorByID(id);
				TColumn column = new TColumn() {
					Header = measure.Name,
					Measure = measure,
					FormatSettings = DashboardItemDataFormatter.CreateValueFormatSettings(measure.InternalDescriptor.Format)
				};
				columns.Add(column);
			}
		}
		protected virtual void AddColumnsByElement(IList<TColumn> columns, TElement element) {
			if(element.DataItemType == KpiElementDataItemType.Delta) {
				DeltaDescriptor delta = MDData.GetDeltaDescriptorById(element.ID);
				AddColumn(columns, delta.ActualMeasureID);
				AddColumn(columns, delta.TargetMeasureID);
				columns.Add(CreateDeltaColumn(delta, DeltaValueType.AbsoluteVariation));
				columns.Add(CreateDeltaColumn(delta, DeltaValueType.PercentOfTarget));
				columns.Add(CreateDeltaColumn(delta, DeltaValueType.PercentVariation));
			}
			else
				AddColumn(columns, element.ID);
		}
		protected abstract IEnumerable<TElement> GetElements();
		protected abstract TElement GetElement();
		protected override IList<TColumn> CreateColumns() {
			List<TColumn> columns = new List<TColumn>();
			if(UseMeasuresAsArguments)
				foreach(TElement element in GetElements())
					AddColumnsByElement(columns, element);
			else {
				foreach(DimensionDescriptor dimension in MDData.GetDimensions(DashboardDataAxisNames.DefaultAxis))
					columns.Add(new TColumn() {
						Header = dimension.Name,
						Dimension = dimension,
						FormatSettings = DashboardItemDataFormatter.CreateValueFormatSettings(dimension.InternalDescriptor.Format)
					});
				AddColumnsByElement(columns, GetElement());
			}
			return columns;
		}
		protected override IList<TRow> CreateRows() {
			List<TRow> rows = new List<TRow>();
			if(UseMeasuresAsArguments)
				rows.Add(new TRow());
			else {
				DataAxis axis = MDData.GetAxis(DashboardDataAxisNames.DefaultAxis);
				foreach(AxisPoint point in axis.GetPoints())
					rows.Add(new TRow() { AxisPoint = point });
			}
			return rows;
		}
		protected override object GetRowCellValue(TRow row, TColumn column) {
			DimensionDescriptor dimension = column.Dimension;
			DeltaDescriptor delta = column.Delta;
			if(dimension != null)
				return row.AxisPoint.GetDimensionValue(dimension).Value;
			else if(delta != null) {
				DeltaValue deltaValue = MDData.GetDeltaValue(row.AxisPoint, delta);
				switch(column.DeltaValueType) {
					case DeltaValueType.AbsoluteVariation:
						return deltaValue.AbsoluteVariation.Value;
					case DeltaValueType.PercentOfTarget:
						return deltaValue.PercentOfTarget.Value;
					case DeltaValueType.PercentVariation:
						return deltaValue.PercentVariation.Value;
					default:
						return deltaValue.ActualValue.Value;
				}
			}
			else
				return MDData.GetValue(row.AxisPoint, column.Measure).Value;
		}
		protected override FormatSettings GetRowCellFormatting(TRow row, TColumn column) {
			if(column.ColEditType == ColumnEditTypes.Sparkline)
				return null;
			if(column.Dimension != null)
				return GetDimensionFormatSettings(row, column, column.Dimension.InternalDescriptor.Format);
			if(column.Delta != null) {
				NumericFormatViewModel format;
				if(column.DeltaValueType == DeltaValueType.ActualValue)
					format = column.Delta.InternalDescriptor.ActualValueFormat;
				else
					format = DashboardItemDataFormatter.GetDeltaFormat(column.Delta, column.DeltaValueType);
				return GetMeasureFormatSettings(row, column, format);
			}
			if(column.Measure != null) {
				NumericFormatViewModel format = column.Measure.InternalDescriptor.Format.NumericFormat;
				return GetMeasureFormatSettings(row, column, format);
			}
			return null;
		}
	}
}
