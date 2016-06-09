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
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Xml.Linq;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.Localization;
using DevExpress.DashboardCommon.DataProcessing;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Compatibility.System.Drawing.Design;
namespace DevExpress.DashboardCommon {
	public enum GaugeViewType { CircularFull, CircularHalf, CircularQuarterRight, CircularQuarterLeft, CircularThreeFourth, LinearHorizontal, LinearVertical }
	[
	DashboardItemType(DashboardItemType.Gauge)
	]
	public class GaugeDashboardItem : KpiDashboardItem<Gauge> {
		const string xmlViewType = "ViewType";
		const string xmlShowGaugeCaptions = "ShowGaugeCaptions";
		const GaugeViewType DefaultViewType = GaugeViewType.CircularFull;
		const bool DefaultShowGaugeCaptions = true;
		GaugeViewType viewType = DefaultViewType;
		bool showGaugeCaptions = DefaultShowGaugeCaptions;
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("GaugeDashboardItemGauges"),
#endif
		Category(CategoryNames.Data),
		Editor(TypeNames.NotifyingCollectionEditor, typeof(UITypeEditor)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)
		]
		public GaugeCollection Gauges { get { return (GaugeCollection)Elements; } }
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("GaugeDashboardItemViewType"),
#endif
		Category(CategoryNames.Style),
		DefaultValue(DefaultViewType)
		]
		public GaugeViewType ViewType {
			get { return viewType; }
			set {
				if (value != viewType) {
					viewType = value;
					OnChanged(ChangeReason.View, this);
				}
			}
		}
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("GaugeDashboardItemShowGaugeCaptions"),
#endif
		Category(CategoryNames.Layout),
		DefaultValue(DefaultShowGaugeCaptions)
		]
		public bool ShowGaugeCaptions {
			get { return showGaugeCaptions; }
			set {
				if(value != showGaugeCaptions) {
					showGaugeCaptions = value;
					OnChanged(ChangeReason.View, this);
				}
			}
		}
		protected internal override string CaptionPrefix { get { return DashboardLocalizer.GetString(DashboardStringId.DefaultNameGaugeItem); } }
		protected override IEnumerable<DataDashboardItemDescription> ValuesDescriptions {
			get {
				yield return new DataDashboardItemDescription(DashboardLocalizer.GetString(DashboardStringId.DescriptionGauges),
					DashboardLocalizer.GetString(DashboardStringId.DescriptionItemValue), ItemKind.Gauge, Gauges);
			}
		}
		protected override bool ShowMeasures { get { return true; } }
		public GaugeDashboardItem()
			: base(new GaugeCollection()) {
		}
		protected internal override DashboardItemViewModel CreateViewModel() {
			List<GaugeViewModel> gaugeViewModels = new List<GaugeViewModel>();
			string seriesAxisName = null;
			if (SeriesDimensions.Count == 0) {
				foreach (Gauge gauge in Gauges)
					if (gauge.ActualValue != null || gauge.TargetValue != null)
						gaugeViewModels.Add(new GaugeViewModel(gauge));
			}
			else {
				seriesAxisName = SeriesAxisName;
				Gauge actualGauge = ActiveElement;
				if (actualGauge != null && (actualGauge.ActualValue != null || actualGauge.TargetValue != null))
					gaugeViewModels.Add(new GaugeViewModel(actualGauge));
			}
			GaugeDashboardItemViewModel viewModel = new GaugeDashboardItemViewModel(this, seriesAxisName);
			viewModel.Gauges.AddRange(gaugeViewModels);
			viewModel.ShowGaugeCaptions = ShowGaugeCaptions;
			viewModel.ViewType = ViewType;
			return viewModel;
		}
		protected internal override void SaveToXml(XElement element) {
			base.SaveToXml(element);
			if(ViewType != DefaultViewType)
				element.Add(new XAttribute(xmlViewType, viewType));
			if(ShowGaugeCaptions != DefaultShowGaugeCaptions)
				element.Add(new XAttribute(xmlShowGaugeCaptions, showGaugeCaptions));
		}
		protected internal override void LoadFromXmlInternal(XElement element) {
			base.LoadFromXmlInternal(element);
			string viewTypeAttr = XmlHelper.GetAttributeValue(element, xmlViewType);
			if (!String.IsNullOrEmpty(viewTypeAttr))
				viewType = XmlHelper.EnumFromString<GaugeViewType>(viewTypeAttr);
			string showGaugeCaptionsAttr = XmlHelper.GetAttributeValue(element, xmlShowGaugeCaptions);
			if(!String.IsNullOrEmpty(showGaugeCaptionsAttr))
				showGaugeCaptions = XmlHelper.FromString<bool>(showGaugeCaptionsAttr);
		}
		protected internal override bool CanSpecifyMeasureNumericFormat(Measure measure) {
			bool canSpecify = false;
			foreach(Gauge gauge in Gauges) {
				Measure actual = gauge.ActualValue;
				Measure target = gauge.TargetValue;
				bool isActualValueType = actual == null || target == null || gauge.DeltaOptions.ValueType == DeltaValueType.ActualValue;
				if(actual == measure || (actual == null && target == measure)) {
					canSpecify = isActualValueType;
					break;
				}
				if(actual != null && target == measure) {
					canSpecify = false;
					break;
				}
			}			
			return canSpecify && base.CanSpecifyMeasureNumericFormat(measure);
		}
		internal override DashboardItemDataDescription CreateDashboardItemDataDescription() {
			DashboardItemDataDescription description = base.CreateDashboardItemDataDescription();
			foreach (Dimension dimension in SeriesDimensions)
				description.AddMainDimension(dimension);
			foreach (Gauge gauge in Gauges)
				if (gauge.TargetValue != null || gauge.DeltaOptions.Changed)
					description.AddMeasure(gauge.ActualValue, gauge.TargetValue, gauge.DeltaOptions);
				else
					description.AddMeasure(gauge.ActualValue);
			return description;
		}
		internal override void AssignDashboardItemDataDescriptionCore(DashboardItemDataDescription description) {
			base.AssignDashboardItemDataDescriptionCore(description);
			AssignDimension(description.Latitude, SeriesDimensions);
			SeriesDimensions.AddRange(description.MainDimensions);
			AssignDimension(description.Longitude, SeriesDimensions);
			SeriesDimensions.AddRange(description.AdditionalDimensions);
			AssignDimension(description.SparklineArgument, HiddenDimensions);
			foreach (MeasureDescription measureBox in description.MeasureDescriptions)
				if (measureBox.MeasureType == MeasureDescriptionType.Delta) {
					Gauge gauge = new Gauge(measureBox.ActualValue, measureBox.TargetValue);
					gauge.DeltaOptions.Assign(measureBox.DeltaOptions);
					Gauges.Add(gauge);
				}
				else
					Gauges.Add(new Gauge(measureBox.Value));
		}
	}
}
