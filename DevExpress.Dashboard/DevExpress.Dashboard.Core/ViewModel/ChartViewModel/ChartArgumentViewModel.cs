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
using DevExpress.DashboardCommon.Native;
namespace DevExpress.DashboardCommon.ViewModel {
	public enum ChartArgumentType { String, DateTime, Integer, Float, Double, Decimal }
	public static class ChartArgumentTypeExtensions {
		public static ChartArgumentType ToChartArgumentType(this DataFieldType type) {
			switch(type) {
				case DataFieldType.DateTime:
					return ChartArgumentType.DateTime;
				case DataFieldType.Decimal:
					return ChartArgumentType.Decimal;
				case DataFieldType.Double:
					return ChartArgumentType.Double;
				case DataFieldType.Float:
					return ChartArgumentType.Float;
				case DataFieldType.Integer:
					return ChartArgumentType.Integer;
				case DataFieldType.Text:
				case DataFieldType.Unknown:
				case DataFieldType.Enum:
				case DataFieldType.Bool:
				case DataFieldType.Custom:
					return ChartArgumentType.String;
				default:
					throw new ArgumentException(type.ToString());
			}
		}
		public static bool IsNumeric(this ChartArgumentType type) {
			switch(type) {
				case ChartArgumentType.Integer:
				case ChartArgumentType.Float:
				case ChartArgumentType.Double:
				case ChartArgumentType.Decimal:
					return true;
				case ChartArgumentType.DateTime:
				case ChartArgumentType.String:
				default:
					return false;
			}			
		}
	}
	public class ChartArgumentComponentViewModel {
		string dataMember;
		ValueFormatViewModel format;
		public string DataMember {
			get { return dataMember; }
			set { dataMember = value; }
		}
		public ValueFormatViewModel Format {
			get { return format; }
			set { format = value; }
		}
		public ChartArgumentComponentViewModel() {
			format = new ValueFormatViewModel();
		}
		public ChartArgumentComponentViewModel(string dataMember, ValueFormatViewModel format) {
			this.dataMember = dataMember;
			this.format = format;
		}
	}
	public interface IArgumentsDashboardItem {
		IList<Dimension> Arguments { get; }
	}
	public class ChartArgumentViewModel {
		static DataFieldType GetArgumentType(IDataSourceSchema schema, IArgumentsDashboardItem item) {
			if(item.Arguments == null || item.Arguments.Count != 1 || schema == null || schema.GetIsOlap())
				return DataFieldType.Text;
			Dimension dimension = item.Arguments[0];
			if(schema != null && dimension != null && !dimension.TopNOptions.Enabled) {
				Type dataFieldType = schema.GetFieldSourceType(dimension.DataMember);
				GroupIntervalInfo groupIntervalInfo = GroupIntervalInfo.GetInstance(dimension, DataBindingHelper.GetDataFieldType(dataFieldType));
				if(groupIntervalInfo != null)
					if(groupIntervalInfo.ChartDataType != null)
						return DataBindingHelper.GetDataFieldType(groupIntervalInfo.ChartDataType);
					else
						return DataSourceHelper.GetActualDataType(dimension, groupIntervalInfo, dataFieldType);
			}
			return DataFieldType.Text;
		}
		readonly ValueFormatViewModel axisXLabelFormat;
		ValueDataType dataType = ValueDataType.String;
		DateTimePresentationUnit dateTimePresentationUnit = DateTimePresentationUnit.Day;
		public ChartArgumentType Type { get; set; }
		public string SummaryArgumentMember { get; set; }
		public ValueFormatViewModel AxisXLabelFormat { get { return axisXLabelFormat; } }
		public ValueDataType DataType { 
			get { return dataType; }
			set { dataType = value; }
		}
		public DateTimePresentationUnit DateTimePresentationUnit {
			get { return dateTimePresentationUnit; }
			set { dateTimePresentationUnit = value; }
		}
		public bool IsOrderedDiscrete { get; set; }
		public bool HasArguments { get; set; }
		public bool IsContinuousDateTimeScale { get; set; }
		public ChartArgumentViewModel() {
		}
		public ChartArgumentViewModel(DataDashboardItem dashboardItem, bool hasArguments) {
			HasArguments = hasArguments;
			IArgumentsDashboardItem item = dashboardItem as IArgumentsDashboardItem;
			IList<Dimension> arguments = item == null ? new Dimension[0] : item.Arguments;
			PieDashboardItem pie = dashboardItem as PieDashboardItem;
			GroupIntervalInfo info = null;
			if(arguments.Count == 1)
				info = GetGroupIntervalInfo(arguments[0], dashboardItem);
			IsOrderedDiscrete = info != null && info.IsDiscreteDate && arguments[0].SortByMeasure == null;
			DataFieldType chartArgumentType = dashboardItem.DataSource == null ? DataFieldType.Text : GetArgumentType(dashboardItem.DataSource.GetDataSourceSchema(dashboardItem.DataMember), item);
			if(pie != null && pie.ProvideValuesAsArguments)
				chartArgumentType = DataFieldType.Text;
			Type = chartArgumentType.ToChartArgumentType();
			if(Type == ChartArgumentType.String)
				dataType = ValueDataType.String;
			else {
				if(Type == ChartArgumentType.DateTime && info != null) {
					dataType = ValueDataType.DateTime;
					DateTimeGroupIntervalInfo dateTimeGroupIntervalInfo = info as DateTimeGroupIntervalInfo;
					if(dateTimeGroupIntervalInfo != null) {
						DiscontinuousDateTimeGroupIntervalInfo discontinuousDateTimeGroupIntervalInfo = dateTimeGroupIntervalInfo as DiscontinuousDateTimeGroupIntervalInfo;
						if(discontinuousDateTimeGroupIntervalInfo != null) {
							dateTimePresentationUnit = discontinuousDateTimeGroupIntervalInfo.Unit;
							IsContinuousDateTimeScale = false;
						}
						else {
							IsContinuousDateTimeScale = true;
						}
					}
				}
				else
					dataType = ValueDataType.Numeric;
			}
			if(pie == null) {
				if(Type == ChartArgumentType.DateTime)
					axisXLabelFormat = new ValueFormatViewModel(new DateTimeFormatViewModel(arguments[0].DateTimeFormat));
				else if(Type.IsNumeric()) {
					NumericFormatViewModel model = GetAxisXLabelNumericFormat(arguments);
					if(model != null)
						axisXLabelFormat = new ValueFormatViewModel(model);
				}
			}
		}
		public ChartArgumentViewModel(ScatterChartDashboardItem dashboardItem) {
			DataFieldType chartArgumentType = DataFieldType.Text;
			if(dashboardItem.DataSource != null && dashboardItem.AxisXMeasure != null) {
				IDataSourceSchema schema = dashboardItem.DataSource.GetDataSourceSchema(dashboardItem.DataMember);
				Type fieldType = schema.GetFieldSourceType(dashboardItem.AxisXMeasure.DataMember);
				chartArgumentType = DataBindingHelper.GetDataFieldType(fieldType);
			}
			Type = chartArgumentType.ToChartArgumentType();
			dataType = Type == ChartArgumentType.String ? ValueDataType.String : ValueDataType.Numeric;
		}
		protected virtual NumericFormatViewModel GetAxisXLabelNumericFormat(IList<Dimension> arguments) {
			return null;
		}
		GroupIntervalInfo GetGroupIntervalInfo(Dimension dimension, DataDashboardItem item) {
			if(item.DataSource == null)
				return null;
			IDataSourceSchema schema = item.DataSource.GetDataSourceSchema(item.DataMember);
			if(schema == null)
				return null;
			return GroupIntervalInfo.GetInstance(dimension, schema.GetFieldType(dimension.DataMember));
		}
	}
	public class RangeFilterArgumentViewModel : ChartArgumentViewModel {
		public RangeFilterArgumentViewModel()
			: base() {
		}
		public RangeFilterArgumentViewModel(SeriesDashboardItem dashboardItem, bool hasArguments)
			: base(dashboardItem, hasArguments) {
		}
		protected override NumericFormatViewModel GetAxisXLabelNumericFormat(IList<Dimension> arguments) {
			return arguments[0].CreateNumericFormatViewModel();
		}
	}
	public class ChartLegendViewModel {
		ChartLegendOutsidePosition outsidePosition;
		ChartLegendInsidePosition insidePosition;
		bool isInsideDiagram;
		bool visible;
		public bool IsInsideDiagram {
			get { return isInsideDiagram; }
			set { isInsideDiagram = value; }
		}
		public ChartLegendOutsidePosition OutsidePosition {
			get { return outsidePosition; }
			set { outsidePosition = value; }
		}
		public bool Visible {
			get { return visible; }
			set { visible = value; }
		}
		public ChartLegendInsidePosition InsidePosition {
			get { return insidePosition; }
			set { insidePosition = value; }
		}
		public ChartLegendViewModel() {
		}
		public ChartLegendViewModel(ChartLegend legend) {
			visible = legend.Visible;
			isInsideDiagram = legend.IsInsideDiagram;
			insidePosition = legend.InsidePosition;
			outsidePosition = legend.OutsidePosition;
		}
	}
}
