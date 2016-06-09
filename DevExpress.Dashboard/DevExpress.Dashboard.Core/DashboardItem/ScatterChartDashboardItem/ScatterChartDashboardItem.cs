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
using DevExpress.DashboardCommon.DataProcessing;
using DevExpress.DashboardCommon.Localization;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.DataAccess;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Compatibility.System.Drawing.Design;
namespace DevExpress.DashboardCommon {
	[
	DashboardItemType(DashboardItemType.ScatterChart)
	]
	public class ScatterChartDashboardItem : DataDashboardItem, IChartDashboardItem {
		internal const string DimensionAxisName = DashboardDataAxisNames.ChartArgumentAxis;
		internal const string EmptyAxisName = DashboardDataAxisNames.ChartSeriesAxis;
		const string xmlMeasureX = "MeasureX";
		const string xmlMeasureY = "MeasureY";
		const string xmlMeasureWeight = "MeasureWeight";
		const string xmlRotated = "Rotated";
		const string xmlAxisX = "AxisX";
		const string xmlAxisY = "AxisY";
		const string xmlDimensions = "Dimensions";
		const string xmlDimension = "Dimension";
		const bool DefaultRotated = false;
		readonly ChartLegend legend;
		readonly DataItemBox<Measure> axisXMeasure;
		readonly DataItemBox<Measure> axisYMeasure;
		readonly DataItemBox<Measure> weightBox;
		readonly ScatterAxisXMeasureHolder axisXMeasureHolder;
		readonly ScatterAxisYMeasureHolder axisYMeasureHolder;
		readonly ChartScatterWeightHolder weightHolder;
		readonly DimensionCollection arguments = new DimensionCollection();
		readonly ScatterPointLabelOptions pointLabel;
		ScatterChartAxis axisX;
		ScatterChartAxis axisY;
		bool rotated = DefaultRotated;
		[
		Category(CategoryNames.Data),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		Editor(TypeNames.NotifyingCollectionEditor, typeof(UITypeEditor))
		]
		public DimensionCollection Arguments { get { return arguments; } }
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("ScatterChartDashboardItemInteractivityOptions"),
#endif
 Category(CategoryNames.Interactivity),
		TypeConverter(TypeNames.DisplayNameObjectConverter),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)
		]
		public DashboardItemInteractivityOptions InteractivityOptions { get { return InteractivityOptionsBase as DashboardItemInteractivityOptions; } }
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("ScatterChartDashboardItemColorScheme"),
#endif
		Category(CategoryNames.Data),
		Editor(TypeNames.DashboardItemColorSchemeEditor, typeof(UITypeEditor)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)
		]
		public ColorScheme ColorScheme { get { return ColorSchemeContainer.Scheme; } }
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("ScatterChartDashboardItemColoringOptions"),
#endif
		Category(CategoryNames.Data),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		TypeConverter(TypeNames.DisplayNameObjectConverter)
		]
		public DashboardItemColoringOptions ColoringOptions { get { return ColoringOptionsBase; } }
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("ScatterChartDashboardItemRotated"),
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
	DevExpressDashboardCoreLocalizedDescription("ScatterChartDashboardItemLegend"),
#endif
		Category(CategoryNames.Layout),
		TypeConverter(TypeNames.DisplayNameObjectConverter),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)
		]
		public ChartLegend Legend { get { return legend; } }
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("ScatterChartDashboardItemAxisX"),
#endif
		Category(CategoryNames.Layout),
		TypeConverter(TypeNames.DisplayNameObjectConverter),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)
		]
		public ScatterChartAxis AxisX { get { return axisX; } }
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("ScatterChartDashboardItemAxisY"),
#endif
		Category(CategoryNames.Layout),
		TypeConverter(TypeNames.DisplayNameObjectConverter),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)
		]
		public ScatterChartAxis AxisY { get { return axisY; } }
		[
		Category(CategoryNames.Layout),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		TypeConverter(TypeNames.DisplayNameObjectConverter)
		]
		public ScatterPointLabelOptions PointLabelOptions {
			get { return pointLabel; }
		}
		[
		Category(CategoryNames.Data),
		Editor(TypeNames.ListSelectorEditor, typeof(UITypeEditor)),
		TypeConverter(TypeNames.CreatableMeasurePropertyTypeConverter),
		DefaultValue(null)
		]
		public Measure AxisXMeasure {
			get { return axisXMeasure.DataItem; }
			set { axisXMeasure.DataItem = value; }
		}
		[
		Category(CategoryNames.Data),
		Editor(TypeNames.ListSelectorEditor, typeof(UITypeEditor)),
		TypeConverter(TypeNames.CreatableMeasurePropertyTypeConverter),
		DefaultValue(null)
		]
		public Measure AxisYMeasure {
			get { return axisYMeasure.DataItem; }
			set { axisYMeasure.DataItem = value; }
		}
		[
		Category(CategoryNames.Data),
		Editor(TypeNames.ListSelectorEditor, typeof(UITypeEditor)),
		TypeConverter(TypeNames.CreatableMeasurePropertyTypeConverter),
		DefaultValue(null)
		]
		public Measure Weight {
			get { return weightBox.DataItem; }
			set { weightBox.DataItem = value; }
		}
		protected internal override bool IsDrillDownEnabled {
			get { return InteractivityOptions.IsDrillDownEnabled; } 
			set { InteractivityOptions.IsDrillDownEnabled = value; }
		}
		protected internal override DashboardItemMasterFilterMode MasterFilterMode {
			get { return InteractivityOptions.MasterFilterMode; } 
			set { InteractivityOptions.MasterFilterMode = value; }
		}
		protected internal override IList<Dimension> SelectionDimensionList { get { return Arguments; } }
		protected internal override bool HasDataItems { get { return base.HasDataItems || arguments.Count > 0 || AxisXMeasure != null || AxisYMeasure != null || Weight != null; } }
		protected internal override bool CanColorByMeasures { get { return false; } }
		protected internal override bool CanColorByDimensions { get { return true; } }
		protected internal override Dimension[] ColoringDimensions { get { return GetColoringDimensions(IsDrillDownEnabled, Arguments); } }
		protected internal override string CaptionPrefix { get { return DashboardLocalizer.GetString(DashboardStringId.DefaultNameScatterChartItem); } }
		protected override IEnumerable<DataDashboardItemDescription> ItemDescriptions {
			get {
				yield return new DataDashboardItemDescription(DashboardLocalizer.GetString(DashboardStringId.DescriptionItemAxisXMeasure),
					DashboardLocalizer.GetString(DashboardStringId.DescriptionItemAxisXMeasure), ItemKind.SingleNumericMeasure, axisXMeasureHolder);
				yield return new DataDashboardItemDescription(DashboardLocalizer.GetString(DashboardStringId.DescriptionItemAxisYMeasure),
					DashboardLocalizer.GetString(DashboardStringId.DescriptionItemAxisYMeasure), ItemKind.SingleNumericMeasure, axisYMeasureHolder);
				yield return new DataDashboardItemDescription(DashboardLocalizer.GetString(DashboardStringId.DescriptionItemMeasureWeight),
					DashboardLocalizer.GetString(DashboardStringId.DescriptionItemMeasureWeight), ItemKind.SingleNumericMeasure, weightHolder);
				yield return new DataDashboardItemDescription(
					DashboardLocalizer.GetString(DashboardStringId.DescriptionArguments),
					DashboardLocalizer.GetString(DashboardStringId.DescriptionArgument),
					DashboardLocalizer.GetString(DashboardStringId.DescriptionArguments),
					ItemKind.Dimension, arguments
				);
			}
		}
		bool IsReady { get { return AxisXMeasure != null && AxisYMeasure != null; } }
		public ScatterChartDashboardItem()
			: base() {
			axisXMeasureHolder = new ScatterAxisXMeasureHolder(this);
			axisXMeasure = InitializeMeasureBox(axisXMeasureHolder, xmlMeasureX);
			axisYMeasureHolder = new ScatterAxisYMeasureHolder(this);
			axisYMeasure = InitializeMeasureBox(axisYMeasureHolder, xmlMeasureY);
			weightHolder = new ChartScatterWeightHolder(this);
			weightBox = InitializeMeasureBox(weightHolder, xmlMeasureWeight);
			legend = new ChartLegend(this);
			axisX = new ScatterChartAxis(new ScatterChartAxisXContainer(this));
			axisY = new ScatterChartAxis(new ScatterChartAxisYContainer(this));
			pointLabel = new ScatterPointLabelOptions(this);
			arguments.CollectionChanged += OnDimensionCollectionChanged;
		}
		protected void OnDimensionCollectionChanged(object sender, NotifyingCollectionChangedEventArgs<Dimension> e) {
			OnDataItemsChanged(e.AddedItems, e.RemovedItems);
		}
		protected override FilterableDashboardItemInteractivityOptions CreateInteractivityOptions() {
			return new DashboardItemInteractivityOptions(false);
		}
		protected internal override string[] GetSelectionDataMemberDisplayNames() {
			if(!IsReady)
				return new string[0];
			return GetSeriesDataMembers();
		}
		protected internal string[] GetSeriesDataMembers() {
			return IterateDimensions().Select(dim => dim.DisplayName).ToArray();
		}
		protected internal string[] GetDimensionActualIds() {
			return IterateDimensions().Select(dim => dim.ActualId).ToArray();
		}
		IList<Dimension> IterateDimensions() {
			if(Arguments.Count == 0)
				return new Dimension[0];
			if(IsDrillDownEnabled)
				return new Dimension[] { CurrentDrillDownDimension };
			return Arguments;
		}
		protected override string[] GetCurrentAxisName() {
			return new string[] { DimensionAxisName };
		}
		protected internal override void SaveToXml(XElement element) {
			base.SaveToXml(element);
			ColorScheme.SaveToXml(element);
			if(Rotated != DefaultRotated)
				element.Add(new XAttribute(xmlRotated, rotated));
			if(legend.ShouldSerialize())
				element.Add(legend.SaveToXml());
			if(axisX.ShouldSerialize()) {
				XElement axisXElement = new XElement(xmlAxisX);
				axisX.SaveToXml(axisXElement);
				element.Add(axisXElement);
			}
			if(axisY.ShouldSerialize()) {
				XElement axisYElement = new XElement(xmlAxisY);
				axisY.SaveToXml(axisYElement);
				element.Add(axisYElement);
			}
			if(pointLabel.ShouldSerialize())
				element.Add(pointLabel.SaveToXml());
			axisXMeasure.SaveToXml(element);
			axisYMeasure.SaveToXml(element);
			weightBox.SaveToXml(element);
			arguments.SaveToXml(element, xmlDimensions, xmlDimension);
		}
		protected internal override void LoadFromXmlInternal(XElement element) {
			base.LoadFromXmlInternal(element);
			ColorScheme.LoadFromXml(element);
			string rotatedString = XmlHelper.GetAttributeValue(element, xmlRotated);
			if(!String.IsNullOrEmpty(rotatedString))
				rotated = XmlHelper.FromString<bool>(rotatedString);
			legend.LoadFromXml(element);
			XElement chartAxisXElement = element.Element(xmlAxisX);
			if(chartAxisXElement != null) {
				ScatterChartAxis chartAxisX = new ScatterChartAxis(new ScatterChartAxisXContainer(this));
				chartAxisX.LoadFromXml(chartAxisXElement);
				this.axisX = chartAxisX;
			}
			XElement chartAxisYElement = element.Element(xmlAxisY);
			if(chartAxisYElement != null) {
				ScatterChartAxis chartAxisY = new ScatterChartAxis(new ScatterChartAxisYContainer(this));
				chartAxisY.LoadFromXml(chartAxisYElement);
				this.axisY = chartAxisY;
			}
			pointLabel.LoadFromXml(element);
			axisXMeasure.LoadFromXml(element);
			axisYMeasure.LoadFromXml(element);
			weightBox.LoadFromXml(element);
			arguments.LoadFromXml(element, xmlDimensions, xmlDimension);
		}
		protected override void OnEndLoading() {
			base.OnEndLoading();
			axisXMeasure.OnEndLoading();
			axisYMeasure.OnEndLoading();
			weightBox.OnEndLoading();
			arguments.OnEndLoading(DataItemRepository, this);
		}
		protected override Dictionary<string, int> GetColorDimensionsByAxis(string axisName, IList<DimensionDefinition> actualColorDimensionDefinitions) {
			 return FilterColorDimensions(Arguments, actualColorDimensionDefinitions);
		}
		protected override Dictionary<string, Dictionary<string, int>> GetColorDimensionsByAxes(List<DimensionDefinition> dimensionDefinitions) {
			Dictionary<string, Dictionary<string, int>> dimensionsByAxes = new Dictionary<string, Dictionary<string, int>>(); 
			dimensionsByAxes.Add(DimensionAxisName, FilterColorDimensions(Arguments, dimensionDefinitions));
			dimensionsByAxes.Add(EmptyAxisName, new Dictionary<string, int>());
			return dimensionsByAxes;
		}
		internal override string[] GetColorPath() {
			Dictionary<string, int> arguments = GetColorDimensionsByAxis(DimensionAxisName);
			List<int> indexes = new List<int>();
			return GetColorPath(arguments, indexes);
		}
		protected override void GetMetadataInternal(HierarchicalMetadataBuilder builder) {
			base.GetMetadataInternal(builder);
			List<Dimension> dimensions = new List<Dimension>();
			int lastDimensionIndex = Arguments.Count - 1;
			if(IsDrillDownEnabled)
				lastDimensionIndex = CurrentDrillDownLevel.Value;
			for(int i = 0; i <= lastDimensionIndex; i++) {
				dimensions.Add(Arguments[i]);
			}
			builder.SetRowHierarchyDimensions(DimensionAxisName, dimensions);
			builder.SetColumnHierarchyDimensions(EmptyAxisName, new Dimension[0]);
			if(AxisXMeasure != null)
				builder.AddMeasure(AxisXMeasure);
			if(AxisYMeasure != null)
				builder.AddMeasure(AxisYMeasure);
			if(Weight != null)
				builder.AddMeasure(Weight);
			builder.AddColorMeasureDescriptors(this);
		}
		internal override DashboardItemDataDescription CreateDashboardItemDataDescription() {
			DashboardItemDataDescription description = base.CreateDashboardItemDataDescription();
			description.AddMeasure(AxisXMeasure);
			description.AddMeasure(AxisYMeasure);
			description.AddMeasure(Weight);
			foreach(Dimension dimension in Arguments)
				description.AddMainDimension(dimension);
			return description;
		}
		internal override void AssignDashboardItemDataDescriptionCore(DashboardItemDataDescription description) {
			base.AssignDashboardItemDataDescriptionCore(description);
			Arguments.AddRange(description.MainDimensions);
			Arguments.AddRange(description.AdditionalDimensions);
			AssignDimension(description.Latitude, Arguments);
			AssignDimension(description.Longitude, Arguments);
			AssignDimension(description.SparklineArgument, HiddenDimensions);
			foreach(Measure measure in description.Measures) {
				if(AxisXMeasure  == null)
					AxisXMeasure = measure;
				else if(AxisYMeasure == null)
					AxisYMeasure = measure;
				else if(Weight == null)
					Weight = measure;
				else
					AssignMeasure(measure, HiddenMeasures);
			}
		}
		protected override SliceDataQuery GetDataQueryInternal(IActualParametersProvider provider) {
			if(!IsReady)
				return new SliceDataQuery();
			var dimensions = IsDrillDownEnabled ?
				Arguments.Take(Arguments.IndexOf(CurrentDrillDownDimension) + 1).ToList() :
				Arguments.ToList();
			var invisibleDimensions = IsDrillDownEnabled ?
				Arguments.Except(dimensions).ToArray() :
				new Dimension[0];
			var filterDimensions = QueryFilterDimensions.Concat(invisibleDimensions).ToList(); 
			SliceDataQueryBuilder queryBuilder;
			ItemModelBuilder itemBuilder = new ItemModelBuilder(DataSourceModel.DataSourceInfo, GetDataItemUniqueName, provider);
			itemBuilder.MeasureConvertToDecimalForNonNumerical = true;
			if(IsBackCompatibilityDataSlicesRequired) {
				queryBuilder = SliceDataQueryBuilder.CreateWithPivotModel(itemBuilder, dimensions, new Dimension[0],
					QueryMeasures, filterDimensions, GetQueryFilterCriteria(provider));
			} else {
				queryBuilder = SliceDataQueryBuilder.CreateEmpty(itemBuilder, filterDimensions, GetQueryFilterCriteria(provider));
				queryBuilder.AddSlice(dimensions, QueryMeasures);
				queryBuilder.SetAxes(dimensions, new Dimension[0]);
			}
			return queryBuilder.FinalQuery();
		}
		protected override Measure[][] GetColoringMeasuresInternal() {
			return new Measure[0][];
		}
		protected internal override bool IsSortingByMeasureEnabled(Dimension dimension, DashboardItemViewModel model) {
			return false;
		}
		protected internal override DashboardItemViewModel CreateViewModel() {
			ChartSeriesTemplateViewModel template = null;
			if(IsReady) {
				IList<string> dataMembers = new List<string>();
				if(AxisYMeasure != null)
					dataMembers.Add(AxisYMeasure.ActualId);
				if(Weight != null)
					dataMembers.Add(Weight.ActualId);
				template = new ChartSeriesTemplateViewModel {
					Name = GetSeriesTemplateName(),
					SeriesType = Weight != null ? ChartSeriesViewModelType.Weighted : ChartSeriesViewModelType.Point,
					DataMembers = dataMembers.ToArray(),
					PointLabel = pointLabel.CreateViewModel(),
					OnlyPercentValues = AxisYMeasure != null && AxisYMeasure.NumericFormat.FormatType == DataItemNumericFormatType.Percent,
					ColorMeasureID = ChartDashboardItemBase.ColorMeasure
				};
			}
			ChartPaneViewModel paneViewModel = new ChartPaneViewModel {
				SeriesTemplates = template != null ? new[] { template } : new ChartSeriesTemplateViewModel[0],
				PrimaryAxisY = axisY.CreateViewModel()
			};
			return new ScatterChartDashboardItemViewModel(this, paneViewModel) {
				AxisX = axisX.CreateViewModel(),
				Legend = new ChartLegendViewModel(legend),
				Rotated = rotated,
				AxisXDataMember = AxisXMeasure != null ? AxisXMeasure.ActualId : null,
				AxisXPercentValues = AxisXMeasure != null && AxisXMeasure.NumericFormat.FormatType == DataItemNumericFormatType.Percent
			};
		}
		string GetSeriesTemplateName() {
			if(CurrentDrillDownDimension != null)
				return CurrentDrillDownDimension.DisplayName;
			if(Arguments.Count > 0)
				return string.Join(" - ", Arguments.Select(arg => arg.DisplayName));
			return DashboardLocalizer.GetString(DashboardStringId.ChartTotalValue);
		}
		protected override void FillEditNameDescriptions(IList<EditNameDescription> descriptions) {
			base.FillEditNameDescriptions(descriptions);
			if(AxisXMeasure != null)
				descriptions.Add(new EditNameDescription(DashboardLocalizer.GetString(DashboardStringId.DescriptionItemAxisXMeasure), new[] { AxisXMeasure }));
			if(AxisYMeasure != null)
				descriptions.Add(new EditNameDescription(DashboardLocalizer.GetString(DashboardStringId.DescriptionItemAxisYMeasure), new[] { AxisYMeasure }));
			if(Weight != null)
				descriptions.Add(new EditNameDescription(DashboardLocalizer.GetString(DashboardStringId.DescriptionItemMeasureWeight), new[] { Weight }));
		}
		protected override IEnumerable<Measure> GetQueryVisibleMeasures() {
			if(AxisXMeasure != null)
				yield return AxisXMeasure;
			if(AxisYMeasure != null)
				yield return AxisYMeasure;
			if(Weight != null)
				yield return Weight;
		}
		protected internal override bool IsSortingEnabled(Dimension dimension) {
			return false;
		}
		protected internal override bool CanSpecifySortMode(Dimension dimension) {
			return false;
		}
		protected override SummaryTypeInfo GetSummaryTypeInfo(Measure measure) {
			if(measure == AxisXMeasure || measure == AxisYMeasure || measure == Weight)
				return SummaryTypeInfo.Number;
			return base.GetSummaryTypeInfo(measure);
		}
		protected override DimensionGroupIntervalInfo GetDimensionGroupIntervalInfo(Dimension dimension) {
			if(arguments.Contains(dimension))
				return DimensionGroupIntervalInfo.Default;
			return base.GetDimensionGroupIntervalInfo(dimension);
		}
	}
}
