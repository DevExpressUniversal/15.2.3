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
using DevExpress.DashboardCommon.Localization;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Compatibility.System.ComponentModel.Design.Serialization;
using DevExpress.Compatibility.System.Drawing.Design;
namespace DevExpress.DashboardCommon {
	[
	DesignerSerializer(TypeNames.DataItemContainerCodeDomSerializer, TypeNames.CodeDomSerializer)
	]
	public class WeightedSeries : ChartSeries {
		internal const string ImagePath = "Bubble";
		const string valueName = "Value";
		const string weightName = "Weight";
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("WeightedSeriesValue"),
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
	DevExpressDashboardCoreLocalizedDescription("WeightedSeriesWeight"),
#endif
		Category(CategoryNames.Data),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Editor(TypeNames.ListSelectorEditor, typeof(UITypeEditor)),
		TypeConverter(TypeNames.CreatableMeasurePropertyTypeConverter)
		]
		public Measure Weight {
			get { return GetMeasure(weightName); }
			set { SetDataItem(weightName, value); }
		}
		internal String ValueDataMember { get { return GetDataItemActualID(Value ?? DefaultMeasure); } }
		internal String WeightDataMember { get { return GetDataItemActualID(Weight ?? DefaultMeasure); } }
		protected override bool ShowOnlyPercentValues { get { return SupportPercentFormatType(Value); } }
		protected internal override string SeriesImageName { get { return string.Format(BaseImagePath, ImagePath); } }
		Measure DefaultMeasure { get { return Value ?? Weight; } }
		public WeightedSeries(Measure value, Measure weight)
			: base(new DataItemDescription[] { 
				new DataItemDescription(valueName, DashboardLocalizer.GetString(DashboardStringId.ValueCaption), value), 
				new DataItemDescription(weightName, DashboardLocalizer.GetString(DashboardStringId.WeightCaption), weight) }) {
		}
		public WeightedSeries()
			: this(null, null) {
		}
		protected override DataItemContainer CreateInstance() {
			return new WeightedSeries();
		}
		protected internal override void Assign(DataItemContainer series) {
			base.Assign(series);
			WeightedSeries weightedSeries = series as WeightedSeries;
			if (weightedSeries != null ) {
				Value = weightedSeries.Value;
				Weight = weightedSeries.Weight;
			}
		}
		protected internal override ChartSeriesTemplateViewModel CreateSeriesTemplateViewModel() {
			ChartSeriesTemplateViewModel viewModel = new ChartSeriesTemplateViewModel {
				SeriesType = ChartSeriesViewModelType.Weighted,
				DataMembers = new string[] { ValueDataMember, WeightDataMember }
			};
			UpdateViewModel(viewModel);
			return viewModel;
		}
	}
}
