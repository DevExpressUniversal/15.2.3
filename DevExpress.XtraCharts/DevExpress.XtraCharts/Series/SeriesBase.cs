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

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Drawing.Design;
using DevExpress.Charts.Native;
using DevExpress.Charts.NotificationCenter;
using DevExpress.Data.Browsing;
using DevExpress.Data.ChartDataSources;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Localization;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts {
	[DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
						"System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design"),
	TypeConverter(typeof(SeriesBaseTypeConverter))]
	public class SeriesBase : ChartElement, ISeriesBase, ISeriesTemplate, IHitTest, IXtraSupportCreateContentPropertyValue, IXtraSupportDeserializeCollectionItem, IXtraSupportAfterDeserialize, ITopNOptions, IPointsFilterOptions, ILegendItem {
		const bool DefaultVisible = true;
		const bool DefaultShowInLegend = true;
		const bool DefaultCheckedInLegend = true;
		const bool DefaultCheckableInLegend = true;
		const ScaleType DefaultArgumentScaleType = ScaleType.Auto;
		const ScaleType DefaultValueScaleType = ScaleType.Numerical;
		const SortingMode DefaultSeriesPointsSorting = SortingMode.None;
		const SeriesPointKey DefaultSeriesPointsSortingKey = SeriesPointKey.Argument;
		const DefaultBoolean DefaultToolTipEnabled = DefaultBoolean.Default;
		const DefaultBoolean DefaultCrosshairEnabled = DefaultBoolean.Default;
		const DefaultBoolean DefaultCrosshairLabelVisibility = DefaultBoolean.Default;
		const DefaultBoolean DefaultLabelsVisibility = DefaultBoolean.Default;
		const DefaultBoolean DefaultCrosshairHighlightPoints = DefaultBoolean.Default;
		const string DefaultToolTipSeriesPattern = "{S}";
		static void ThrowIncompatibleScaleTypeException(ChartStringId stringId, ScaleType scaleType, SeriesViewBase view) {
			throw new ArgumentException(String.Format(ChartLocalizer.GetString(stringId), GetScaleTypeName(scaleType), view == null ? String.Empty : view.StringId));
		}
		static void ThrowInvalidScaleTypeException(ChartStringId stringId, string dataMember, ScaleType scaleType) {
			throw new ArgumentException(String.Format(ChartLocalizer.GetString(stringId), dataMember, Series.GetScaleTypeName(scaleType)));
		}
		static void ThrowIncompatibleSummaryFunctionException(ChartStringId stringId, SummaryFunctionDescription description, ScaleType scaleType) {
			throw new ArgumentException(String.Format(ChartLocalizer.GetString(stringId), description.DisplayName, GetScaleTypeName(scaleType)));
		}
		internal static string GetScaleTypeName(ScaleType scaleType) {
			return ScaleTypeUtils.GetName(scaleType);
		}
		readonly IColorizer defaultColorizer = new ColorObjectColorizer();
		IColorizer colorizer;
		ColorProvider colorProvider;
		bool visible = DefaultVisible;
		bool showInLegend = DefaultShowInLegend;
		bool checkedInLegend = DefaultCheckedInLegend;
		bool checkableInLegend = DefaultCheckableInLegend;
		string argumentDataMember = null;
		string colorDataMember = null;
		string toolTipHintDataMember = null;
		string summaryFunction = String.Empty;
		string legendText = String.Empty;
		string toolTipSeriesPattern = DefaultToolTipSeriesPattern;
		string toolTipPointPattern;
		string crosshairLabelPattern;
		string legendTextPattern;
		AspxSerializerWrapper<SeriesViewBase> viewSerializerWrapper;
		AspxSerializerWrapper<SeriesLabelBase> labelSerializerWrapper;
		AspxSerializerWrapper<PointOptions> legendPointOptionsSerializerWrapper;
		AspxSerializerWrapper<PointOptions> pointOptionsSerializerWrapper;
		DataFilterCollection dataFilters;
		ScaleType argumentScaleType = DefaultArgumentScaleType;
		ScaleType argumentScaleByDataSource = ScaleType.Auto;
		ScaleType valueScaleType = ScaleType.Numerical;
		SeriesViewBase view;
		SeriesLabelBase label;
		SeriesPointKey pointsSortingKey = SeriesPointKey.Argument;
		SortingMode pointsSorting = SortingMode.None;
		ValueDataMemberCollection valueDataMembers;
		PointOptions legendPointOptions;
		TopNOptions topNOptions;
		DefaultBoolean toolTipEnabled = DefaultToolTipEnabled;
		DefaultBoolean crosshairEnabled = DefaultCrosshairEnabled;
		DefaultBoolean crosshairLabelVisibility = DefaultCrosshairLabelVisibility;
		DefaultBoolean labelsVisibility = DefaultLabelsVisibility;
		DefaultBoolean crosshairHighlightPoints = DefaultCrosshairHighlightPoints;
		ChartElementSynchronizer pointOptionsSynchronizer;
		DataContext DataContext {
			get { return ContainerAdapter != null ? ContainerAdapter.DataContext : null; }
		}
		protected bool DesignMode {
			get { return (Chart != null) && Chart.Container.DesignMode; }
		}
		protected virtual bool HasPoints {
			get { return false; }
		}
		internal ColorProvider ColorProvider { get { return colorProvider; } }
		internal bool ActualCrosshairEnabled {
			get {
				Chart chart = Chart;
				if (chart == null || DesignMode || !chart.SupportCrosshair)
					return false;
				if (CrosshairEnabled != DefaultBoolean.Default)
					return CrosshairEnabled == DefaultBoolean.False ? false : true;
				if (chart.CrosshairEnabled != DefaultBoolean.Default)
					return chart.CrosshairEnabled == DefaultBoolean.False ? false : true;
				return true;
			}
		}
		internal bool ActualToolTipEnabled {
			get {
				Chart chart = Chart;
				if (chart == null || DesignMode || !chart.SupportToolTips)
					return false;
				if (ToolTipEnabled != DefaultBoolean.Default)
					return ToolTipEnabled == DefaultBoolean.False ? false : true;
				if (chart.ToolTipEnabled != DefaultBoolean.Default)
					return chart.ToolTipEnabled == DefaultBoolean.False ? false : true;
				return !chart.SupportCrosshair && !(chart.Diagram is SimpleDiagram);
			}
		}
		internal bool ActualCrosshairLabelVisibility {
			get {
				if (CrosshairLabelVisibility != DefaultBoolean.Default)
					return CrosshairLabelVisibility == DefaultBoolean.False ? false : true;
				return (Chart != null) ? Chart.CrosshairOptions.ShowCrosshairLabels : false;
			}
		}
		internal bool ActualCrosshairHighlightPoints {
			get {
				if (CrosshairHighlightPoints == DefaultBoolean.Default)
					return (Chart != null) ? Chart.CrosshairOptions.HighlightPoints : false;
				else
					return CrosshairHighlightPoints == DefaultBoolean.True;
			}
		}
		internal bool IsSummaryBinding {
			get { return !String.IsNullOrEmpty(summaryFunction); }
		}
		internal bool IsSupportedLabel {
			get { return view != null && view.IsSupportedLabel; }
		}
		internal bool IsSupportedPointOptions {
			get { return view != null && view.IsSupportedPointOptions; }
		}
		internal bool IsToolTipsSupported {
			get { return view != null && view.IsSupportedToolTips; }
		}
		internal bool ShouldApplyTopNOptions {
			get { return valueScaleType == DevExpress.XtraCharts.ScaleType.Numerical && view != null && view.PointDimension == 1; }
		}
		internal bool ShouldUseTopNOthers {
			get { return ActualArgumentScaleType == ScaleType.Qualitative; }
		}
		internal string[] ActualValueDataMembers {
			get { return IsSummaryBinding ? new SummaryFunctionParser(summaryFunction).Arguments : ValueDataMembers.GetArray(); }
		}
		internal string ActualLegendTextPattern {
			get {
				if (!String.IsNullOrEmpty(legendTextPattern))
					return legendTextPattern;
				return view.LabelPatternToLegendPattern();
			}
		}
		internal IColorizer ActualColorizer {
			get {
				if (colorizer == null)
					return defaultColorizer;
				return colorizer;
			}
		}
		protected internal Chart Chart {
			get {
				return (Owner != null) ? Owner.Chart : null;
			}
		}
		protected internal new DataContainer Owner {
			get { return (DataContainer)base.Owner; }
			set {
				if (value != base.Owner)
					base.Owner = value;
			}
		}
		protected internal DataContainer DataContainer {
			get {
				return Owner;
			}
		}
		protected internal bool ShouldBeDrawnOnDiagram {
			get {
				if (Chart == null)
					return false;
				bool useLegendCheckBox = Chart.Legend.UseCheckBoxes && checkableInLegend;
				return useLegendCheckBox ? visible && checkedInLegend : visible;
			}
		}
		[Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool ActualLabelsVisibility {
			get {
				if (LabelsVisibility == DefaultBoolean.Default)
					if (((Chart != null) && Chart.Is3DDiagram) || ((Chart != null) && (Chart.Diagram is SimpleDiagram)) ||
						((Chart != null) && (Chart.Container != null) && (Chart.Container.ControlType == ChartContainerType.XRControl) && (Label != null)))
						return Label.DefaultVisible;
					else
						return false;
				else
					return LabelsVisibility == DefaultBoolean.False ? false : true;
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("SeriesBaseArgumentDataMember"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.SeriesBase.ArgumentDataMember"),
		Category(Categories.Data),
		Editor("DevExpress.XtraCharts.Design.DataMemberEditor," + AssemblyInfo.SRAssemblyChartsWizard, typeof(UITypeEditor)),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty]
		public string ArgumentDataMember {
			get { return argumentDataMember; }
			set {
				if (value != argumentDataMember) {
					CheckArgumentDataMember(GetDataSource(), value, ArgumentScaleType);
					SendNotification(new ElementWillChangeNotification(this));
					argumentDataMember = value;
					ResetDataSourceArgumentScaleType();
					BindingChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("SeriesBaseColorDataMember"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.SeriesBase.ColorDataMember"),
		Category(Categories.Data),
		Editor("DevExpress.XtraCharts.Design.DataMemberEditor," + AssemblyInfo.SRAssemblyChartsWizard, typeof(UITypeEditor)),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty]
		public string ColorDataMember {
			get { return colorDataMember; }
			set {
				if (value != colorDataMember) {
					CheckColorDataMember(GetDataSource(), value);
					SendNotification(new ElementWillChangeNotification(this));
					colorDataMember = value;
					BindingChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("SeriesBaseToolTipHintDataMember"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.SeriesBase.ToolTipHintDataMember"),
		Category(Categories.Data),
		Editor("DevExpress.XtraCharts.Design.DataMemberEditor," + AssemblyInfo.SRAssemblyChartsWizard, typeof(UITypeEditor)),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty]
		public string ToolTipHintDataMember {
			get { return toolTipHintDataMember; }
			set {
				if (value != toolTipHintDataMember) {
					CheckToolTipHintDataMember(GetDataSource(), value);
					SendNotification(new ElementWillChangeNotification(this));
					toolTipHintDataMember = value;
					BindingChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("SeriesBaseValueDataMembers"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.SeriesBase.ValueDataMembers"),
		Category(Categories.Data),
		TypeConverter(typeof(ValueDataMemberCollectionConverter)),
		Editor("DevExpress.XtraCharts.Design.ValueDataMemberCollectionEditor," + AssemblyInfo.SRAssemblyChartsWizard, typeof(UITypeEditor)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)]
		public ValueDataMemberCollection ValueDataMembers {
			get { return valueDataMembers; }
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("SeriesBaseSummaryFunction"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.SeriesBase.SummaryFunction"),
		Category(Categories.Data),
		RefreshProperties(RefreshProperties.All),
		Editor("DevExpress.XtraCharts.Design.SummaryFunctionEditor," + AssemblyInfo.SRAssemblyChartsWizard, typeof(UITypeEditor)),
		XtraSerializableProperty]
		public string SummaryFunction {
			get { return summaryFunction; }
			set {
				if (value != summaryFunction) {
					if (!Loading)
						CheckSummaryFunction(value);
					SendNotification(new ElementWillChangeNotification(this));
					summaryFunction = value;
					BindingChanged();
				}
			}
		}
		[Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		NonTestableProperty,
		XtraSerializableProperty]
		public string ValueDataMembersSerializable {
			get { return valueDataMembers.ToString(); }
			set {
				ValueDataMemberCollection collection = new ValueDataMemberCollection(value);
				if (!Loading && collection.Count != this.view.PointDimension)
					throw new ArgumentException(String.Format(ChartLocalizer.GetString(ChartStringId.MsgIncorrectValueDataMemberCount), this.view.PointDimension));
				valueDataMembers.Assign(collection);
			}
		}
		[Category(Categories.Data),
		Browsable(false),
		XtraSerializableProperty]
		public ConjunctionTypes DataFiltersConjunctionMode {
			get { return dataFilters.ConjunctionMode; }
			set { dataFilters.ConjunctionMode = value; }
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("SeriesBaseDataFilters"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.SeriesBase.DataFilters"),
		TypeConverter(typeof(DevExpress.Utils.Design.CollectionTypeConverter)),
		Category(Categories.Data),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		Editor("DevExpress.XtraCharts.Design.DataFilterCollectionEditor," + AssemblyInfo.SRAssemblyChartsWizard, typeof(UITypeEditor)),
		NestedTagProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Collection, true)]
		public DataFilterCollection DataFilters {
			get { return dataFilters; }
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("SeriesBaseArgumentScaleType"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.SeriesBase.ArgumentScaleType"),
		Category(Categories.Behavior),
		RefreshProperties(RefreshProperties.All),
		TypeConverter(typeof(ArgumentScaleTypeTypeConverter)),
		XtraSerializableProperty]
		public ScaleType ArgumentScaleType {
			get { return argumentScaleType; }
			set {
				if (value != argumentScaleType) {
					if (!Loading)
						ValidatePointsByArgumentScaleType(value);
					SendNotification(new ElementWillChangeNotification(this));
					ScaleType oldValue = argumentScaleType;
					SetArgumentScaleType(value);
					RaisePropertyChanged<ScaleType>("ArgumentScaleType", oldValue, argumentScaleType);
					RaiseControlChanged();
				}
			}
		}
		[Browsable(false),
		NonTestableProperty,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ScaleType ActualArgumentScaleType {
			get {
				if (ArgumentScaleType == ScaleType.Auto)
					return GetActualArgumentScaleType();
				return ArgumentScaleType;
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("SeriesBaseValueScaleType"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.SeriesBase.ValueScaleType"),
		Category(Categories.Behavior),
		RefreshProperties(RefreshProperties.All),
		TypeConverter(typeof(ValueScaleTypeTypeConverter)),
		XtraSerializableProperty]
		public ScaleType ValueScaleType {
			get { return valueScaleType; }
			set {
				if (value != valueScaleType) {
					if (!Loading)
						ValidatePointsByValueScaleType(value);
					SendNotification(new ElementWillChangeNotification(this));
					ScaleType oldValue = valueScaleType;
					SetValueScaleType(value);
					RaisePropertyChanged<ScaleType>("ValueScaleType", oldValue, valueScaleType);
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("SeriesBaseVisible"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.SeriesBase.Visible"),
		TypeConverter(typeof(BooleanTypeConverter)),
		Category(Categories.Appearance),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty]
		public bool Visible {
			get { return visible; }
			set {
				if (value != visible) {
					SendNotification(new ElementWillChangeNotification(this));
					visible = value;
					RaisePropertyChanged<bool>("Visible", !visible, visible);
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("SeriesBaseLabelsVisibility"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.SeriesBase.LabelsVisibility"),
		TypeConverter(typeof(DefaultBooleanConverter)),
		Category(Categories.Appearance),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty]
		public DefaultBoolean LabelsVisibility {
			get { return labelsVisibility; }
			set {
				if (value != labelsVisibility) {
					SendNotification(new ElementWillChangeNotification(this));
					labelsVisibility = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("SeriesBaseShowInLegend"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.SeriesBase.ShowInLegend"),
		TypeConverter(typeof(BooleanTypeConverter)),
		Category(Categories.Behavior),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty]
		public virtual bool ShowInLegend {
			get { return showInLegend; }
			set {
				if (value != showInLegend) {
					SendNotification(new ElementWillChangeNotification(this));
					SetShowInLegend(value);
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("SeriesBaseLegendText"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.SeriesBase.LegendText"),
		Category(Categories.Behavior),
		Localizable(true),
		XtraSerializableProperty]
		public string LegendText {
			get { return legendText; }
			set {
				if (value != legendText) {
					SendNotification(new ElementWillChangeNotification(this));
					legendText = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("SeriesBaseView"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.SeriesBase.View"),
		Category(Categories.Elements),
		RefreshProperties(RefreshProperties.All),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		Editor("DevExpress.XtraCharts.Design.SeriesViewEditor," + AssemblyInfo.SRAssemblyChartsWizard, typeof(UITypeEditor)),
		XtraSerializableProperty(XtraSerializationVisibility.Content, true, false, false, -1)]
		public SeriesViewBase View {
			get { return view; }
			set { UpdateView(value, false); }
		}
		[Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		NonTestableProperty,
		NestedTagProperty]
		public IList ViewSerializable { get { return viewSerializerWrapper; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("SeriesBaseLabel"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.SeriesBase.Label"),
		Category(Categories.Elements),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		XtraSerializableProperty(XtraSerializationVisibility.Content, true)]
		public SeriesLabelBase Label {
			get { return label; }
			set {
				if (!Loading)
					throw new InvalidOperationException(ChartLocalizer.GetString(ChartStringId.MsgLabelSettingRuntimeError));
				if (!Object.ReferenceEquals(value, label)) {
					SendNotification(new ElementWillChangeNotification(this));
					SetSeriesLabel(value);
					RaiseControlChanged();
				}
			}
		}
		[Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		NonTestableProperty,
		NestedTagProperty]
		public IList LabelSerializable {
			get { return labelSerializerWrapper; }
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("SeriesBaseSeriesPointsSorting"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.SeriesBase.SeriesPointsSorting"),
		Category(Categories.Behavior),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty]
		public SortingMode SeriesPointsSorting {
			get { return pointsSorting; }
			set {
				if (value != pointsSorting) {
					SendNotification(new ElementWillChangeNotification(this));
					SortingMode oldPointsSorting = pointsSorting;
					SetPointsSorting(value);
					RaiseSortedPropertyChanged(new PropertyUpdateInfo<SortMode>(this, "SortingMode", (SortMode)oldPointsSorting, (SortMode)pointsSorting));
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("SeriesBaseSeriesPointsSortingKey"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.SeriesBase.SeriesPointsSortingKey"),
		Category(Categories.Behavior),
		TypeConverter(typeof(SeriesPointKeyConverter)),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty]
		public SeriesPointKey SeriesPointsSortingKey {
			get { return pointsSortingKey; }
			set {
				if (value != pointsSortingKey) {
					if (!CheckSortingKey(value))
						throw new ArgumentException(String.Format(ChartLocalizer.GetString(ChartStringId.MsgInvalidSortingKey), value));
					SetSortingKey(value);
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("SeriesBaseTopNOptions"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.SeriesBase.TopNOptions"),
		Category(Categories.Behavior),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NestedTagProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public TopNOptions TopNOptions {
			get { return topNOptions; }
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("SeriesBaseToolTipSeriesPattern"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.SeriesBase.ToolTipSeriesPattern"),
		Category(Categories.Behavior),
		Editor("DevExpress.XtraCharts.Design.ToolTipSeriesPatternEditor," + AssemblyInfo.SRAssemblyChartsWizard, typeof(UITypeEditor)),
		XtraSerializableProperty]
		public string ToolTipSeriesPattern {
			get { return toolTipSeriesPattern; }
			set {
				if (value != toolTipSeriesPattern) {
					SendNotification(new ElementWillChangeNotification(this));
					toolTipSeriesPattern = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("SeriesBaseToolTipPointPattern"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.SeriesBase.ToolTipPointPattern"),
		Category(Categories.Behavior),
		Editor("DevExpress.XtraCharts.Design.ToolTipPointPatternEditor," + AssemblyInfo.SRAssemblyChartsWizard, typeof(UITypeEditor)),
		XtraSerializableProperty]
		public string ToolTipPointPattern {
			get { return toolTipPointPattern; }
			set {
				if (value != toolTipPointPattern) {
					SendNotification(new ElementWillChangeNotification(this));
					toolTipPointPattern = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("SeriesBaseToolTipEnabled"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.SeriesBase.ToolTipEnabled"),
		Category(Categories.Behavior),
		TypeConverter(typeof(DefaultBooleanConverter)),
		XtraSerializableProperty]
		public DefaultBoolean ToolTipEnabled {
			get { return toolTipEnabled; }
			set {
				if (value != toolTipEnabled) {
					SendNotification(new ElementWillChangeNotification(this));
					toolTipEnabled = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("SeriesBaseCrosshairEnabled"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.SeriesBase.CrosshairEnabled"),
		TypeConverter(typeof(DefaultBooleanConverter)),
		Category(Categories.Behavior),
		XtraSerializableProperty]
		public DefaultBoolean CrosshairEnabled {
			get { return crosshairEnabled; }
			set {
				if (value != crosshairEnabled) {
					SendNotification(new ElementWillChangeNotification(this));
					crosshairEnabled = value;
					RaiseControlChanged(new PropertyUpdateInfo(this, "CrosshairEnabled"));
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("SeriesBaseCrosshairLabelVisibility"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.SeriesBase.CrosshairLabelVisibility"),
		TypeConverter(typeof(DefaultBooleanConverter)),
		Category(Categories.Behavior),
		XtraSerializableProperty]
		public DefaultBoolean CrosshairLabelVisibility {
			get { return crosshairLabelVisibility; }
			set {
				if (value != crosshairLabelVisibility) {
					SendNotification(new ElementWillChangeNotification(this));
					crosshairLabelVisibility = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("SeriesBaseCrosshairLabelPattern"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.SeriesBase.CrosshairLabelPattern"),
		Category(Categories.Behavior),
		Editor("DevExpress.XtraCharts.Design.PatternEditor," + AssemblyInfo.SRAssemblyChartsWizard, typeof(UITypeEditor)),
		XtraSerializableProperty]
		public string CrosshairLabelPattern {
			get { return crosshairLabelPattern; }
			set {
				if (value != crosshairLabelPattern) {
					SendNotification(new ElementWillChangeNotification(this));
					crosshairLabelPattern = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("SeriesBaseCrosshairHighlightPoints"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.SeriesBase.CrosshairHighlightPoints"),
		TypeConverter(typeof(DefaultBooleanConverter)),
		Category(Categories.Behavior),
		XtraSerializableProperty]
		public DefaultBoolean CrosshairHighlightPoints {
			get { return crosshairHighlightPoints; }
			set {
				if (value != crosshairHighlightPoints) {
					SendNotification(new ElementWillChangeNotification(this));
					crosshairHighlightPoints = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("SeriesBaseCheckedInLegend"),
#endif
		TypeConverter(typeof(BooleanTypeConverter)),
		Category(Categories.Behavior),
		XtraSerializableProperty,
		DXDisplayNameIgnore]
		public bool CheckedInLegend {
			get { return checkedInLegend; }
			set {
				if (value != checkedInLegend) {
					SendNotification(new ElementWillChangeNotification(this));
					checkedInLegend = value;
					if (ContainerAdapter != null)
						ContainerAdapter.OnLegendItemChecked(new LegendItemCheckedEventArgs(this, value));
					RaiseControlChanged(new PropertyUpdateInfo(this, Series.CheckedInLegendProperty));
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("SeriesBaseCheckableInLegend"),
#endif
		TypeConverter(typeof(BooleanTypeConverter)),
		Category(Categories.Behavior),
		XtraSerializableProperty,
		DXDisplayNameIgnore]
		public bool CheckableInLegend {
			get { return checkableInLegend; }
			set {
				if (value != checkableInLegend) {
					SendNotification(new ElementWillChangeNotification(this));
					checkableInLegend = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("SeriesBaseLegendTextPattern"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.SeriesBase.LegendTextPattern"),
		Category(Categories.Behavior),
		Editor("DevExpress.XtraCharts.Design.PatternEditor," + AssemblyInfo.SRAssemblyChartsWizard, typeof(UITypeEditor)),
		XtraSerializableProperty]
		public string LegendTextPattern {
			get { return legendTextPattern; }
			set {
				if (value != legendTextPattern) {
					SendNotification(new ElementWillChangeNotification(this));
					legendTextPattern = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("SeriesBaseColorizer"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.SeriesBase.Colorizer"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		Editor("DevExpress.XtraCharts.Design.ColorizerPickerEditor," + AssemblyInfo.SRAssemblyChartsWizard, typeof(UITypeEditor)),
		Category(Categories.Appearance),
		TypeConverter(typeof(ExpandableObjectConverter)),
		RefreshProperties(RefreshProperties.All),
		NestedTagProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Content, true)]
		public IColorizer Colorizer {
			get { return colorizer; }
			set {
				if (colorizer != value) {
					if (colorizer != null) {
						ChartColorizerBase colorizerBase = colorizer as ChartColorizerBase;
						if (colorizerBase != null)
							colorizerBase.PropertyChanging -= ColorizerPropertyChanging;
						colorizer.PropertyChanged -= ColorizerPropertyChanged;
					}
					if (value != null) {
						colorizer = value;
						ChartColorizerBase colorizerBase = colorizer as ChartColorizerBase;
						if (colorizerBase != null)
							colorizerBase.PropertyChanging += ColorizerPropertyChanging;
						colorizer.PropertyChanged += ColorizerPropertyChanged;
						if (colorizer is ChartColorizerBase)
							BindColorizerToChartPalettesHelper.BindColorizerToChartPalettes(colorizer as ChartColorizerBase, this as IOwnedElement);
						colorProvider = new ColorProvider(colorizer);
						BindingChanged();
					}
					else {
						colorProvider = new ColorProvider(defaultColorizer);
						colorizer = null;
						BindingChanged();
					}
				}
			}
		}
		#region Obsolete Properties
		[Obsolete("This property is obsolete now. Use the ArgumentScaleType property instead."),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)]
		public ScaleType ScaleType {
			get { return ArgumentScaleType; }
			set { ArgumentScaleType = value; }
		}
		[Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DefaultValue(""),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		Obsolete("This property is obsolete now.")]
		public string PointOptionsTypeName {
			get { return String.Empty; }
			set { }
		}
		[Obsolete("This property is now obsolete. Use the SeriesPointsSorting property instead."),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)]
		public SortingMode PointsSorting {
			get { return SeriesPointsSorting; }
			set { SeriesPointsSorting = value; }
		}
		[Obsolete("This property is now obsolete. Use the SeriesPointsSortingKey property instead."),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)]
		public SortingKey PointsSortingKey {
			get { return (SortingKey)SeriesPointsSortingKey; }
			set { SeriesPointsSortingKey = (SeriesPointKey)value; }
		}
		[Browsable(false),
EditorBrowsable(EditorBrowsableState.Never),
DefaultValue(""),
XtraSerializableProperty(XtraSerializationVisibility.Hidden),
Obsolete("This property is obsolete now.")]
		public string LabelTypeName {
			get { return String.Empty; }
			set { }
		}
		[Obsolete("This property is obsolete now. Use the SeriesLabelBase.TextPattern property instead."),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public PointOptions PointOptions {
			get { return Label != null ? Label.PointOptionsInternal : null; }
			set {
				if (Label != null)
					Label.PointOptionsInternal = value;
			}
		}
		[Obsolete("This property is obsolete now. Use the SeriesLabelBase.TextPattern property instead."),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		NonTestableProperty,
		NestedTagProperty]
		public IList PointOptionsSerializable {
			get { return pointOptionsSerializerWrapper; }
		}
		[Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DefaultValue(""),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		Obsolete("This property is obsolete now.")]
		public string SeriesViewTypeName {
			get { return String.Empty; }
			set { }
		}
		[Obsolete("This property is obsolete now. Use the LegendTextPattern property instead."),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)]
		public PointOptions LegendPointOptions {
			get { return legendPointOptions; }
			set {
				if (!Loading)
					throw new InvalidOperationException(ChartLocalizer.GetString(ChartStringId.MsgLegendPointOptionsSettingRuntimeError));
				if (!Object.ReferenceEquals(legendPointOptions, value)) {
					SendNotification(new ElementWillChangeNotification(this));
					SetLegendPointOptions(value);
					RaiseControlChanged();
				}
			}
		}
		[Obsolete("This property is obsolete now."),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		NonTestableProperty,
		NestedTagProperty]
		public IList LegendPointOptionsSerializable { get { return legendPointOptionsSerializerWrapper; } }
		[Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		NonTestableProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)]
		public bool SynchronizePointOptions { get; set; }
		#endregion
		public SeriesBase()
			: this(SeriesViewFactory.DefaultViewType) {
		}
		public SeriesBase(ViewType viewType)
			: base() {
			viewSerializerWrapper = new AspxSerializerWrapper<SeriesViewBase>(delegate() { return View; },
				delegate(SeriesViewBase value) { View = value; });
			labelSerializerWrapper = new AspxSerializerWrapper<SeriesLabelBase>(delegate() { return Label; },
				delegate(SeriesLabelBase value) { Label = value; });
			legendPointOptionsSerializerWrapper = new AspxSerializerWrapper<PointOptions>(delegate() { return legendPointOptions; },
				delegate(PointOptions value) { SetLegendPointOptions(value); });
			pointOptionsSerializerWrapper = new AspxSerializerWrapper<PointOptions>(delegate() { return Label != null ? Label.PointOptionsInternal : null; },
				delegate(PointOptions value) { if (Label != null) Label.PointOptionsInternal = value; }, false);
			valueDataMembers = new ValueDataMemberCollection(this);
			dataFilters = new DataFilterCollection(this);
			topNOptions = new TopNOptions(this);
			pointOptionsSynchronizer = new ChartElementSynchronizer();
			SetView(SeriesViewFactory.CreateInstance(viewType), true);
			colorProvider = new ColorProvider(defaultColorizer);
		}
		#region ISeriesBase implementation
		Scale ISeriesBase.ArgumentScaleType { get { return (Scale)ActualArgumentScaleType; } }
		Scale ISeriesBase.UserArgumentScaleType {
			get {
				if (ArgumentScaleType == ScaleType.Auto)
					return (Scale)argumentScaleByDataSource;
				return (Scale)ArgumentScaleType;
			}
		}
		Scale ISeriesBase.ValueScaleType { get { return (Scale)ValueScaleType; } }
		ISeriesView ISeriesBase.SeriesView { get { return View; } }
		#endregion
		#region ISeriesTemplate implementation
		ISeries ISeriesTemplate.CreateSeriesForBinding(string seriesName, object seriesValue) {
			Series newSeries = CreateSeries(seriesName);
			newSeries.Autocreated = true;
			newSeries.Tag = seriesValue;
			return newSeries;
		}
		String ISeriesTemplate.ArgumentDataMember { get { return ArgumentDataMember; } }
		String ISeriesTemplate.ToolTipHintDataMember { get { return ToolTipHintDataMember; } }
		IList<string> ISeriesTemplate.ValueDataMembers { get { return ActualValueDataMembers; } }
		#endregion
		#region IHitTest implementation
		SeriesHitTestState hitTestState;
		object IHitTest.Object { get { return HitObject; } }
		HitTestState IHitTest.State { get { return HitState; } }
		protected internal virtual object HitObject { get { return this; } }
		protected internal virtual SeriesHitTestState HitState { get { return hitTestState; } }
		#endregion
		#region ITopNOptions implementation
		bool ITopNOptions.ShouldApplyTopNOptions { get { return ShouldApplyTopNOptions; } }
		bool ITopNOptions.ShouldUseTopNOthers { get { return ShouldUseTopNOthers; } }
		#endregion
		#region IPointsFilterOptions implementation
		bool IPointsFilterOptions.Enable {
			get { return ShouldApplyTopNOptions && TopNOptions.Enabled; }
		}
		PointsFilterType IPointsFilterOptions.FilterType {
			get {
				switch (TopNOptions.Mode) {
					case TopNMode.Count:
						return PointsFilterType.TopN;
					case TopNMode.ThresholdValue:
						return PointsFilterType.MoreOrEqualValue;
					case TopNMode.ThresholdPercent:
						return PointsFilterType.MoreOrEqualPercentValue;
					default:
						return PointsFilterType.TopN;
				}
			}
		}
		string IPointsFilterOptions.OthersArgument {
			get {
				string argument = String.IsNullOrEmpty(TopNOptions.OthersArgument) ?
					ChartLocalizer.GetString(ChartStringId.OthersArgument) : TopNOptions.OthersArgument;
				return argument;
			}
		}
		bool IPointsFilterOptions.ShowOthers {
			get { return ShouldUseTopNOthers && TopNOptions.ShowOthers; }
		}
		double IPointsFilterOptions.ThresholdValue {
			get {
				switch (TopNOptions.Mode) {
					case TopNMode.Count:
						return TopNOptions.Count;
					case TopNMode.ThresholdValue:
						return TopNOptions.ThresholdValue;
					case TopNMode.ThresholdPercent:
						return TopNOptions.ThresholdPercent;
					default:
						return TopNOptions.Count;
				}
			}
		}
		#endregion
		#region ISeriesPointFactory implementation
		ISeriesPoint ISeriesPointFactory.CreateSeriesPoint(object argument) {
			return new SeriesPoint(argument, 0.0);
		}
		ISeriesPoint ISeriesPointFactory.CreateSeriesPoint(ISeries owner, object argument, double internalArgument, object[] values, double[] internalValues, object tag) {
			return new SeriesPoint(argument, internalArgument, values, internalValues, tag);
		}
		ISeriesPoint ISeriesPointFactory.CreateSeriesPoint(ISeries owner, object argument, double internalArgument, object[] values, double[] internalValues, object tag, object toolTipHint) {
			return new SeriesPoint(argument, internalArgument, values, internalValues, tag, toolTipHint != null ? toolTipHint.ToString() : string.Empty);
		}
		ISeriesPoint ISeriesPointFactory.CreateSeriesPoint(ISeries owner, object argument, double internalArgument, object[] values, double[] internalValues, object tag, object toolTipHint, object color) {
			SeriesPoint seriesPoint = new SeriesPoint(argument, internalArgument, values, internalValues, tag, toolTipHint != null ? toolTipHint.ToString() : string.Empty);
			if (color != null)
				seriesPoint.Color = colorProvider.GetColor(argument, values, color, this);
			return seriesPoint;
		}
		ISeriesPoint ISeriesPointFactory.CreateSeriesPoint(ISeries owner, object argument, object[] values, object tag, object[] colors) {
			SeriesPoint seriesPoint = new SeriesPoint(argument, 0.0, values, null, tag, string.Empty);
			if (colors != null && colors.Length > 0)
				seriesPoint.Color = colorProvider.GetColor(argument, values, colors, this);
			return seriesPoint;
		}
		#endregion
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			switch (propertyName) {
				case "ArgumentDataMember":
					return ShouldSerializeArgumentDataMember();
				case "ValueDataMembersSerializable":
					return ShouldSerializeValueDataMembersSerializable();
				case "ToolTipHintDataMember":
					return ShouldSerializeToolTipHintDataMember();
				case "SummaryFunction":
					return ShouldSerializeSummaryFunction();
				case "View":
					return ShouldSerializeViewCore();
				case "DataFiltersConjunctionMode":
					return ShouldSerializeDataFiltersConjunctionMode();
				case "ArgumentScaleType":
					return ShouldSerializeArgumentScaleType();
				case "ValueScaleType":
					return ShouldSerializeValueScaleType();
				case "Visible":
					return ShouldSerializeVisible();
				case "ShowInLegend":
					return ShouldSerializeShowInLegend();
				case "LegendText":
					return ShouldSerializeLegendText();
				case "Label":
					return ShouldSerializeLabelCore();
				case "PointOptions":
					return ShouldSerializePointOptions();
				case "SeriesPointsSorting":
					return ShouldSerializeSeriesPointsSorting();
				case "SeriesPointsSortingKey":
					return ShouldSerializeSeriesPointsSortingKey();
				case "ToolTipSeriesPattern":
					return ShouldSerializeToolTipSeriesPattern();
				case "ToolTipPointPattern":
					return ShouldSerializeToolTipPointPattern();
				case "CrosshairEnabled":
					return ShouldSerializeCrosshairEnabled();
				case "CrosshairLabelVisibility":
					return ShouldSerializeCrosshairLabelVisibility();
				case "CrosshairLabelPattern":
					return ShouldSerializeCrosshairLabelPattern();
				case "CrosshairHighlightPoints":
					return ShouldSerializeCrosshairHighlightPoints();
				case "ToolTipEnabled":
					return ShouldSerializeToolTipEnabled();
				case "LabelsVisibility":
					return ShouldSerializeLabelsVisibility();
				case "Colorizer":
					return ShouldSerializeColorizer();
				case "CheckedInLegend":
					return ShouldSerializeCheckedInLegend();
				case "CheckableInLegend":
					return ShouldSerializeCheckableInLegend();
				case "LegendTextPattern":
					return ShouldSerializeLegendTextPattern();
				default:
					return base.XtraShouldSerialize(propertyName);
			}
		}
		object IXtraSupportCreateContentPropertyValue.Create(XtraItemEventArgs e) {
			return (e.Item.Name == "View" || e.Item.Name == "Label") ?
				XtraSerializingUtils.GetContentPropertyInstance(e, SerializationUtils.ExecutingAssembly, SerializationUtils.PublicNamespace) : null;
		}
		void IXtraSupportAfterDeserialize.AfterDeserialize(XtraItemEventArgs e) {
			if (e.Item.Name == "View")
				View = (SeriesViewBase)e.Item.Value;
			else if (e.Item.Name == "Label")
				Label = (SeriesLabelBase)e.Item.Value;
			else if (e.Item.Name == "LegendPointOptions")
				SetLegendPointOptions((PointOptions)e.Item.Value);
		}
		void IXtraSupportDeserializeCollectionItem.SetIndexCollectionItem(string propertyName, XtraSetItemIndexEventArgs e) {
			XtraSetIndexCollectionItem(propertyName, e);
		}
		object IXtraSupportDeserializeCollectionItem.CreateCollectionItem(string propertyName, XtraItemEventArgs e) {
			return XtraCreateCollectionItem(propertyName, e);
		}
		protected virtual void XtraSetIndexCollectionItem(string propertyName, XtraSetItemIndexEventArgs e) {
			if (propertyName == "DataFilters")
				DataFilters.Add((DataFilter)e.Item.Value);
		}
		protected virtual object XtraCreateCollectionItem(string propertyName, XtraItemEventArgs e) {
			if (propertyName == "DataFilters")
				return new DataFilter();
			return null;
		}
		#endregion
		#region ShouldSerialize & Reset
		bool ShouldSerializeArgumentDataMember() {
			return !String.IsNullOrEmpty(argumentDataMember);
		}
		void ResetArgumentDataMember() {
			ArgumentDataMember = string.Empty;
		}
		bool ShouldSerializeColorDataMember() {
			return !String.IsNullOrEmpty(colorDataMember);
		}
		void ResetColorDataMember() {
			ColorDataMember = string.Empty;
		}
		bool ShouldSerializeSummaryFunction() {
			return !String.IsNullOrEmpty(summaryFunction);
		}
		void ResetSummaryFunction() {
			SummaryFunction = string.Empty;
		}
		bool ShouldSerializeToolTipHintDataMember() {
			return !String.IsNullOrEmpty(toolTipHintDataMember);
		}
		void ResetToolTipHintDataMember() {
			ToolTipHintDataMember = string.Empty;
		}
		bool ShouldSerializeValueDataMembersSerializable() {
			return !ShouldSerializeSummaryFunction() && !valueDataMembers.IsEmpty();
		}
		bool ShouldSerializeDataFiltersConjunctionMode() {
			return DataFiltersConjunctionMode != DataFilterCollection.DefaultConjunctionMode;
		}
		bool ShouldSerializeArgumentScaleType() {
			return ArgumentScaleType != DefaultArgumentScaleType;
		}
		void ResetArgumentScaleType() {
			ArgumentScaleType = DefaultArgumentScaleType;
		}
		bool ShouldSerializeScaleType() {
			return false;
		}
		bool ShouldSerializeValueScaleType() {
			return ValueScaleType != DefaultValueScaleType;
		}
		void ResetValueScaleType() {
			ValueScaleType = DefaultValueScaleType;
		}
		bool ShouldSerializeVisible() {
			return this.visible != DefaultVisible;
		}
		void ResetVisible() {
			Visible = DefaultVisible;
		}
		bool ShouldSerializeShowInLegend() {
			return this.showInLegend != DefaultShowInLegend;
		}
		void ResetShowInLegend() {
			ShowInLegend = DefaultShowInLegend;
		}
		bool ShouldSerializeLegendText() {
			return !String.IsNullOrEmpty(legendText) && legendText != PatternUtils.SeriesNamePattern && legendText != PatternUtils.SeriesNamePatternLowercase;
		}
		void ResetLegendText() {
			LegendText = string.Empty;
		}
		bool ShouldSerializeViewTypeName() {
			return false;
		}
		bool ShouldSerializeViewCore() {
			return view != null && view.ShouldSerialize();
		}
		bool ShouldSerializeView() {
			return ShouldSerializeViewCore() && ChartContainer != null && ChartContainer.ControlType != ChartContainerType.WebControl;
		}
		bool ShouldSerializeViewSerializable() {
			return ShouldSerializeViewCore() && ChartContainer != null && ChartContainer.ControlType == ChartContainerType.WebControl;
		}
		bool ShouldSerializeLabelTypeName() {
			return false;
		}
		bool ShouldSerializeLabelCore() {
			return label != null && label.ShouldSerialize();
		}
		bool ShouldSerializeLabel() {
			return ShouldSerializeLabelCore() && ChartContainer != null && ChartContainer.ControlType != ChartContainerType.WebControl;
		}
		bool ShouldSerializeLabelSerializable() {
			return ShouldSerializeLabelCore() && ChartContainer != null && ChartContainer.ControlType == ChartContainerType.WebControl;
		}
		bool ShouldSerializePointOptionsTypeName() {
			return false;
		}
		bool ShouldSerializePointOptions() {
			return false;
		}
		bool ShouldSerializePointOptionsSerializable() {
			return false;
		}
		bool ShouldSerializeLegendPointOptions() {
			return false;
		}
		bool ShouldSerializeLegendPointOptionsSerializable() {
			return false;
		}
		bool ShouldSerializeSynchronizePointOptions() {
			return false;
		}
		bool ShouldSerializeSeriesViewTypeName() {
			return false;
		}
		bool ShouldSerializePointsSorting() {
			return false;
		}
		bool ShouldSerializePointsSortingKey() {
			return false;
		}
		bool ShouldSerializeSeriesPointsSorting() {
			return SeriesPointsSorting != DefaultSeriesPointsSorting;
		}
		void ResetSeriesPointsSorting() {
			SeriesPointsSorting = DefaultSeriesPointsSorting;
		}
		bool ShouldSerializeSeriesPointsSortingKey() {
			return SeriesPointsSortingKey != DefaultSeriesPointsSortingKey;
		}
		void ResetSeriesPointsSortingKey() {
			SeriesPointsSortingKey = DefaultSeriesPointsSortingKey;
		}
		bool ShouldSerializeTopNOptions() {
			return topNOptions.ShouldSerialize();
		}
		bool ShouldSerializeToolTipSeriesPattern() {
			return toolTipSeriesPattern != DefaultToolTipSeriesPattern;
		}
		void ResetToolTipSeriesPattern() {
			ToolTipSeriesPattern = DefaultToolTipSeriesPattern;
		}
		bool ShouldSerializeToolTipPointPattern() {
			return !String.IsNullOrEmpty(toolTipPointPattern);
		}
		void ResetToolTipPointPattern() {
			ToolTipPointPattern = string.Empty;
		}
		bool ShouldSerializeCrosshairLabelPattern() {
			return !String.IsNullOrEmpty(crosshairLabelPattern);
		}
		void ResetCrosshairLabelPattern() {
			CrosshairLabelPattern = string.Empty;
		}
		bool ShouldSerializeCrosshairHighlightPoints() {
			return crosshairHighlightPoints != DefaultCrosshairHighlightPoints;
		}
		void ResetCrosshairHighlightPoints() {
			CrosshairHighlightPoints = DefaultCrosshairHighlightPoints;
		}
		bool ShouldSerializeCrosshairEnabled() {
			return crosshairEnabled != DefaultCrosshairEnabled;
		}
		void ResetCrosshairEnabled() {
			CrosshairEnabled = DefaultCrosshairEnabled;
		}
		bool ShouldSerializeCrosshairLabelVisibility() {
			return crosshairLabelVisibility != DefaultCrosshairLabelVisibility;
		}
		void ResetCrosshairLabelVisibility() {
			CrosshairLabelVisibility = DefaultCrosshairLabelVisibility;
		}
		bool ShouldSerializeColorizer() {
			return colorizer != null;
		}
		void ResetColorizer() {
			Colorizer = null;
		}
		bool ShouldSerializeToolTipEnabled() {
			return toolTipEnabled != DefaultToolTipEnabled;
		}
		void ResetToolTipEnabled() {
			ToolTipEnabled = DefaultToolTipEnabled;
		}
		bool ShouldSerializeLabelsVisibility() {
			return this.labelsVisibility != DefaultLabelsVisibility;
		}
		void ResetLabelsVisibility() {
			LabelsVisibility = DefaultLabelsVisibility;
		}
		bool ShouldSerializeCheckedInLegend() {
			return checkedInLegend != DefaultCheckedInLegend;
		}
		void ResetCheckedInLegend() {
			CheckedInLegend = DefaultCheckedInLegend;
		}
		bool ShouldSerializeCheckableInLegend() {
			return checkableInLegend != DefaultCheckableInLegend;
		}
		void ResetCheckableInLegend() {
			CheckableInLegend = DefaultCheckableInLegend;
		}
		bool ShouldSerializeLegendTextPattern() {
			return !String.IsNullOrEmpty(legendTextPattern);
		}
		void ResetLegendTextPattern() {
			LegendTextPattern = string.Empty;
		}
		protected internal override bool ShouldSerialize() {
			return base.ShouldSerialize() ||
						ShouldSerializeColorizer() ||
						ShouldSerializeArgumentDataMember() ||
						ShouldSerializeSummaryFunction() ||
						ShouldSerializeValueDataMembersSerializable() ||
						ShouldSerializeView() ||
						ShouldSerializeDataFiltersConjunctionMode() ||
						DataFilters.Count > 0 ||
						ShouldSerializeArgumentScaleType() ||
						ShouldSerializeValueScaleType() ||
						ShouldSerializeVisible() ||
						ShouldSerializeShowInLegend() ||
						ShouldSerializeLabelCore() ||
						ShouldSerializeSeriesViewTypeName() ||
						ShouldSerializeSeriesPointsSorting() ||
						ShouldSerializeSeriesPointsSortingKey() ||
						ShouldSerializeTopNOptions() ||
						ShouldSerializeToolTipHintDataMember() ||
						ShouldSerializeToolTipSeriesPattern() ||
						ShouldSerializeToolTipPointPattern() ||
						ShouldSerializeCrosshairEnabled() ||
						ShouldSerializeCrosshairLabelVisibility() ||
						ShouldSerializeCrosshairLabelPattern() ||
						ShouldSerializeCrosshairHighlightPoints() ||
						ShouldSerializeToolTipEnabled() ||
						ShouldSerializeLabelsVisibility() ||
						ShouldSerializeCheckedInLegend() ||
						ShouldSerializeLegendTextPattern();
		}
		#endregion
		void RaiseSortedPropertyChanged(PropertyUpdateInfo updateInfo) {
			RaiseControlChanged(updateInfo);
		}
		bool CheckSortingKey(SeriesPointKey key) {
			return Loading || CommonUtils.CheckSortingKey(key, view.PointDimension);
		}
		bool ShouldUpdateSizeMargins(SeriesViewBase oldView, SeriesViewBase newView) {
			if (oldView == null || newView == null)
				return true;
			if (!newView.DiagramType.Equals(oldView.DiagramType))
				return true;
			return oldView.SideMarginsEnabled == oldView.ActualSideMarginsEnabled;
		}
		void DisposeLabel() {
			if (label != null) {
				label.Dispose();
				label.PointOptionsInternal.Synchronizer = null;
				pointOptionsSynchronizer.Source = null;
				label = null;
			}
		}
		void DisposeLegendPointOptions() {
			if (legendPointOptions != null) {
				legendPointOptions.Synchronizer = null;
				pointOptionsSynchronizer.Destination = null;
				legendPointOptions.Dispose();
				legendPointOptions = null;
			}
		}
		void CheckSummaryFunction(string functionString) {
			object dataSource = GetDataSource();
			CheckSummaryFunction(dataSource, functionString);
		}
		void CheckToolTipHintDataMember(object dataSource, string toolTipHintDataMember) {
			if (!Loading && dataSource != null && !String.IsNullOrEmpty(toolTipHintDataMember)) {
				string actualDataMember = BindingProcedure.ConvertToActualDataMember(GetChartDataMember(), toolTipHintDataMember);
				if (!BindingHelper.CheckDataMember(DataContext, dataSource, actualDataMember))
					throw new ArgumentException(String.Format(ChartLocalizer.GetString(ChartStringId.MsgIncorrectDataMember), toolTipHintDataMember));
			}
		}
		void CreateNewSeriesLabel(SeriesViewBase view) {
			SeriesLabelBase newLabel = view.CreateSeriesLabel();
			if (newLabel != null) {
				newLabel.SetNewPointOptions(view.CreatePointOptions());
				if (label != null)
					newLabel.Assign(label);
				newLabel.EnsureResolveOverlappingMode();
			}
			SetSeriesLabel(newLabel);
		}
		void SetSeriesLabel(SeriesLabelBase newLabel) {
			PointOptions pointOptions = null;
			if (newLabel != null && newLabel.PointOptionsInternal == null)
				pointOptions = (PointOptions)label.PointOptionsInternal.Clone();
			DisposeLabel();
			label = newLabel;
			if (label != null) {
				label.SeriesBase = this;
				if (label.PointOptionsInternal != null) {
					if (pointOptions != null)
						label.PointOptionsInternal.Assign(pointOptions);
					pointOptionsSynchronizer.Source = label.PointOptionsInternal;
					label.PointOptionsInternal.Synchronizer = pointOptionsSynchronizer;
				}
				if (label.InternalVisibility != DefaultBoolean.Default)
					LabelsVisibility = label.InternalVisibility;
			}
		}
		void CreateNewLegendPointOptions(SeriesViewBase view) {
			PointOptions newPointOptions = view.CreatePointOptions();
			if (legendPointOptions != null)
				newPointOptions.Assign(legendPointOptions);
			SetLegendPointOptions(newPointOptions, false);
		}
		void SetLegendPointOptions(PointOptions pointOptions) {
			SetLegendPointOptions(pointOptions, true);
		}
		void SetLegendPointOptions(PointOptions pointOptions, bool updateLegendTextPattern) {
			DisposeLegendPointOptions();
			legendPointOptions = pointOptions;
			if (legendPointOptions != null) {
				legendPointOptions.SeriesBase = this;
				legendPointOptions.Synchronizer = pointOptionsSynchronizer;
				pointOptionsSynchronizer.Destination = legendPointOptions;
				if (updateLegendTextPattern)
					UpdateLegendTextPattern(legendPointOptions);
			}
		}
		void DisposeView() {
			if (view != null) {
				view.Dispose();
				view = null;
			}
		}
		void UpdateView(SeriesViewBase view, bool assignParams) {
			if (!Object.ReferenceEquals(this.view, view)) {
				SendNotification(new ElementWillChangeNotification(this));
				bool shouldUpdateSideMargins = ShouldUpdateSizeMargins(this.view, view);
				SetView(view, assignParams);
				RaiseControlChanged(new ChartElementUpdateInfo(this, ChartElementChange.NonSpecific));
			}
		}
		void AssignView(SeriesViewBase templateView) {
			if (templateView != null) {
				if (view == null || !view.GetType().Equals(templateView.GetType()))
					SetView(SeriesViewFactory.CreateInstance(templateView), true);
				view.Assign(templateView);
			}
			else
				SetView(null, false);
		}
		ScaleType CalculateActualArgumentScaleType() {
			object dataSource = GetDataSource();
			if (dataSource != null && !string.IsNullOrEmpty(ArgumentDataMember))
				return (ScaleType)BindingHelperCore.GetScaleType(dataSource, ArgumentDataMember);
			return ScaleType.Auto;
		}
		void ColorizerPropertyChanged(object sender, PropertyChangedEventArgs e) {
			if (e.PropertyName == "ShowInLegend" || e.PropertyName == "LegendItemPattern")
				RaiseControlChanged();
			else
				BindingChanged();
		}
		void ColorizerPropertyChanging(object sender, EventArgs e) {
			SendNotification(new ElementWillChangeNotification(this));
		}
		protected internal void ResetDataSourceArgumentScaleType() {
			argumentScaleByDataSource = ScaleType.Auto;
		}
		protected ScaleType GetScaleFromDataSource() {
			if (argumentScaleByDataSource == ScaleType.Auto)
				UpdateScaleFromDataSource();
			return argumentScaleByDataSource;
		}
		protected void UpdateScaleFromDataSource() {
			object dataSource = GetDataSource();
			string member = BindingProcedure.ConvertToActualDataMember(GetChartDataMember(), ArgumentDataMember);
			if (dataSource != null && !string.IsNullOrEmpty(member))
				argumentScaleByDataSource = (ScaleType)BindingHelperCore.GetScaleType(dataSource, member);
		}
		protected void SetValueScaleTypeField(ScaleType valueScaleType) {
			this.valueScaleType = valueScaleType;
		}
		protected void UpdateLegendTextPattern(ChartElement sender) {
			if (legendPointOptions != null && sender != null &&
				(sender == legendPointOptions || sender.Owner == legendPointOptions))
				legendTextPattern = PointOptions.ConstructPatternFromPointOptions(legendPointOptions, ActualArgumentScaleType, ValueScaleType);
		}
		protected override void Dispose(bool disposing) {
			if (disposing) {
				Owner = null;
				DisposeLabel();
				DisposeLegendPointOptions();
				DisposeView();
				if (dataFilters != null) {
					dataFilters.Dispose();
					dataFilters = null;
				}
			}
			base.Dispose(disposing);
		}
		protected override ChartElement CreateObjectForClone() {
			return new SeriesBase();
		}
		protected virtual ScaleType GetActualArgumentScaleType() {
			GetScaleFromDataSource();
			if (argumentScaleByDataSource == ScaleType.Auto)
				return ScaleType.Numerical;
			return argumentScaleByDataSource;
		}
		protected virtual void SetValueScaleType(ScaleType valueScaleType) {
			SetValueScaleTypeField(valueScaleType);
			if (Owner != null)
				Owner.DisposeTempAutocreatedSeries();
		}
		protected virtual void SetShowInLegend(bool showInLegend) {
			this.showInLegend = showInLegend;
		}
		protected virtual void SetPointsSorting(SortingMode pointsSorting) {
			this.pointsSorting = pointsSorting;
		}
		protected virtual void SetSortingKey(SeriesPointKey pointsSortingKey) {
			SendNotification(new ElementWillChangeNotification(this));
			SeriesPointKey oldPointsSortingKey = this.pointsSortingKey;
			this.pointsSortingKey = pointsSortingKey;
			RaiseSortedPropertyChanged(new PropertyUpdateInfo<SeriesPointKeyNative>(this, "SeriesPointKey", (SeriesPointKeyNative)oldPointsSortingKey, (SeriesPointKeyNative)pointsSortingKey));
		}
		protected virtual void ProcessSetView() {
		}
		internal object GetDataSource() {
			return SeriesDataBindingUtils.GetDataSource(this);
		}
		internal string GetChartDataMember() {
			return SeriesDataBindingUtils.GetDataMember(this);
		}
		internal void SetView(SeriesViewBase view, bool assignParams) {
			if (view == null) {
				DisposeView();
				return;
			}
			hitTestState = view.CreateHitTestState();
			hitTestState.Series = this;
			if (!Loading)
				CheckScaleTypes(view);
			UpdateScaleTypes(view);
			view.Owner = this;
			if (assignParams && this.view != null) {
				view.Assign(this.view);
				if (!Loading)
					view.OnEndLoading();
			}
			DisposeView();
			SeriesViewBase oldView = this.view;
			this.view = view;
			ProcessSetView();
			ValueDataMembers.SetDimension(view.PointDimension);
			if (!CheckSortingKey(pointsSortingKey))
				pointsSortingKey = SeriesPointKey.Argument;
			if (!Loading)
				try {
					CheckSummaryFunction(summaryFunction);
				}
				catch {
					summaryFunction = String.Empty;
				}
			RaisePropertyChanged<SeriesViewBase>("View", oldView, this.view);
			CreateNewSeriesLabel(view);
			CreateNewLegendPointOptions(view);
		}
		internal bool Contains(object obj) {
			return obj == this || obj == label || (view != null && view.Contains(obj));
		}
		internal void SetArgumentDataMember(string value) {
			argumentDataMember = value;
		}
		internal Series CreateSeries(string name) {
			Series series = new Series(name, SeriesViewFactory.GetViewType(this.view));
			series.Assign(this);
			return series;
		}
		internal void CheckDataMember(string dataMember, ScaleType? scaleType) {
			CheckDataMember(GetDataSource(), dataMember, scaleType);
		}
		internal void CheckDataMember(object dataSource, string dataMember, ScaleType? scaleType) {
			string actualDataMember = BindingProcedure.ConvertToActualDataMember(GetChartDataMember(), dataMember);
			if (!BindingHelper.CheckDataMember(DataContext, dataSource, actualDataMember))
				throw new ArgumentException(String.Format(ChartLocalizer.GetString(ChartStringId.MsgIncorrectDataMember), dataMember));
			if (scaleType != null && !BindingHelper.CheckDataMember(DataContext, dataSource, actualDataMember, scaleType.Value)) {
				string message = String.Format(ChartLocalizer.GetString(ChartStringId.MsgIncompatibleValueDataMember),
					dataMember, Series.GetScaleTypeName(scaleType.Value));
				throw new ArgumentException(message);
			}
		}
		internal void CheckSummaryFunction(object dataSource, string functionString) {
			SummaryFunctionParser parser = new SummaryFunctionParser(functionString);
			if (String.IsNullOrEmpty(parser.FunctionName))
				return;
			SummaryFunctionDescription description = (Chart == null) ? null : Chart.SummaryFunctions[parser.FunctionName];
			if (description != null) {
				if (description.ResultScaleType != null && description.ResultScaleType.Value != valueScaleType)
					ThrowIncompatibleSummaryFunctionException(ChartStringId.MsgIncompatibleSummaryFunction, description, valueScaleType);
				if (description.ResultDimension != view.PointDimension) {
					string str = String.Format(ChartLocalizer.GetString(ChartStringId.MsgIncompatibleSummaryFunctionDimension),
						parser.FunctionName, view.StringId, description.ResultDimension, view.PointDimension);
					throw new ArgumentException(str);
				}
				if (description.ArgumentDescriptions.Length != parser.Arguments.Length) {
					string message = String.Format(ChartLocalizer.GetString(ChartStringId.MsgIncorrectSummaryFunctionParametersCount),
						description.DisplayName, description.ArgumentDescriptions.Length, parser.Arguments.Length);
					throw new ArgumentException(message);
				}
			}
			for (int i = 0; i < parser.Arguments.Length; i++) {
				ScaleType? scaleType = null;
				if (description != null) {
					scaleType = description.ArgumentDescriptions[i].ScaleType;
					if (scaleType == null && description.IsStandard)
						scaleType = valueScaleType;
				}
				CheckDataMember(dataSource, parser.Arguments[i], scaleType);
			}
		}
		internal void CheckBinding(object dataSource) {
			string chartDataMember = GetChartDataMember();
			if (!BindingHelper.CheckDataMember(DataContext, dataSource, BindingProcedure.ConvertToActualDataMember(chartDataMember, argumentDataMember), ArgumentScaleType))
				argumentDataMember = null;
			for (int i = 0; i < ValueDataMembers.Count; i++)
				if (!BindingHelper.CheckDataMember(DataContext, dataSource, BindingProcedure.ConvertToActualDataMember(chartDataMember, ValueDataMembers[i]), valueScaleType))
					valueDataMembers[i] = String.Empty;
			if (IsSummaryBinding) {
				try {
					CheckSummaryFunction(dataSource, summaryFunction);
				}
				catch {
					summaryFunction = String.Empty;
				}
			}
			for (int i = 0; i < dataFilters.Count; )
				if (BindingHelper.CheckDataMember(DataContext, dataSource, BindingProcedure.ConvertToActualDataMember(chartDataMember, dataFilters[i].ActualColumnName)))
					i++;
				else
					dataFilters.RemoveAt(i);
		}
		internal void BindingChanged() {
			BindingChanged(new ChartElementUpdateInfo(this, ChartElementChange.NonSpecific));
		}
		internal void BreakPointOptionsRelations() {
			pointOptionsSynchronizer.IsSynchronized = false;
		}
		internal void CheckArgumentDataMember(object dataSource, string dataMember, ScaleType argumentScaleType) {
			if (!Loading && dataSource != null && !String.IsNullOrEmpty(dataMember)) {
				string actualDataMember = BindingProcedure.ConvertToActualDataMember(GetChartDataMember(), dataMember);
				if (!BindingHelper.CheckDataMember(DataContext, dataSource, actualDataMember))
					throw new ArgumentException(String.Format(ChartLocalizer.GetString(ChartStringId.MsgIncorrectDataMember), dataMember));
				if (!BindingHelper.CheckDataMember(DataContext, dataSource, actualDataMember, argumentScaleType))
					throw new ArgumentException(String.Format(ChartLocalizer.GetString(ChartStringId.MsgIncompatibleArgumentDataMember),
						dataMember, Series.GetScaleTypeName(argumentScaleType)));
			}
		}
		internal void CheckColorDataMember(object dataSource, string colorMember) {
			if (!Loading && dataSource != null && !String.IsNullOrEmpty(colorMember)) {
				string actualColorMember = BindingProcedure.ConvertToActualDataMember(GetChartDataMember(), colorMember);
				if (!BindingHelper.CheckDataMember(DataContext, dataSource, actualColorMember))
					throw new ArgumentException(String.Format("", colorMember));
			}
		}
		internal void ValidateIndicators(IRefinedSeries refinedSeries) {
			XYDiagram2DSeriesViewBase xyView = view as XYDiagram2DSeriesViewBase;
			if (xyView != null)
				foreach (Indicator indicator in xyView.Indicators)
					indicator.Validate(xyView, refinedSeries);
		}
		internal void CheckBindingRuntime(object dataSource) {
			if ((Chart == null) || !(Equals(Chart.DataContainer.SeriesTemplate) && (dataSource is IChartDataSource) && Chart.DataContainer.PivotGridDataSourceOptions.AutoBindingSettingsEnabled))
				CheckArgumentDataMember(dataSource, ArgumentDataMember, ArgumentScaleType);
			foreach (string item in ValueDataMembers)
				CheckDataMember(dataSource, item, ValueScaleType);
			CheckSummaryFunction(dataSource, SummaryFunction);
			foreach (DataFilter filter in DataFilters)
				CheckDataMember(dataSource, filter.ActualColumnName, null);
		}
		internal void CheckScaleTypes(SeriesViewBase view) {
			SeriesScaleTypeUtils.CheckScaleTypes(this, view);
		}
		internal void UpdateScaleTypes(SeriesViewBase view) {
			SeriesScaleTypeUtils.UpdateScaleTypes(this, view);
		}
		protected internal virtual void SetArgumentScaleType(ScaleType argumentScaleType) {
			if (view != null)
				view.SetArgumentScaleType(argumentScaleType);
			this.argumentScaleType = argumentScaleType;
		}
		protected internal virtual void OnEndLoading(bool validatePointsId) {
			if (view != null)
				view.OnEndLoading();
			if (label != null)
				label.OnEndLoading();
			pointOptionsSynchronizer.EndLoading();
			foreach (DataFilter filter in dataFilters)
				filter.OnEndLoading();
		}
		protected internal virtual void ValidatePointsByArgumentScaleType(ScaleType argumentScaleType) {
			if ((argumentScaleType != ScaleType.Numerical) && !view.NonNumericArgumentSupported)
				ThrowIncompatibleScaleTypeException(ChartStringId.MsgIncompatibleArgumentScaleType, argumentScaleType, view);
			if (!BindingHelper.CheckDataMember(DataContext, GetDataSource(), BindingProcedure.ConvertToActualDataMember(GetChartDataMember(), argumentDataMember), argumentScaleType))
				ThrowInvalidScaleTypeException(ChartStringId.MsgIncompatibleArgumentDataMember, argumentDataMember, argumentScaleType);
			if (view != null)
				view.CheckArgumentScaleType(argumentScaleType);
		}
		protected internal virtual void ValidatePointsByValueScaleType(ScaleType valueScaleType) {
			if (valueScaleType == DevExpress.XtraCharts.ScaleType.Qualitative ||
				valueScaleType == DevExpress.XtraCharts.ScaleType.Auto ||
				(valueScaleType == DevExpress.XtraCharts.ScaleType.DateTime && view != null && !view.DateTimeValuesSupported))
				ThrowIncompatibleScaleTypeException(ChartStringId.MsgIncompatibleValueScaleType, valueScaleType, view);
			object dataSource = GetDataSource();
			string chartDataMember = GetChartDataMember();
			if (IsSummaryBinding) {
				SummaryFunctionParser parser = new SummaryFunctionParser(summaryFunction);
				SummaryFunctionDescription description = Chart.SummaryFunctions[parser.FunctionName];
				if (description != null) {
					DevExpress.XtraCharts.ScaleType? scaleType = description.ResultScaleType;
					if (scaleType == null) {
						if (description.IsStandard) {
							foreach (string argument in parser.Arguments)
								if (!BindingHelper.CheckDataMember(DataContext, dataSource, BindingProcedure.ConvertToActualDataMember(chartDataMember, argument), valueScaleType))
									ThrowInvalidScaleTypeException(ChartStringId.MsgIncompatibleValueDataMember, argument, valueScaleType);
						}
					}
					else if (scaleType != valueScaleType)
						ThrowIncompatibleSummaryFunctionException(ChartStringId.MsgIncompatibleSummaryFunction, description, valueScaleType);
				}
			}
			else
				foreach (string dataMember in valueDataMembers)
					if (!BindingHelper.CheckDataMember(DataContext, dataSource, BindingProcedure.ConvertToActualDataMember(chartDataMember, dataMember), valueScaleType))
						ThrowInvalidScaleTypeException(ChartStringId.MsgIncompatibleValueDataMember, dataMember, valueScaleType);
		}
		protected internal virtual void BindingChanged(ChartUpdateInfoBase changeInfo) {
			RaiseControlChanged(changeInfo);
		}
		protected override bool ProcessChanged(ChartElement sender, ChartUpdateInfoBase changeInfo) {
			if (Label != null)
				Label.UpdateTextPattern(sender);
			UpdateLegendTextPattern(sender);
			DataContainer dataContainer = Owner as DataContainer;
			if (dataContainer != null)
				dataContainer.UpdateBinding(true, false);
			return base.ProcessChanged(sender, changeInfo);
		}
		protected override void OnOwnerChanged(ChartElement oldOwner, ChartElement newOwner) {
			if (colorizer is ChartColorizerBase)
				BindColorizerToChartPalettesHelper.BindColorizerToChartPalettes(colorizer as ChartColorizerBase, this as IOwnedElement);
			base.OnOwnerChanged(oldOwner, newOwner);
		}
		protected internal string GetToolTipSeriesPattern() {
			return "{S}";
		}
		public void ChangeView(ViewType viewType) {
			UpdateView(SeriesViewFactory.CreateInstance(viewType), true);
		}
		[Obsolete("This method is obsolete now. Set to String.Empty LegendTextPattern property instead.")]
		public void ResetLegendPointOptions() {
			SendNotification(new ElementWillChangeNotification(this));
			pointOptionsSynchronizer.IsSynchronized = true;
			RaiseControlChanged();
		}
		public override string ToString() {
			return "(SeriesBase)";
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			SeriesBase seriesBase = obj as SeriesBase;
			if (seriesBase == null)
				return;
			AssignView(seriesBase.view);
			visible = seriesBase.visible;
			showInLegend = seriesBase.showInLegend;
			legendText = seriesBase.legendText;
			SetArgumentScaleType(seriesBase.argumentScaleType);
			valueScaleType = seriesBase.valueScaleType;
			pointsSorting = seriesBase.pointsSorting;
			pointsSortingKey = seriesBase.pointsSortingKey;
			if (label != null) {
				if (seriesBase.label != null)
					label.Assign(seriesBase.label);
				label.EnsureResolveOverlappingMode();
			}
			pointOptionsSynchronizer.IsSynchronized = seriesBase.pointOptionsSynchronizer.IsSynchronized;
			legendPointOptions.Assign(seriesBase.legendPointOptions);
			argumentDataMember = seriesBase.argumentDataMember;
			colorDataMember = seriesBase.colorDataMember;
			toolTipHintDataMember = seriesBase.toolTipHintDataMember;
			int count = Math.Min(valueDataMembers.Count, seriesBase.valueDataMembers.Count);
			for (int i = 0; i < count; i++)
				valueDataMembers[i] = seriesBase.valueDataMembers[i];
			summaryFunction = seriesBase.summaryFunction;
			DataFilters.ConjunctionMode = seriesBase.dataFilters.ConjunctionMode;
			dataFilters.Clear();
			foreach (DataFilter filter in seriesBase.dataFilters)
				dataFilters.Add((DataFilter)((ICloneable)filter).Clone());
			topNOptions.Assign(seriesBase.topNOptions);
			toolTipPointPattern = seriesBase.toolTipPointPattern;
			toolTipSeriesPattern = seriesBase.toolTipSeriesPattern;
			toolTipEnabled = seriesBase.toolTipEnabled;
			crosshairEnabled = seriesBase.crosshairEnabled;
			crosshairLabelVisibility = seriesBase.crosshairLabelVisibility;
			crosshairLabelPattern = seriesBase.crosshairLabelPattern;
			crosshairHighlightPoints = seriesBase.crosshairHighlightPoints;
			labelsVisibility = seriesBase.labelsVisibility;
			checkedInLegend = seriesBase.checkedInLegend;
			checkableInLegend = seriesBase.checkableInLegend;
			legendTextPattern = seriesBase.legendTextPattern;
			if (seriesBase.colorizer != null)
				this.Colorizer = seriesBase.colorizer;
		}
	}
}
namespace DevExpress.XtraCharts.Native {
	public enum LegendPatternSyncronization {
		LegendPointOptions,
		None
	}
}
