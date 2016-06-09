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
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Drawing.Design;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Compatibility.System.Collections.Specialized;
using DevExpress.Compatibility.System.Drawing.Design;
using DevExpress.Compatibility.System.ComponentModel.Design.Serialization;
namespace DevExpress.DashboardCommon {
	[
	DesignerSerializer(TypeNames.DataItemContainerCodeDomSerializer, TypeNames.CodeDomSerializer)
	]
	public class HighLowCloseSeries : ChartSeries {
		internal const string ImagePath = "HighLowClose";
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("HighLowCloseSeriesHigh"),
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
	DevExpressDashboardCoreLocalizedDescription("HighLowCloseSeriesLow"),
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
	DevExpressDashboardCoreLocalizedDescription("HighLowCloseSeriesClose"),
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
		internal string HighDataMember { get { return GetDataItemActualID(High ?? DefaultMeasure); } }
		internal string LowDataMember { get { return GetDataItemActualID(Low ?? DefaultMeasure); } }
		internal string CloseDataMember { get { return GetDataItemActualID(Close ?? DefaultMeasure); } }
		protected override bool ShowOnlyPercentValues { get { return SupportPercentFormatType(High) && SupportPercentFormatType(Low) && SupportPercentFormatType(Close); } }
		protected internal override string SeriesImageName { get { return string.Format(BaseImagePath, ImagePath); } }
		Measure DefaultMeasure { get { return High ?? Low ?? Close; } }
		public HighLowCloseSeries(Measure high, Measure low, Measure close)
			: base(new DataItemDescription[] { 
				new DataItemDescription(OpenHighLowCloseConstants.HighName, OpenHighLowCloseConstants.HighCaption, high),
				new DataItemDescription(OpenHighLowCloseConstants.LowName, OpenHighLowCloseConstants.LowCaption, low),
				new DataItemDescription(OpenHighLowCloseConstants.CloseName, OpenHighLowCloseConstants.CloseCaption, close)}) {
		}
		public HighLowCloseSeries()
			: this(null, null, null) {
		}
		protected override DataItemContainer CreateInstance() {
			return new HighLowCloseSeries();
		}
		protected internal override void Assign(DataItemContainer series) {
			base.Assign(series);
			HighLowCloseSeries highLowCloseSeries = series as HighLowCloseSeries;
			if (highLowCloseSeries != null) {
				High = highLowCloseSeries.High;
				Low = highLowCloseSeries.Low;
				Close = highLowCloseSeries.Close;
			}
		}
		protected internal override ChartSeriesTemplateViewModel CreateSeriesTemplateViewModel() {
			ChartSeriesTemplateViewModel viewModel = new ChartSeriesTemplateViewModel {
				SeriesType = ChartSeriesViewModelType.HighLowClose,
				DataMembers = new string[] { HighDataMember, LowDataMember, CloseDataMember }
			};
			UpdateViewModel(viewModel);
			return viewModel;
		}
	}
}
