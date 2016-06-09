#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{                                                                   }
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

using DevExpress.Utils;
using DevExpress.XtraCharts.Localization;
using DevExpress.XtraCharts.Native;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Collections.Generic;
using System;
using DevExpress.Utils.Design;
using DevExpress.XtraCharts.Design;
namespace DevExpress.XtraCharts.Designer.Native {
	[
	ModelOf(typeof(SeriesBase)),
	HasOptionsControl,
	TypeConverter(typeof(SeriesBaseTypeConverter))]
	public class DesignerSeriesModelBase : DesignerChartElementModelBase {
		readonly SeriesBase series;
		ChartColorizerBaseModel colorizerModel;
		SeriesLabelBaseModel labelModel;
		SeriesViewModelBase viewModel;
		TopNOptionsModel topNOptionsModel;
		ChangeViewTypeCommand changeViewTypeCommand;
		DataFilterCollectionModel dataFiltersModel;
		protected ScaleType[] SeriesScaleTypes {
			get {
				return new ScaleType[0];
			}
		}
		protected ScaleType[] ArgumentScaleTypes {
			get {
				return series != null ? new ScaleType[] { series.ArgumentScaleType } : new ScaleType[0];
			}
		}
		protected ScaleType[] ColorScaleTypes {
			get {
				return new ScaleType[] { ScaleType.Numerical, ScaleType.Qualitative };
			}
		}
		protected ScaleType[] ToolTipHintScaleTypes {
			get {
				return new ScaleType[0];
			}
		}
		internal SeriesBase SeriesBase { get { return series; } }
		internal override string ChartTreeText {
			get {
				return "Auto-created series";
			}
		}
		protected internal override ChartElement ChartElement { get { return series; } }
		protected internal override bool HasOptionsControl { get { return true; } }
		[Editor(typeof(DataMemberModelEditor), typeof(UITypeEditor)),
		Category("Data")]
		public string ArgumentDataMember {
			get { return SeriesBase.ArgumentDataMember; }
			set { SetProperty("ArgumentDataMember", value); }
		}
		[Category("Behavior"), TypeConverter(typeof(ArgumentScaleTypeTypeConverter))]
		public ScaleType ArgumentScaleType {
			get { return SeriesBase.ArgumentScaleType; }
			set { SetProperty("ArgumentScaleType", value); }
		}
		[Category("Behavior"), TypeConverter(typeof(BooleanTypeConverter))]
		public bool CheckableInLegend {
			get { return SeriesBase.CheckableInLegend; }
			set { SetProperty("CheckableInLegend", value); }
		}
		[Category("Behavior"), TypeConverter(typeof(BooleanTypeConverter))]
		public bool CheckedInLegend {
			get { return SeriesBase.CheckedInLegend; }
			set { SetProperty("CheckedInLegend", value); }
		}
		[Editor(typeof(DataMemberModelEditor), typeof(UITypeEditor)),
		Category("Data")]
		public string ColorDataMember {
			get { return SeriesBase.ColorDataMember; }
			set { SetProperty("ColorDataMember", value); }
		}
		[TypeConverter(typeof(ExpandableObjectConverter)),
		Editor("DevExpress.XtraCharts.Designer.Native.ColorizerUITypeEditor," + AssemblyInfo.SRAssemblyChartsWizard, typeof(UITypeEditor)),
		Category("Appearance")]
		public ChartColorizerBaseModel Colorizer {
			get { return colorizerModel; }
			set { SetProperty("Colorizer", value); }
		}
		[Category("Behavior"), TypeConverter(typeof(DefaultBooleanConverter))]
		public DefaultBoolean CrosshairEnabled {
			get { return SeriesBase.CrosshairEnabled; }
			set { SetProperty("CrosshairEnabled", value); }
		}
		[Category("Behavior"), TypeConverter(typeof(DefaultBooleanConverter))]
		public DefaultBoolean CrosshairHighlightPoints {
			get { return SeriesBase.CrosshairHighlightPoints; }
			set { SetProperty("CrosshairHighlightPoints", value); }
		}
		[Editor(typeof(ModelPatternEditor), typeof(UITypeEditor)),
		Category("Behavior")]
		public string CrosshairLabelPattern {
			get { return SeriesBase.CrosshairLabelPattern; }
			set { SetProperty("CrosshairLabelPattern", value); }
		}
		[Category("Behavior"), TypeConverter(typeof(DefaultBooleanConverter))]
		public DefaultBoolean CrosshairLabelVisibility {
			get { return SeriesBase.CrosshairLabelVisibility; }
			set { SetProperty("CrosshairLabelVisibility", value); }
		}
		[Category("Data"), Editor(typeof(DataFilterModelCollectionEditor), typeof(UITypeEditor))]
		public DataFilterCollectionModel DataFilters { get { return dataFiltersModel; } }
		[Category("Elements")]
		public SeriesLabelBaseModel Label {
			get { return labelModel; }
			set { SetProperty("Label", value); }
		}
		[
		PropertyForOptions,
		DependentUpon("Visible"),
		Category("Appearance"),
		TypeConverter(typeof(DefaultBooleanConverter))]
		public DefaultBoolean LabelsVisibility {
			get { return SeriesBase.LabelsVisibility; }
			set { SetProperty("LabelsVisibility", value); }
		}
		[Category("Behavior")]
		public string LegendText {
			get { return SeriesBase.LegendText; }
			set { SetProperty("LegendText", value); }
		}
		[Editor(typeof(ModelPatternEditor), typeof(UITypeEditor)),
		Category("Behavior")]
		public string LegendTextPattern {
			get { return SeriesBase.LegendTextPattern; }
			set { SetProperty("LegendTextPattern", value); }
		}
		[Category("Behavior")]
		public SortingMode SeriesPointsSorting {
			get { return SeriesBase.SeriesPointsSorting; }
			set { SetProperty("SeriesPointsSorting", value); }
		}
		[Category("Behavior"), TypeConverter(typeof(SeriesPointKeyConverter))]
		public SeriesPointKey SeriesPointsSortingKey {
			get { return SeriesBase.SeriesPointsSortingKey; }
			set { SetProperty("SeriesPointsSortingKey", value); }
		}
		[
		PropertyForOptions,
		DependentUpon("Visible"),
		Category("Behavior"),
		TypeConverter(typeof(BooleanTypeConverter))]
		public bool ShowInLegend {
			get { return SeriesBase.ShowInLegend; }
			set { SetProperty("ShowInLegend", value); }
		}
		[Editor(typeof(SummaryFunctionModelEditor), typeof(UITypeEditor)),
		Category("Data")]
		public string SummaryFunction {
			get { return SeriesBase.SummaryFunction; }
			set { SetProperty("SummaryFunction", value); }
		}
		[Category("Behavior"), TypeConverter(typeof(DefaultBooleanConverter))]
		public DefaultBoolean ToolTipEnabled {
			get { return SeriesBase.ToolTipEnabled; }
			set { SetProperty("ToolTipEnabled", value); }
		}
		[Editor(typeof(DataMemberModelEditor), typeof(UITypeEditor)),
		Category("Data")]
		public string ToolTipHintDataMember {
			get { return SeriesBase.ToolTipHintDataMember; }
			set { SetProperty("ToolTipHintDataMember", value); }
		}
		[Editor(typeof(ToolTipPointModelPatternEditor), typeof(UITypeEditor)),
		Category("Behavior")]
		public string ToolTipPointPattern {
			get { return SeriesBase.ToolTipPointPattern; }
			set { SetProperty("ToolTipPointPattern", value); }
		}
		[Editor(typeof(ToolTipSeriesModelPatternEditor), typeof(UITypeEditor)),
		Category("Behavior")]
		public string ToolTipSeriesPattern {
			get { return SeriesBase.ToolTipSeriesPattern; }
			set { SetProperty("ToolTipSeriesPattern", value); }
		}
		[Category("Behavior")]
		public TopNOptionsModel TopNOptions { get { return topNOptionsModel; } }
		[
		Editor(typeof(ValueDataMemberCollectionEditor), typeof(UITypeEditor)),
		TypeConverter(typeof(ValueDataMemberModelCollectionConverter)),
		Category("Data")
		]
		public ValueDataMemberCollection ValueDataMembers {
			get { return SeriesBase.ValueDataMembers; }
		}
		[Category("Behavior"), TypeConverter(typeof(ValueScaleTypeTypeConverter))]
		public ScaleType ValueScaleType {
			get { return SeriesBase.ValueScaleType; }
			set { SetProperty("ValueScaleType", value); }
		}
		[TypeConverter(typeof(ExpandableObjectConverter)),
		PropertyForOptions,
		AllocateToGroup("View"),
		Category("Elements")]
		public SeriesViewModelBase View { get { return viewModel; } }
		[PropertyForOptions,
		Browsable(false)]
		public ViewType ViewType {
			get { return SeriesViewFactory.GetViewType(series.View); }
			set {
				if (SeriesViewFactory.GetViewType(series.View).Equals(value)) {
					series.ChangeView(value);
				}
			}
		}
		[PropertyForOptions,
		Category("Appearance"),
		TypeConverter(typeof(BooleanTypeConverter))]
		public bool Visible {
			get { return SeriesBase.Visible; }
			set { SetProperty("Visible", value); }
		}
		public DesignerSeriesModelBase(CommandManager commandManager)
			: base(commandManager) {
		}
		public DesignerSeriesModelBase(SeriesBase series, CommandManager commandManager)
			: base(commandManager) {
			this.series = series;
			this.changeViewTypeCommand = new ChangeViewTypeCommand(this, commandManager);
		}
		protected internal override bool IsSupportsDataControl(bool isDesignTime) {
			return isDesignTime ? true : SeriesDataBindingUtils.GetDataSource(series) != null;
		}
		protected override void ProcessMessage(ViewMessage message) {
			if (message.Name == "ViewType")
				changeViewTypeCommand.Execute(message.Value);
			else
				base.ProcessMessage(message);
		}
		protected override void AddChildren() {
			if (viewModel != null)
				Children.Add(viewModel);
			if (colorizerModel != null)
				Children.Add(colorizerModel);
			if (labelModel != null)
				Children.Add(labelModel);
			if (topNOptionsModel != null)
				Children.Add(topNOptionsModel);
			if (dataFiltersModel != null)
				Children.Add(dataFiltersModel);
			base.AddChildren();
		}
		public override ChartTreeElement GetTreeElement(ChartTreeCollectionElement parentCollection) {
			return new ChartTreeElement(this, parentCollection);
		}
		public override List<DataMemberInfo> GetDataMembersInfo() {
			List<DataMemberInfo> dataMembersInfo = new List<DataMemberInfo>();
			DesignerChartModel chartModel = FindParent<DesignerChartModel>();
			if (viewModel != null && chartModel != null) {
				dataMembersInfo.Add(new DataMemberInfo("SeriesDataMember", "Series", chartModel.Chart.DataContainer.SeriesDataMember, SeriesScaleTypes));
				dataMembersInfo.Add(new DataMemberInfo("ArgumentDataMember", "Argument", series.ArgumentDataMember, ArgumentScaleTypes));
				List<DataMemberInfo> valuesDMInfo = viewModel.GetDataMembersInfo();
				if (valuesDMInfo != null)
					dataMembersInfo.AddRange(valuesDMInfo);
				dataMembersInfo.Add(new DataMemberInfo("ColorDataMember", "Color", series.ColorDataMember, ColorScaleTypes));
				dataMembersInfo.Add(new DataMemberInfo("ToolTipHintDataMember", "ToolTip Hint", series.ToolTipHintDataMember, ToolTipHintScaleTypes));
			}
			return dataMembersInfo;
		}
		public override void Update() {
			if (this.viewModel == null || this.viewModel.ChartElement != SeriesBase.View)
				this.viewModel = ModelHelper.CreateSeriesViewModelInstance(SeriesBase.View, CommandManager);
			if (this.colorizerModel == null || this.colorizerModel.Colorizer != SeriesBase.Colorizer)
				this.colorizerModel = (ChartColorizerBaseModel)ModelHelper.CreateChartColorizerBaseModelInstance(SeriesBase.Colorizer as ChartColorizerBase, CommandManager);
			if (this.labelModel == null || this.labelModel.ChartElement != SeriesBase.Label)
				this.labelModel = ModelHelper.CreateSeriesLabelBaseModelInstance(SeriesBase.Label, CommandManager);
			if (this.labelModel != null) 
				this.labelModel.SeriesModel = this;
			if (this.topNOptionsModel == null || this.topNOptionsModel.ChartElement != SeriesBase.TopNOptions)
				this.topNOptionsModel = new TopNOptionsModel(SeriesBase.TopNOptions, CommandManager);
			if (this.dataFiltersModel == null || !this.dataFiltersModel.ChartCollection.Equals(SeriesBase.DataFilters))
				this.dataFiltersModel = new DataFilterCollectionModel(SeriesBase.DataFilters, CommandManager, Chart);
			ClearChildren();
			AddChildren();
			base.Update();
		}
	}
	[ModelOf(typeof(Series)), TypeConverter(typeof(SeriesTypeConverter))]
	public class DesignerSeriesModel : DesignerSeriesModelBase, ISupportModelVisibility {
		readonly Series series;
		readonly SeriesPointCollectionModel pointsModel;
		ChartImageModel tooltipImageModel;
		internal Series Series { get { return series; } }
		internal override string ChartTreeText {
			get {
				string incompatible = "";
				if (Parent != null && Parent.Parent != null && series.View != null) {
					var chart = (Chart)Parent.Parent.ChartElement;
					if (chart.Diagram != null) {
						Type diagramType = chart.Diagram.GetType();
						bool isCompatible = series.View.DiagramType == diagramType;
						if (!isCompatible)
							incompatible = ChartLocalizer.GetString(ChartStringId.IncompatibleSeriesView);
					}
				}
				return series != null ? series.Name + " " + incompatible : base.ChartTreeText;
			}
		}
		protected internal override ChartElement ChartElement { get { return series; } }
		protected internal override string ChartTreeImageKey {
			get { return View != null ? View.ChartTreeImageKey : base.ChartTreeImageKey; }
		}
		[PropertyForOptions,
		Category("Misc")]
		public string Name {
			get { return Series.Name; }
			set { SetProperty("Name", value); }
		}
		[Browsable(false)]
		public object DataSource {
			get { return Series.DataSource; }
			set { SetProperty("DataSource", value); }
		}
		[TypeConverter(typeof(ExpandableObjectConverter)),
		Category("Data")]
		public ChartImageModel ToolTipImage { get { return tooltipImageModel; } }
		[Browsable(false)]
		public SeriesPointCollectionModel Points { get { return pointsModel; } }
		public DesignerSeriesModel(Series series, CommandManager commandManager)
			: base(series, commandManager) {
			this.series = series;
			this.pointsModel = new SeriesPointCollectionModel(series.Points, commandManager, Chart);
		}
		protected override void AddChildren() {
			if (tooltipImageModel != null)
				Children.Add(tooltipImageModel);
			if (pointsModel != null)
				Children.Add(pointsModel);
			base.AddChildren();
		}
		public override void Update() {
			this.tooltipImageModel = new ChartImageModel(Series.ToolTipImage, CommandManager);
			this.pointsModel.Update();
			ClearChildren();
			AddChildren();
			base.Update();
		}
		public override ChartTreeElement GetTreeElement(ChartTreeCollectionElement parentCollection) {
			if (series.IsAutoCreated)
				return null;
			else
				return new ChartTreeElement(this, parentCollection);
		}
		public override List<DataMemberInfo> GetDataMembersInfo() {
			List<DataMemberInfo> dataMembersInfo = new List<DataMemberInfo>();
			if (View != null) {
				dataMembersInfo.Add(new DataMemberInfo("ArgumentDataMember", "Argument", series.ArgumentDataMember, ArgumentScaleTypes));
				List<DataMemberInfo> valuesDMInfo = View.GetDataMembersInfo();
				if (valuesDMInfo != null)
					dataMembersInfo.AddRange(valuesDMInfo);
				dataMembersInfo.Add(new DataMemberInfo("ColorDataMember", "Color", series.ColorDataMember, ColorScaleTypes));
				dataMembersInfo.Add(new DataMemberInfo("ToolTipHintDataMember", "ToolTip Hint", series.ToolTipHintDataMember, ToolTipHintScaleTypes));
			}
			return dataMembersInfo;
		}
	}
}
