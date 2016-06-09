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
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Compatibility.System.Collections.Specialized;
using DevExpress.Compatibility.System.ComponentModel.Design.Serialization;
using DevExpress.Compatibility.System.Drawing.Design;
namespace DevExpress.DashboardCommon {
	public enum OpenHighLowCloseSeriesType { CandleStick, Stock }
	[
	DesignerSerializer(TypeNames.DataItemContainerCodeDomSerializer, TypeNames.CodeDomSerializer)
	]
	public class OpenHighLowCloseSeries : ChartSeries {
		const OpenHighLowCloseSeriesType DefaultSeriesType = OpenHighLowCloseSeriesType.CandleStick;
		const string xmlSeriesType = "SeriesType";
		OpenHighLowCloseSeriesType seriesType;
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("OpenHighLowCloseSeriesOpen"),
#endif
		Category(CategoryNames.Data),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Editor(TypeNames.ListSelectorEditor, typeof(UITypeEditor)),
		TypeConverter(TypeNames.CreatableMeasurePropertyTypeConverter)
		]
		public Measure Open {
			get { return GetMeasure(OpenHighLowCloseConstants.OpenName); }
			set { SetDataItem(OpenHighLowCloseConstants.OpenName, value); }
		}
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("OpenHighLowCloseSeriesHigh"),
#endif
		Category(CategoryNames.Data),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Editor(TypeNames.ListSelectorEditor, typeof(UITypeEditor)),
		TypeConverter(TypeNames.CreatableMeasurePropertyTypeConverter)
		]
		public Measure High {
			get { return GetMeasure(OpenHighLowCloseConstants.HighName); }
			set { SetDataItem(OpenHighLowCloseConstants.HighName, value); }
		}
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("OpenHighLowCloseSeriesLow"),
#endif
		Category(CategoryNames.Data),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Editor(TypeNames.ListSelectorEditor, typeof(UITypeEditor)),
		TypeConverter(TypeNames.CreatableMeasurePropertyTypeConverter)
		]
		public Measure Low {
			get { return GetMeasure(OpenHighLowCloseConstants.LowName); }
			set { SetDataItem(OpenHighLowCloseConstants.LowName, value); }
		}
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("OpenHighLowCloseSeriesClose"),
#endif
		Category(CategoryNames.Data),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Editor(TypeNames.ListSelectorEditor, typeof(UITypeEditor)),
		TypeConverter(TypeNames.CreatableMeasurePropertyTypeConverter)
		]
		public Measure Close {
			get { return GetMeasure(OpenHighLowCloseConstants.CloseName); }
			set { SetDataItem(OpenHighLowCloseConstants.CloseName, value); }
		}
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("OpenHighLowCloseSeriesSeriesType"),
#endif
		Category(CategoryNames.Data),
		DefaultValue(DefaultSeriesType)
		]
		public OpenHighLowCloseSeriesType SeriesType {
			get { return seriesType; }
			set {
				if(value != seriesType) {
					seriesType = value;
					OnChanged(ChangeReason.View);
				}
			}
		}
		internal String OpenDataMember { get { return GetDataItemActualID(Open ?? DefaultMeasure); } }
		internal String HighDataMember { get { return GetDataItemActualID(High ?? DefaultMeasure); } }
		internal String LowDataMember { get { return GetDataItemActualID(Low ?? DefaultMeasure); } }
		internal String CloseDataMember { get { return GetDataItemActualID(Close ?? DefaultMeasure); } }
		protected override bool ShowOnlyPercentValues { get { return SupportPercentFormatType(Open) && SupportPercentFormatType(High) && SupportPercentFormatType(Low) && SupportPercentFormatType(Close); } }
		protected internal override string SeriesImageName { get { return String.Format(BaseImagePath, seriesType); } }
		Measure DefaultMeasure { get { return Open ?? High ?? Low ?? Close; } }
		public OpenHighLowCloseSeries(Measure open, Measure high, Measure low, Measure close, OpenHighLowCloseSeriesType seriesType)
			: base(new DataItemDescription[] { 
				new DataItemDescription(OpenHighLowCloseConstants.OpenName, OpenHighLowCloseConstants.OpenCaption, open),
				new DataItemDescription(OpenHighLowCloseConstants.HighName, OpenHighLowCloseConstants.HighCaption, high),
				new DataItemDescription(OpenHighLowCloseConstants.LowName, OpenHighLowCloseConstants.LowCaption, low),
				new DataItemDescription(OpenHighLowCloseConstants.CloseName, OpenHighLowCloseConstants.CloseCaption, close)}) {
			this.seriesType = seriesType;
		}
		public OpenHighLowCloseSeries(Measure open, Measure high, Measure low, Measure close)
			: this(open, high, low, close, DefaultSeriesType) {
		}
		public OpenHighLowCloseSeries(OpenHighLowCloseSeriesType seriesType)
			: this(null, null, null, null, seriesType) {
		}
		public OpenHighLowCloseSeries()
			: this(null, null, null, null) {
		}
		protected override DataItemContainer CreateInstance() {
			return new OpenHighLowCloseSeries();
		}
		protected internal override void Assign(DataItemContainer series) {
			base.Assign(series);
			OpenHighLowCloseSeries openHighLowCloseSeries = series as OpenHighLowCloseSeries;
			if (openHighLowCloseSeries != null) {
				seriesType = openHighLowCloseSeries.SeriesType;
				Open = openHighLowCloseSeries.Open;
				High = openHighLowCloseSeries.High;
				Low = openHighLowCloseSeries.Low;
				Close = openHighLowCloseSeries.Close;
			}
		}
		protected internal override ChartSeriesTemplateViewModel CreateSeriesTemplateViewModel() {
			ChartSeriesViewModelType viewModelSeriesType = seriesType == OpenHighLowCloseSeriesType.CandleStick ? ChartSeriesViewModelType.CandleStick : ChartSeriesViewModelType.Stock;
			ChartSeriesTemplateViewModel viewModel = new ChartSeriesTemplateViewModel {
				SeriesType = viewModelSeriesType,
				DataMembers = new string[] { OpenDataMember, HighDataMember, LowDataMember, CloseDataMember }
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
				seriesType = XmlHelper.EnumFromString<OpenHighLowCloseSeriesType>(seriesTypeAttr);
		}
	}
}
