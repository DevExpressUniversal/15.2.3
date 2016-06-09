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
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Drawing.Design;
using System.Xml.Linq;
using DevExpress.DashboardCommon.Localization;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Compatibility.System.ComponentModel.Design.Serialization;
using DevExpress.Compatibility.System.Drawing.Design;
namespace DevExpress.DashboardCommon {
	public enum SimpleSeriesType { Bar, StackedBar, FullStackedBar, Point, Line, StackedLine, FullStackedLine, StepLine, Spline,
								   Area, StackedArea, FullStackedArea, StepArea, SplineArea, StackedSplineArea, FullStackedSplineArea }
	[
	DesignerSerializer(TypeNames.DataItemContainerCodeDomSerializer, TypeNames.CodeDomSerializer),
	TypeConverter(TypeNames.SimpleSeriesConverter)
	]
	public class SimpleSeries : ChartSeries {
		const string xmlSeriesType = "SeriesType";
		const string valueName = "Value";
		const SimpleSeriesType DefaultSeriesType = SimpleSeriesType.Bar;
		SimpleSeriesType seriesType;
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("SimpleSeriesValue"),
#endif
		Category(CategoryNames.Data),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Editor(TypeNames.ListSelectorEditor, typeof(UITypeEditor)),
		TypeConverter(TypeNames.CreatableMeasurePropertyTypeConverter)
		]
		public Measure Value { 
			get { return GetMeasure(valueName); } 
			set { SetDataItem(valueName, value); } 
		}
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("SimpleSeriesSeriesType"),
#endif
		Category(CategoryNames.General),
		TypeConverter(TypeNames.SimpleSeriesSeriesTypeConverter),
		DefaultValue(DefaultSeriesType)
		]
		public SimpleSeriesType SeriesType {
			get { return seriesType; }
			set {
				if (value != seriesType) {
					seriesType = value;
					OnChanged(ChangeReason.View);
				}
			}
		}
		internal String ValueDataMember { get { return GetDataItemActualID(Value); } }
		protected override bool ShowOnlyPercentValues { get { return SupportPercentFormatType(Value); } }
		protected internal override string SeriesImageName { get { return String.Format(BaseImagePath, seriesType); } }
		public SimpleSeries(Measure value, SimpleSeriesType seriesType)
			: base(new DataItemDescription[] { new DataItemDescription(valueName, DashboardLocalizer.GetString(DashboardStringId.ValueCaption), value) }) {
			this.seriesType = seriesType;
		}
		public SimpleSeries(Measure value)
			: this(value, DefaultSeriesType) {
		}
		public SimpleSeries(SimpleSeriesType seriesType)
			: this(null, seriesType) {
		}
		public SimpleSeries()
			: this(null, DefaultSeriesType) {
		}
		protected override DataItemContainer CreateInstance() {
			return new SimpleSeries();
		}
		protected internal override void Assign(DataItemContainer series) {
			base.Assign(series);
			SimpleSeries simpleSeries = series as SimpleSeries;
			if (simpleSeries != null) {
				seriesType = simpleSeries.seriesType;
				Value = simpleSeries.Value;
			}
		}
		protected internal override ChartSeriesTemplateViewModel CreateSeriesTemplateViewModel() {
			ChartSeriesViewModelType viewModelSeriesType;
			switch(seriesType) {
				case SimpleSeriesType.StackedBar:
					viewModelSeriesType = ChartSeriesViewModelType.StackedBar;
					break;
				case SimpleSeriesType.FullStackedBar:
					viewModelSeriesType = ChartSeriesViewModelType.FullStackedBar;
					break;
				case SimpleSeriesType.Point:
					viewModelSeriesType = ChartSeriesViewModelType.Point;
					break;
				case SimpleSeriesType.Line:
					viewModelSeriesType = ChartSeriesViewModelType.Line;
					break;
				case SimpleSeriesType.StackedLine:
					viewModelSeriesType = ChartSeriesViewModelType.StackedLine;
					break;
				case SimpleSeriesType.FullStackedLine:
					viewModelSeriesType = ChartSeriesViewModelType.FullStackedLine;
					break;
				case SimpleSeriesType.StepLine:
					viewModelSeriesType = ChartSeriesViewModelType.StepLine;
					break;
				case SimpleSeriesType.Spline:
					viewModelSeriesType = ChartSeriesViewModelType.Spline;
					break;
				case SimpleSeriesType.Area:
					viewModelSeriesType = ChartSeriesViewModelType.Area;
					break;
				case SimpleSeriesType.StackedArea:
					viewModelSeriesType = ChartSeriesViewModelType.StackedArea;
					break;
				case SimpleSeriesType.FullStackedArea:
					viewModelSeriesType = ChartSeriesViewModelType.FullStackedArea;
					break;
				case SimpleSeriesType.StepArea:
					viewModelSeriesType = ChartSeriesViewModelType.StepArea;
					break;
				case SimpleSeriesType.SplineArea:
					viewModelSeriesType = ChartSeriesViewModelType.SplineArea;
					break;
				case SimpleSeriesType.StackedSplineArea:
					viewModelSeriesType = ChartSeriesViewModelType.StackedSplineArea;
					break;
				case SimpleSeriesType.FullStackedSplineArea:
					viewModelSeriesType = ChartSeriesViewModelType.FullStackedSplineArea;
					break;
				default:
					viewModelSeriesType = ChartSeriesViewModelType.Bar;
					break;
			}
			ChartSeriesTemplateViewModel viewModel = new ChartSeriesTemplateViewModel {
				SeriesType = viewModelSeriesType,
				DataMembers = new string[] { ValueDataMember }
			};
			UpdateViewModel(viewModel);
			return viewModel;
		}
		protected internal override void SaveToXml(XElement element) {
			base.SaveToXml(element);
			if(SeriesType != DefaultSeriesType)
				element.Add(new XAttribute(xmlSeriesType, seriesType));
		}
		protected internal override void LoadFromXml(XElement element) {
			base.LoadFromXml(element);
			string seriesTypeAttr = XmlHelper.GetAttributeValue(element, xmlSeriesType);
			if (!String.IsNullOrEmpty(seriesTypeAttr))
				seriesType = XmlHelper.EnumFromString<SimpleSeriesType>(seriesTypeAttr);
		}
	}
}
