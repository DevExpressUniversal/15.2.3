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
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using DevExpress.DashboardCommon.Localization;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.DashboardCommon {
	internal interface IChartSeriesOptions {
		bool PlotOnSecondaryAxis { get; set; }
		bool IgnoreEmptyPoints { get; set; }
		bool ShowPointMarkers { get; set; }
		PointLabelOptions PointLabelOptions { get; }
	}
	public abstract class ChartSeries : NamedDataItemContainer, IChartSeriesOptions {
		internal const string BaseImagePath = "ChartSeries_{0}";
		internal static void SaveBoolPropertyToXml(XElement element, string xmlName, bool value, bool defaultValue) {
			if(value != defaultValue)
				element.Add(new XAttribute(xmlName, value));
		}
		internal static void LoadBoolPropertyFromXml(XElement element, string xmlName, ref bool value) {
			string attribute = XmlHelper.GetAttributeValue(element, xmlName);
			if(!String.IsNullOrEmpty(attribute))
				value = XmlHelper.FromString<bool>(attribute);
		}
		const string xmlPlotOnSecondaryAxis = "PlotOnSecondaryAxis";
		const string xmlIgnoreEmptyPoints = "IgnoreEmptyPoints";
		const string xmlShowPointMarkers = "ShowPointMarkers";
		const bool DefaultPlotOnSecondaryAxis = false;
		const bool DefaultIgnoreEmptyPoints = false;
		const bool DefaultShowPointMarkers = false;
		bool plotOnSecondaryAxis = DefaultPlotOnSecondaryAxis;
		bool ignoreEmptyPoints = DefaultIgnoreEmptyPoints;
		bool showPointMarkers = DefaultShowPointMarkers;
		PointLabelOptions pointLabel = new PointLabelOptions();
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("ChartSeriesPlotOnSecondaryAxis"),
#endif
		Category(CategoryNames.Layout),
		DefaultValue(DefaultPlotOnSecondaryAxis)
		]
		public bool PlotOnSecondaryAxis {
			get { return plotOnSecondaryAxis; }
			set {
				if(value != plotOnSecondaryAxis) {
					plotOnSecondaryAxis = value;
					OnChanged(ChangeReason.View);
				}
			}
		}
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("ChartSeriesIgnoreEmptyPoints"),
#endif
		Category(CategoryNames.Layout),
		DefaultValue(DefaultIgnoreEmptyPoints)
		]
		public bool IgnoreEmptyPoints {
			get { return ignoreEmptyPoints; }
			set {
				if(ignoreEmptyPoints != value) {
					ignoreEmptyPoints = value;
					OnChanged(ChangeReason.View);
				}
			}
		}
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("ChartSeriesShowPointMarkers"),
#endif
		Category(CategoryNames.Layout),
		DefaultValue(DefaultShowPointMarkers)
		]
		public bool ShowPointMarkers {
			get { return showPointMarkers; }
			set {
				if(showPointMarkers != value) {
					showPointMarkers = value;
					OnChanged(ChangeReason.View);
				}
			}
		}
		[
		Category(CategoryNames.Layout),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		TypeConverter(TypeNames.DisplayNameObjectConverter)
		]
		public PointLabelOptions PointLabelOptions {
			get { return pointLabel; }
		}
		internal ChartPane Pane { get; set; }
		protected internal override string DefaultName {
			get {
				StringBuilder nameBuilder = new StringBuilder();
				List<string> measureCaptions = GetMeasureCaptions();
				if(measureCaptions.Count > 0) {
					nameBuilder.Append(measureCaptions[0]);
					for(int i = 1; i < measureCaptions.Count; i++)
						nameBuilder.AppendFormat(DashboardLocalizer.GetString(DashboardStringId.FormatStringSeriesName), measureCaptions[i]);
				}
				return nameBuilder.ToString();
			}
		}
		protected internal abstract string SeriesImageName { get; }
		protected internal string ColorDefinitionName {
			get { 
				return String.Join("_", Measures.Select(measure => DataItemDefinitionDisplayTextProvider.GetMeasureDefinitionString(measure.GetMeasureDefinition())).OrderBy(str => str)); 
			}
		}
		protected abstract bool ShowOnlyPercentValues { get; }
		protected ChartSeries(IEnumerable<DataItemDescription> measureDescriptions)
			: base(measureDescriptions) {
				pointLabel.Changed += PointLabelChanged;
		}
		~ChartSeries(){
			pointLabel.Changed -= PointLabelChanged;
		}
		protected bool SupportPercentFormatType(Measure measure) {
			return measure != null && measure.NumericFormat.FormatType == DataItemNumericFormatType.Percent;
		}
		void PointLabelChanged(object sender, ChangedEventArgs e) {
			OnChanged(ChangeReason.View);
		}
		internal List<string> GetMeasureCaptions() {
			List<string> measureCaptions = new List<string>();
			foreach(string measureName in DataItemKeys) {
				Measure measure = GetMeasure(measureName);
				if(measure != null) {
					string measureCaption = measure.DisplayName;
					if(!string.IsNullOrEmpty(measureCaption))
						measureCaptions.Add(measureCaption);
				}
			}
			return measureCaptions;
		}
		protected internal override void Assign(DataItemContainer series) {
			base.Assign(series);
			ChartSeries chartSeries = series as ChartSeries;
			if (chartSeries != null)
				Pane = chartSeries.Pane;
			chartSeries.plotOnSecondaryAxis = plotOnSecondaryAxis;
			chartSeries.ignoreEmptyPoints = ignoreEmptyPoints;
		}
		protected internal abstract ChartSeriesTemplateViewModel CreateSeriesTemplateViewModel();
		protected internal void UpdateViewModel(ChartSeriesTemplateViewModel viewModel) {
			viewModel.MeasureCaptions = GetMeasureCaptions();
			viewModel.Name = DisplayName;
			viewModel.PlotOnSecondaryAxis = PlotOnSecondaryAxis;
			viewModel.IgnoreEmptyPoints = IgnoreEmptyPoints;
			viewModel.ShowPointMarkers = ShowPointMarkers;
			viewModel.PointLabel = PointLabelOptions.CreateViewModel();
			viewModel.OnlyPercentValues = ShowOnlyPercentValues;
		}
		protected internal override void SaveToXml(XElement element) {
			base.SaveToXml(element);
			SaveBoolPropertyToXml(element, xmlPlotOnSecondaryAxis, plotOnSecondaryAxis, DefaultPlotOnSecondaryAxis);
			SaveBoolPropertyToXml(element, xmlIgnoreEmptyPoints, ignoreEmptyPoints, DefaultIgnoreEmptyPoints);
			SaveBoolPropertyToXml(element, xmlShowPointMarkers, showPointMarkers, DefaultShowPointMarkers);
			if(pointLabel.ShouldSerialize())
				element.Add(pointLabel.SaveToXml());
		}
		protected internal override void LoadFromXml(XElement element) {
			base.LoadFromXml(element);
			LoadBoolPropertyFromXml(element, xmlPlotOnSecondaryAxis, ref plotOnSecondaryAxis);
			LoadBoolPropertyFromXml(element, xmlIgnoreEmptyPoints, ref ignoreEmptyPoints);
			LoadBoolPropertyFromXml(element, xmlShowPointMarkers, ref showPointMarkers);
			pointLabel.LoadFromXml(element);
		}
		internal void AssignOptions(IChartSeriesOptions options) {
			plotOnSecondaryAxis = options.PlotOnSecondaryAxis;
			ignoreEmptyPoints = options.IgnoreEmptyPoints;
			showPointMarkers = options.ShowPointMarkers;
			pointLabel.Assign(options.PointLabelOptions);
		}
		protected internal override DataItemContainerActualContent GetActualContent() {
			return new DataItemContainerActualContent();
		}
		protected override SummaryTypeInfo GetSummaryTypeInfo(Measure measure) {
			return SummaryTypeInfo.Number;
		}
	}
}
