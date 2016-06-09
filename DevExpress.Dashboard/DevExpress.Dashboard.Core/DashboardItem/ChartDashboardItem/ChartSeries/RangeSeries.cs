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
	public enum RangeSeriesType { SideBySideRangeBar, RangeArea }
	[
	DesignerSerializer(TypeNames.DataItemContainerCodeDomSerializer, TypeNames.CodeDomSerializer)
	]
	public class RangeSeries : ChartSeries {
		const string value1Name = "Value1";
		const string value2Name = "Value2";
		const string xmlSeriesType = "SeriesType";
		const RangeSeriesType DefaultSeriesType = RangeSeriesType.SideBySideRangeBar;
		RangeSeriesType seriesType;
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("RangeSeriesValue1"),
#endif
		Category(CategoryNames.Data),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Editor(TypeNames.ListSelectorEditor, typeof(UITypeEditor)),
		TypeConverter(TypeNames.CreatableMeasurePropertyTypeConverter)
		]
		public Measure Value1 {
			get { return GetMeasure(value1Name); }
			set { SetDataItem(value1Name, value); }
		}
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("RangeSeriesValue2"),
#endif
		Category(CategoryNames.Data),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Editor(TypeNames.ListSelectorEditor, typeof(UITypeEditor)),
		TypeConverter(TypeNames.CreatableMeasurePropertyTypeConverter)
		]
		public Measure Value2 {
			get { return GetMeasure(value2Name); }
			set { SetDataItem(value2Name, value); }
		}
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("RangeSeriesSeriesType"),
#endif
		Category(CategoryNames.Data),
		DefaultValue(DefaultSeriesType)
		]
		public RangeSeriesType SeriesType {
			get { return seriesType; }
			set {
				if(value != seriesType) {
					seriesType = value;
					OnChanged(ChangeReason.View);
				}
			}
		}
		internal String Value1DataMember { get { return GetDataItemActualID(Value1 ?? DefaultMeasure); } }
		internal String Value2DataMember { get { return GetDataItemActualID(Value2 ?? DefaultMeasure); } }
		protected override bool ShowOnlyPercentValues { get { return SupportPercentFormatType(Value1) && SupportPercentFormatType(Value2); } }
		protected internal override string SeriesImageName { get { return String.Format(BaseImagePath, seriesType); } }
		Measure DefaultMeasure { get { return Value1 ?? Value2; } }
		public RangeSeries(Measure value1, Measure value2, RangeSeriesType seriesType)
			: base(new DataItemDescription[] { 
				new DataItemDescription(value1Name, DashboardLocalizer.GetString(DashboardStringId.Value1Caption), value1), 
				new DataItemDescription(value2Name, DashboardLocalizer.GetString(DashboardStringId.Value2Caption), value2) }) {
			this.seriesType = seriesType;
		}
		public RangeSeries(Measure value1, Measure value2)
			: this(value1, value2, DefaultSeriesType) {
		}
		public RangeSeries(RangeSeriesType seriesType)
			: this(null, null, seriesType) {
		}
		public RangeSeries()
			: this(null, null, DefaultSeriesType) {
		}
		protected override DataItemContainer CreateInstance() {
			return new RangeSeries();
		}
		protected internal override void Assign(DataItemContainer series) {
			base.Assign(series);
			RangeSeries rangeSeries = series as RangeSeries;
			if (rangeSeries != null) {
				seriesType = rangeSeries.seriesType;
				Value1 = rangeSeries.Value1;
				Value2 = rangeSeries.Value2;
			}
		}
		protected internal override ChartSeriesTemplateViewModel CreateSeriesTemplateViewModel() {
			ChartSeriesViewModelType viewModelSeriesType = seriesType == RangeSeriesType.SideBySideRangeBar ? ChartSeriesViewModelType.SideBySideRangeBar : ChartSeriesViewModelType.RangeArea;
			ChartSeriesTemplateViewModel viewModel = new ChartSeriesTemplateViewModel {
				SeriesType = viewModelSeriesType,
				DataMembers = new string[] { Value1DataMember, Value2DataMember }
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
				seriesType = XmlHelper.EnumFromString<RangeSeriesType>(seriesTypeAttr);
		}
	}
}
