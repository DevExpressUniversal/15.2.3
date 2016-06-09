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
using System.Drawing.Design;
using System.Linq;
using System.Xml.Linq;
using DevExpress.DashboardCommon.Localization;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.DataAccess.Native;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Compatibility.System.Drawing.Design;
namespace DevExpress.DashboardCommon {
	[
	DashboardItemType(DashboardItemType.Chart)
	]
	public class ChartDashboardItem : ChartDashboardItemBase, IChartAxisContainer, IChartDashboardItem {
		const string xmlRotated = "Rotated";
		const string xmlAxisX = "AxisX";
		const bool DefaultRotated = false;
		static readonly List<SeriesViewGroup> seriesViewGroups = new List<SeriesViewGroup>();
		internal static string CreateChartPaneDescriptionCaption(ChartPane pane) {
			return string.Format("{0} ({1})", DashboardLocalizer.GetString(DashboardStringId.DescriptionValues), pane != null ? pane.Name : string.Empty);
		}
		internal static IEnumerable<SeriesViewGroup> SeriesViewGroups {
			get {
				if (seriesViewGroups.Count == 0) {
					SeriesViewGroup barGroup = new SeriesViewGroup(DashboardLocalizer.GetString(DashboardStringId.SeriesTypeGroupBar),
						new ChartSimpleSeriesConverter(SimpleSeriesType.Bar), new ChartSimpleSeriesConverter(SimpleSeriesType.StackedBar),
						new ChartSimpleSeriesConverter(SimpleSeriesType.FullStackedBar));
					SeriesViewGroup pointLineGroup = new SeriesViewGroup(DashboardLocalizer.GetString(DashboardStringId.SeriesTypeGroupPointLine),
						new ChartSimpleSeriesConverter(SimpleSeriesType.Point), new ChartSimpleSeriesConverter(SimpleSeriesType.Line),
						new ChartSimpleSeriesConverter(SimpleSeriesType.StackedLine), new ChartSimpleSeriesConverter(SimpleSeriesType.FullStackedLine),
						new ChartSimpleSeriesConverter(SimpleSeriesType.StepLine), new ChartSimpleSeriesConverter(SimpleSeriesType.Spline));
					SeriesViewGroup areaGroup = new SeriesViewGroup(DashboardLocalizer.GetString(DashboardStringId.SeriesTypeGroupArea),
						new ChartSimpleSeriesConverter(SimpleSeriesType.Area), new ChartSimpleSeriesConverter(SimpleSeriesType.StackedArea),
						new ChartSimpleSeriesConverter(SimpleSeriesType.FullStackedArea), new ChartSimpleSeriesConverter(SimpleSeriesType.StepArea),
						new ChartSimpleSeriesConverter(SimpleSeriesType.SplineArea), new ChartSimpleSeriesConverter(SimpleSeriesType.StackedSplineArea),
						new ChartSimpleSeriesConverter(SimpleSeriesType.FullStackedSplineArea));
					List<ChartRangeSeriesConverter> rangeConverters = new List<ChartRangeSeriesConverter>();
					foreach (RangeSeriesType seriesType in Enum.GetValues(typeof(RangeSeriesType)))
						rangeConverters.Add(new ChartRangeSeriesConverter(seriesType));
					SeriesViewGroup rangeGroup = new SeriesViewGroup(DashboardLocalizer.GetString(DashboardStringId.SeriesTypeGroupRange), rangeConverters.ToArray());
					SeriesViewGroup bubbleGroup = new SeriesViewGroup(DashboardLocalizer.GetString(DashboardStringId.SeriesTypeGroupBubble), new ChartWeightedSeriesConverter());
					List<ChartSeriesConverter> financialConverters = new List<ChartSeriesConverter>();
					financialConverters.Add(new ChartHighLowCloseSeriesConverter());
					foreach (OpenHighLowCloseSeriesType seriesType in Enum.GetValues(typeof(OpenHighLowCloseSeriesType)))
						financialConverters.Add(new ChartOpenHighLowCloseSeriesConverter(seriesType));
					SeriesViewGroup financialGroup = new SeriesViewGroup(DashboardLocalizer.GetString(DashboardStringId.SeriesTypeGroupFinancial), financialConverters.ToArray());
					seriesViewGroups.AddRange(new SeriesViewGroup[] { barGroup, pointLineGroup, areaGroup, rangeGroup, bubbleGroup, financialGroup });
				}
				return seriesViewGroups;
			}
		}
		protected override ItemKind HeaderKind { get { return ItemKind.ChartHeader; } }
		readonly Locker locker = new Locker();
		readonly ChartPaneCollection panes = new ChartPaneCollection();
		readonly CollectionPrefixNameGenerator<ChartPane> paneNameGenerator;
		readonly ChartLegend legend;
		ChartAxisX axisX;
		bool rotated = DefaultRotated;
		ChartSeriesConverter defaultSeriesConverter = new ChartSimpleSeriesConverter(SimpleSeriesType.Bar);
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("ChartDashboardItemPanes"),
#endif
		Category(CategoryNames.Data),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		Editor(TypeNames.ChartPanesCollectionEditor, typeof(UITypeEditor))
		]
		public ChartPaneCollection Panes { get { return panes; } }
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("ChartDashboardItemRotated"),
#endif
		Category(CategoryNames.Layout),
		DefaultValue(DefaultRotated)
		]
		public bool Rotated {
			get { return rotated; }
			set {
				if(value != rotated) {
					rotated = value;
					OnChanged(ChangeReason.View);
				}
			}
		}
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("ChartDashboardItemAxisX"),
#endif
		Category(CategoryNames.Layout),
		TypeConverter(TypeNames.DisplayNameObjectConverter),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)
		]
		public ChartAxisX AxisX { get { return axisX; } }
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("ChartDashboardItemLegend"),
#endif
		Category(CategoryNames.Layout),
		TypeConverter(TypeNames.DisplayNameObjectConverter),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)
		]
		public ChartLegend Legend { get { return legend; } }
		protected override bool HasValues {
			get {
				foreach(ChartSeries series in Series)
					if(series.DataItemsCount != 0)
						return true;
				return false;
			}
		}
		protected internal override string CaptionPrefix { get { return DashboardLocalizer.GetString(DashboardStringId.DefaultNameChartItem); } }
		protected override IEnumerable<DataDashboardItemDescription> ValuesDescriptions {
			get {
				if(panes.Count == 0)
					yield return CreateChartPaneDescription(null);
				else
					foreach(ChartPane pane in panes)
						yield return CreateChartPaneDescription(pane);
			}
		}
		protected override bool IsSupportArgumentNumericGroupIntervals { get { return Arguments.Count == 1 || (IsDrillDownEnabled && InteractivityOptions.TargetDimensions == TargetDimensions.Arguments); } }
		protected internal override bool HasDataItems { get { return base.HasDataItems || HasValues; } }
		protected internal override Dictionary<string, string> ColorMeasureDescriptorsInfo {
			get { 
				Dictionary<string, string> dict = new Dictionary<string,string>();
				foreach(ChartSeries series in Series){
					string key = series.ColorDefinitionName;
					if(!dict.ContainsKey(key))
						dict.Add(key, series.DisplayName);
				}
				return dict;
			}
		}
		protected internal override bool CanColorByMeasures { get { return CanColor; } }
		protected internal override bool CanColorByDimensions { get { return CanColor; } }
		internal override bool IsGloballyColored { get { return ColoringOptionsBase.UseGlobalColors; } }
		IEnumerable<ChartSeries> ColoredSeries { get { return Panes.SelectMany(p => p.Series).Where(series => !(series is HighLowCloseSeries || series is OpenHighLowCloseSeries)); } }
		bool CanColor { get { return ColoredSeries.Count() > 0; } }
		internal IEnumerable<ChartSeries> Series {
			get { 
				foreach (ChartPane pane in panes)
					foreach (ChartSeries series in pane.Series)
						yield return series; 
			}
		}
		internal ChartSeriesConverter DefaultSeriesConverter {
			get { return defaultSeriesConverter; }
			set {
				if (defaultSeriesConverter != value) {
					defaultSeriesConverter = value;
					OnChanged(ChangeReason.View);
				}
			}
		}
		public ChartDashboardItem() {
			panes.CollectionChanged += (sender, e) => OnPanesCollectionChanged(e.AddedItems, e.RemovedItems);
			paneNameGenerator = new CollectionPrefixNameGenerator<ChartPane>(panes);
			axisX = new ChartAxisX(this);
			legend = new ChartLegend(this);
		}
		DataDashboardItemDescription CreateChartPaneDescription(ChartPane pane) {
			return new DataDashboardItemDescription(CreateChartPaneDescriptionCaption(pane), DashboardLocalizer.GetString(DashboardStringId.DescriptionItemValue),
				ItemKind.ChartPane, new ChartPaneDescription(SeriesViewGroups, DefaultSeriesConverter, pane));
		}
		void OnPanesCollectionChanged(IEnumerable<ChartPane> addedPanes, IEnumerable<ChartPane> removedPanes) {
			if(locker.IsLocked)
				return;
			List<ChartSeries> addedSeries = new List<ChartSeries>();
			List<ChartSeries> removedSeries = new List<ChartSeries>();
			foreach(ChartPane pane in addedPanes) {
				pane.DashboardItem = this;
				addedSeries.AddRange(pane.Series);
			}			
			foreach(ChartPane pane in removedPanes) {
				pane.DashboardItem = null;
				removedSeries.AddRange(pane.Series);
			}
			OnDataItemContainersChanged(addedSeries, removedSeries);
			RaiseDescriptionsChanged();
		}
		internal void OnSeriesCollectionChanged(IEnumerable<ChartSeries> addedSeries, IEnumerable<ChartSeries> removedSeries) {
			OnDataItemContainersChanged(addedSeries, removedSeries);
		}
		protected override Measure[][] GetColoringMeasuresInternal() {
			List<Measure[]> measures = new List<Measure[]>();
			foreach(ChartSeries series in ColoredSeries) {
				if(series.Measures.Count() > 0)
					measures.Add(series.Measures.ToArray());
			}
			return measures.ToArray();
		}
		protected override bool ColorDimension(Dimension dimension) {
			return SeriesDimensions.Contains(dimension);
		}
		protected override void Dispose(bool disposing) {
			if(disposing)
				paneNameGenerator.Dispose();
			base.Dispose(disposing);
		}
		protected internal override bool IsSortingByMeasureEnabled(Dimension dimension, DashboardItemViewModel model) { 
			return base.IsSortingByMeasureEnabled(dimension, model) && (!Arguments.Contains(dimension) || ((ChartDashboardItemViewModel)model).Argument.Type == ChartArgumentType.String);
		}
		protected internal override DashboardItemViewModel CreateViewModel() {
			List<ChartPaneViewModel> panesViewModel = new List<ChartPaneViewModel>();
			foreach (ChartPane pane in panes) {
				ChartPaneViewModel paneViewModel = pane.CreateViewModel();
				if (paneViewModel != null)
					panesViewModel.Add(paneViewModel);
			}
			ChartDashboardItemViewModel result = new ChartDashboardItemViewModel(this, panesViewModel);
			result.AxisX = axisX.CreateViewModel(result);
			return result;
		}
		protected internal override void SaveToXml(XElement element) {
			base.SaveToXml(element);
			panes.SaveToXml(element);
			if(Rotated != DefaultRotated)
				element.Add(new XAttribute(xmlRotated, rotated));
			if(legend.ShouldSerialize())
				element.Add(legend.SaveToXml());
			if(axisX.ShouldSerialize()) {
				XElement axisXElement = new XElement(xmlAxisX);
				axisX.SaveToXml(axisXElement);
				element.Add(axisXElement);
			}
		}
		protected internal override void LoadFromXmlInternal(XElement element) {
			base.LoadFromXmlInternal(element);
			panes.LoadFromXml(element);
			string rotatedString = XmlHelper.GetAttributeValue(element, xmlRotated);
			if (!String.IsNullOrEmpty(rotatedString))
				rotated = XmlHelper.FromString<bool>(rotatedString);
			legend.LoadFromXml(element);
			XElement chartAxisXElement = element.Element(xmlAxisX);
			if (chartAxisXElement != null) {
				ChartAxisX chartAxisX = new ChartAxisX(this);
				chartAxisX.LoadFromXml(chartAxisXElement);
				this.axisX = chartAxisX;
			}
		}
		protected override void OnEndLoading() {
			base.OnEndLoading();
			foreach(ChartPane pane in panes)
				pane.OnEndLoading();
		}
		internal string GetAxisXTitle() {
			if(Arguments.Count == 0)
				return string.Empty;
			if(Arguments.Count == 1) {
				if(DataSource != null)
					return Arguments[0].DisplayName;
			}
			if(Arguments.Count > 1)
				return DashboardLocalizer.GetString(DashboardStringId.AxisXNameArguments);
			return string.Empty;
		}
		internal ChartPane CreateDefaultChartPane() {
			ChartPane pane = new ChartPane();
			locker.Lock();
			try {
				panes.Add(pane);
				pane.DashboardItem = this;
			}
			finally {
				locker.Unlock();
			}
			return pane;
		}
		protected override void GetMetadataInternal(HierarchicalMetadataBuilder builder) {
			base.GetMetadataInternal(builder);
			foreach(ChartSeries series in Series) {
				foreach(Measure measure in series.Measures)
					builder.AddMeasure(measure);
			}
			builder.AddColorMeasureDescriptors(this);
		}
		internal override DashboardItemDataDescription CreateDashboardItemDataDescription() {
			DashboardItemDataDescription description = base.CreateDashboardItemDataDescription();
			description.HasAdditionalDimensions = true;
			foreach (Dimension dimension in SeriesDimensions)
				description.AddAdditionalDimension(dimension);
			foreach (Dimension dimension in Arguments)
				description.AddMainDimension(dimension);
			foreach (ChartSeries series in Series) {
				description.AddSeries(series);
				foreach (Measure measure in series.Measures)
					description.AddMeasure(measure);
			}
			return description;
		}
		internal override void AssignDashboardItemDataDescriptionCore(DashboardItemDataDescription description) {
			base.AssignDashboardItemDataDescriptionCore(description);
			AssignDimension(description.Latitude, Arguments);
			AssignDimension(description.Longitude, Arguments);
			if (!description.HasAdditionalDimensions && description.MainDimensions.Count != 0 && description.AdditionalDimensions.Count == 0) {
				List<Dimension> dimensions = new List<Dimension>();
				dimensions.AddRange(description.MainDimensions);
				DataFieldType fieldType = dimensions[0].DataFieldType;
				while (dimensions.Count > 0) {
					if (dimensions[0].DataFieldType == fieldType) {
						Arguments.Add(dimensions[0]);
						dimensions.RemoveAt(0);
					}
					else
						break;
				}
				SeriesDimensions.AddRange(dimensions);
			}
			else {
				Arguments.AddRange(description.MainDimensions);
				SeriesDimensions.AddRange(description.AdditionalDimensions);
			}
			if (Arguments.Count == 0 && SeriesDimensions.Count != 0) {
				List<Dimension> arg = new List<Dimension>();
				arg.AddRange(SeriesDimensions);
				SeriesDimensions.Clear();
				Arguments.AddRange(arg);
			}
			AssignDimension(description.SparklineArgument, HiddenDimensions);
			if (Panes.Count == 0 && (description.MeasureDescriptions.Count != 0 || description.Series.Count != 0))
				Panes.Add(new ChartPane());
			if (description.Series.Count != 0)
				foreach (ChartSeries series in description.Series)
					panes[0].Series.Add(series);
			else
				foreach (Measure measure in description.Measures)
					panes[0].Series.Add(new SimpleSeries(measure));
		}
		protected override void FillEditNameDescriptions(IList<EditNameDescription> descriptions) {
			base.FillEditNameDescriptions(descriptions);
			if (SeriesDimensions.Count == 0 || Series.Count() > 1)
				descriptions.Add(new EditNameDescription(DashboardLocalizer.GetString(DashboardStringId.DescriptionSeries), Series));
		}
		protected internal override ColoringSchemeDefinition GetColoringScheme() {
			return CanColor? base.GetColoringScheme() : new ColoringSchemeDefinition();
		}
		protected internal override bool IsColoringEnabled(DataItem dataItem) {
			if(!base.IsColoringEnabled(dataItem))
				return false;
			Measure measure = dataItem as Measure;
			if(measure != null)
				return ColoredSeries.SelectMany(series => series.Measures).Contains(measure);
			return true;
		}
		protected override IEnumerable<Measure> GetQueryVisibleMeasures() {
			return Series.SelectMany(s => s.DataItems.OfType<Measure>()).NotNull();
		}
		#region IChartAxisContainer Members
		bool IChartAxisContainer.isReverseRequiredForContinuousScale {
			get { return Arguments[0].SortOrder == DimensionSortOrder.Descending; }
		}
		void IChartAxisContainer.OnChanged(ChangeReason reason, object context) {
			OnChanged(reason, context);
		}
		string IChartAxisContainer.GetTitle(bool isSecondary) {
			return GetAxisXTitle();
		}
		#endregion
	}
}
