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
using System.Drawing;
using System.Drawing.Design;
using System.Globalization;
using DevExpress.Charts.Native;
using DevExpress.Data.Browsing;
using DevExpress.Data.Design;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.Utils.Serializing;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Localization;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts {
	[
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design"),
	TypeConverter(typeof(SeriesTypeConverter))
	]
	public class Series : SeriesBase, ISupportInitialize, ICustomTypeDescriptor, ISeries, IXtraSerializable, ISupportID {
		#region Nested classes
		class SeriesPropertyDescriptorCollection : PropertyDescriptorCollection {
			bool ShouldProcessPointsProperty(Series series) {
				Chart chart = series.Chart;
				return chart != null && !chart.XtraSerializing && chart.Container != null &&
					chart.Container.ControlType == ChartContainerType.WebControl;
			}
			public SeriesPropertyDescriptorCollection(Series series, ICollection descriptors)
				: base(new PropertyDescriptor[0]) {
				foreach (PropertyDescriptor pd in descriptors)
					if (pd.DisplayName == "DataSource") {
						IChartDataProvider dataProvider = series.ChartContainer != null ? series.ChartContainer.DataProvider : null;
						Add(new CustomPropertyDescriptor(pd, dataProvider == null || dataProvider.SeriesDataSourceVisible));
					}
					else if (pd.DisplayName == "Points" && ShouldProcessPointsProperty(series))
						Add(new CustomPropertyDescriptor(pd, true, series.UnboundMode));
					else
						Add(pd);
			}
		}
		#endregion
		internal static string CheckedInLegendProperty = "CheckedInLegend";
		internal static string GetLegendText(string text, Series series) {
			List<string> parsedPattern = PatternUtils.ParsePattern(text);
			string result = String.Empty;
			foreach (string fragment in parsedPattern)
				switch (fragment) {
					case PatternUtils.SeriesNamePattern:
					case PatternUtils.SeriesNamePatternLowercase:
						result += series.Name;
						break;
					case PatternUtils.StackedGroupPattern:
					case PatternUtils.StackedGroupPatternLowercase:
						ISideBySideStackedBarSeriesView sideBySideStackedBarView = series.View as ISideBySideStackedBarSeriesView;
						result += sideBySideStackedBarView == null ? fragment :
							ObjectToStringConversion.ObjectToString(sideBySideStackedBarView.StackedGroup);
						break;
					default:
						result += fragment;
						break;
				}
			return result;
		}
		readonly SeriesPointCollection points;
		int id = -1;
		string name;
		object dataSource = null;
		bool unboundMode = true;
		bool pointsUpdateInProcess = false;
		bool autocreated = false;
		bool permanent = false;
		bool loading = false;
		ChartImage toolTipImage;
		ScaleType argumentScaleByCore;
		bool CanUseBoundPoints {
			get { return (Chart != null) && Chart.CanUseBoundPoints; }
		}
		bool CanUpdateBoundPoints {
			get {
				if (Owner == null || GetDataSource() == null || String.IsNullOrEmpty(ArgumentDataMember) ||
					(!IsSummaryBinding && ValueDataMembers.Count == 0))
					return false;
				foreach (string valueDataMember in ActualValueDataMembers)
					if (String.IsNullOrEmpty(valueDataMember))
						return false;
				return true;
			}
		}
		protected override bool HasPoints {
			get { return Points != null && Points.Count > 0; }
		}
		internal bool PointsUpdateInProcess { get { return pointsUpdateInProcess || Loading; } }
		internal bool UnboundMode {
			get { return unboundMode; }
		}
		internal bool Autocreated {
			get { return autocreated; }
			set { autocreated = value; }
		}
		internal bool Permanent {
			get { return !autocreated || permanent; }
			set { permanent = value; }
		}
		internal bool CanDataSnapshot {
			get { return !unboundMode && points.Count > 0; }
		}
		internal bool UseTemplateHit {
			get { return autocreated && DesignMode; }
		}
		internal bool UseRandomPoints {
			get { return (DataContainer != null) && !DataContainer.Chart.ViewController.HasProcessedPoints && DataContainer.Chart.ViewController.IsDesignMode; }
		}
		internal HitTestController HitTestController {
			get {
				ChartDebug.Assert(Chart != null, "Chart can not be null");
				return Chart.HitTestController;
			}
		}
		internal string ActualLegendText {
			get {
				if (String.IsNullOrEmpty(LegendText))
					return Name;
				return GetLegendText(LegendText, this);
			}
		}
		protected internal override bool Loading {
			get { return loading || base.Loading; }
		}
		[Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool IsAutoCreated {
			get { return autocreated; }
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("SeriesPoints"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.Series.Points"),
		TypeConverter(typeof(CollectionTypeConverter)),
		Category("Elements"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		Editor("DevExpress.XtraCharts.Design.SeriesCollectionEditor," + AssemblyInfo.SRAssemblyChartsWizard, typeof(UITypeEditor)),
		NestedTagProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Collection, true)]
		public SeriesPointCollection Points {
			get { return points; }
		}
		[Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty]
		public int SeriesID {
			get { return id; }
			set {
				if (!Loading)
					throw new NotSupportedException(ChartLocalizer.GetString(ChartStringId.MsgInternalPropertyChangeError));
				id = value;
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("SeriesName"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.Series.Name"),
		Localizable(true),
		XtraSerializableProperty]
		public string Name {
			get { return name; }
			set {
				if (value != name) {
					SendNotification(new ElementWillChangeNotification(this));
					name = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("SeriesDataSource"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.Series.DataSource"),
		Category("Data"),
		NonTestableProperty,
		AttributeProvider(typeof(IListSource)),
		TypeConverter(typeof(DataSourceConverter)),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty(XtraSerializationVisibility.Reference)]
		public object DataSource {
			get { return dataSource; }
			set {
				if (value != dataSource) {
					if (!Loading && !DesignMode)
						CheckBindingRuntime(value);
					SendNotification(new ElementWillChangeNotification(this));
					SetDataSource(value);
					if ((Chart != null) && (Chart.Container != null) && !Chart.Container.Loading)
						CheckBinding((dataSource == null) ? Chart.DataContainer.DataSource : dataSource);
					BindingChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("SeriesToolTipImage"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.Series.ToolTipImage"),
		Category(Categories.Data),
		XtraSerializableProperty(XtraSerializationVisibility.Content),
		NestedTagProperty,
		TypeConverter(typeof(ExpandableObjectConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NonTestableProperty]
		public ChartImage ToolTipImage { get { return toolTipImage; } }
		public Series(string name, ViewType viewType) : base(viewType) {
			this.name = name;
			toolTipImage = new ChartImage(this);
			points = new SeriesPointCollection(this);
		}
		public Series() : this(String.Empty, SeriesViewFactory.DefaultViewType) {
		}
		#region ISupportID implementation
		int ISupportID.ID {
			get { return id; }
			set {
				ChartDebug.Assert(value >= 0);
				if (value >= 0)
					id = value;
			}
		}
		#endregion
		#region IHitTest implementation
		protected internal override object HitObject { get { return UseTemplateHit ? Owner.SeriesTemplate.HitObject : base.HitObject; } }
		protected internal override SeriesHitTestState HitState { get { return UseTemplateHit ? Owner.SeriesTemplate.HitState : base.HitState; } }
		#endregion
		#region ISeries implementation
		IEnumerable<ISeriesPoint> ISeries.Points { get { return points; } }
		IList<ISeriesPoint> ISeries.ActualPoints { get { return points; } }
		IList<ISeriesPoint> ISeries.PointsToInsert { get { throw new NotImplementedException(); } }
		IList<ISeriesPoint> ISeries.PointsToRemove { get { throw new NotImplementedException(); } }
		bool ISeries.ArePointsSorted { get { return SeriesPointsSorting != SortingMode.None; } }
		bool ISeries.ShouldSortPointsInfo { get { return View.ShouldSortPoints; } }
		bool ISeries.Visible { get { return Visible; } }
		bool ISeries.LabelsVisibility { get { return ActualLabelsVisibility; } }
		bool ISeries.ShouldBeDrawnOnDiagram { get { return ShouldBeDrawnOnDiagram; } }
		IList<string> ISeries.ValueDataMembers { get { return ActualValueDataMembers; } }
		string ISeries.ColorDataMember { get { return ColorDataMember; } }
		IEnumerable<IDataFilter> ISeries.DataFilters { get { return DataFilters; } }
		Conjunction ISeries.DataFiltersConjunction { get { return (Conjunction)DataFilters.ConjunctionMode; } }
		SeriesPointKeyNative ISeries.SeriesPointsSortingKey { get { return (SeriesPointKeyNative)this.SeriesPointsSortingKey; } }
		SortMode ISeries.SeriesPointsSortingMode { get { return (SortMode)this.SeriesPointsSorting; } }
		void ISeries.AddSeriesPoint(ISeriesPoint point) {
			SeriesPoint pointItem = point as SeriesPoint;
			if (pointItem != null)
				points.Add(pointItem);
		}
		void ISeries.SetArgumentScaleType(Scale scaleType) {
			argumentScaleByCore = (ScaleType)scaleType;
		}
		void ISeries.ClearColorCache() {
			ColorProvider.Reset();
		}
		#endregion
		#region ISupportInitialize implementation
		void ISupportInitialize.BeginInit() {
			loading = true;
		}
		void ISupportInitialize.EndInit() {
			loading = false;
		}
		#endregion
		#region ICustomTypeDescriptor implementation
		System.ComponentModel.AttributeCollection ICustomTypeDescriptor.GetAttributes() {
			return TypeDescriptor.GetAttributes(this, true);
		}
		System.ComponentModel.TypeConverter ICustomTypeDescriptor.GetConverter() {
			return TypeDescriptor.GetConverter(this, true);
		}
		EventDescriptor ICustomTypeDescriptor.GetDefaultEvent() {
			return TypeDescriptor.GetDefaultEvent(this, true);
		}
		PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty() {
			return TypeDescriptor.GetDefaultProperty(this, true);
		}
		object ICustomTypeDescriptor.GetEditor(Type editorBaseType) {
			return TypeDescriptor.GetEditor(this, editorBaseType, true);
		}
		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes) {
			return new SeriesPropertyDescriptorCollection(this, TypeDescriptor.GetProperties(this, true));
		}
		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties() {
			return new SeriesPropertyDescriptorCollection(this, TypeDescriptor.GetProperties(this, true));
		}
		string ICustomTypeDescriptor.GetClassName() {
			return GetType().Name;
		}
		string ICustomTypeDescriptor.GetComponentName() {
			return GetType().Name;
		}
		EventDescriptorCollection ICustomTypeDescriptor.GetEvents() {
			return TypeDescriptor.GetEvents(this, true);
		}
		EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes) {
			return TypeDescriptor.GetEvents(this, attributes, true);
		}
		object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd) {
			return this;
		}
		#endregion
		#region ShouldSerialize & Reset
		bool ShouldSerializePoints() {
			return unboundMode;
		}
		bool ShouldSerializeSeriesID() {
			foreach (SeriesPoint point in points)
				if (point.Annotations.Count > 0)
					return true;
			return false;
		}
		bool ShouldSerializeName() {
			return !String.IsNullOrEmpty(name);
		}
		void ResetName() {
			Name = string.Empty;
		}
		bool ShouldSerializeDataSource() {
			return ChartContainer != null && ChartContainer.DataProvider != null && ChartContainer.DataProvider.ShouldSerializeDataSource(dataSource);
		}
		void ResetDataSource() {
			DataSource = null;
		}
		bool ShouldSerializeToolTipImage() {
			return toolTipImage.ShouldSerialize();
		}
		protected internal override bool ShouldSerialize() {
			return true;
		}
		#endregion
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			switch (propertyName) {
				case "Name":
					return ShouldSerializeName();
				case "Points":
					return ShouldSerializePoints();
				case "SeriesID":
					return ShouldSerializeSeriesID();
				case "ToolTipImage":
					return ShouldSerializeToolTipImage();
				case "DataSource":
					return ChartContainer != null && ChartContainer.ControlType == ChartContainerType.XRControl && dataSource != null;
				default:
					return base.XtraShouldSerialize(propertyName);
			}
		}
		protected override void XtraSetIndexCollectionItem(string propertyName, XtraSetItemIndexEventArgs e) {
			if (propertyName == "Points")
				Points.Add((SeriesPoint)e.Item.Value);
			base.XtraSetIndexCollectionItem(propertyName, e);
		}
		protected override object XtraCreateCollectionItem(string propertyName, XtraItemEventArgs e) {
			return propertyName == "Points" ? new SeriesPoint() : base.XtraCreateCollectionItem(propertyName, e);
		}
		void IXtraSerializable.OnStartSerializing() {
		}
		void IXtraSerializable.OnEndSerializing() {
		}
		void IXtraSerializable.OnStartDeserializing(LayoutAllowEventArgs e) {
			((ISupportInitialize)this).BeginInit();
		}
		void IXtraSerializable.OnEndDeserializing(string restoredVersion) {
			((ISupportInitialize)this).EndInit();
		}
		#endregion
		void DisposePoints(List<ISeriesPoint> points) {
			if (points != null)
				foreach (SeriesPoint point in points)
					point.Dispose();
		}
		void DataWasChanged(object sender, ListChangedEventArgs args) {
			if ((Chart != null) && Chart.ViewController.RefreshDataInternal(true) && ChartContainer != null && ChartContainer.Chart != null)
				ContainerAdapter.Invalidate();
		}
		void UnsubscribeDataSourceEvent() {
			IBindingList bindingList = this.dataSource as IBindingList;
			if (bindingList != null)
				bindingList.ListChanged -= new ListChangedEventHandler(DataWasChanged);
		}
		void SetDataSource(object dataSource) {
			ResetDataSourceArgumentScaleType();
			UnsubscribeDataSourceEvent();
			this.dataSource = dataSource;
			IBindingList bindingList = dataSource as IBindingList;
			if (bindingList != null)
				bindingList.ListChanged += new ListChangedEventHandler(DataWasChanged);
		}
		void ValidateArguments(ScaleType scaleType) {
			if (scaleType == ScaleType.Auto)
				return;
			if (!UseRandomPoints && (Points != null) && !Loading)
				foreach (SeriesPoint point in Points)
					if (!point.IsAuxiliary && !AxisScaleTypeMap.CheckArgumentScaleType(point, (Scale)scaleType)) {
						string message = String.Format(ChartLocalizer.GetString(ChartStringId.MsgIncompatiblePointType),
							point.Argument, GetScaleTypeName(scaleType));
						throw new ArgumentException(message);
					}
		}
		void AlignPointsToView() {
			if (points != null)
				foreach (SeriesPoint point in points)
					point.IncreaseDimension(View.PointDimension);
		}
		IList<ISeriesPoint> GetBoundPoints() {
			if (!CanUpdateBoundPoints)
				return null;
			IChartContainer chartContainer = ChartContainer;
			DataContextBase dataContext = ContainerAdapter.DataContext;
			object dataSource = GetDataSource();
			string dataMember = GetChartDataMember();
			SeriesBindingProcedure bindingProc;
			if (IsSummaryBinding) {
				string functionName = new SummaryFunctionParser(SummaryFunction).FunctionName;
				SummaryFunctionDescription desc = chartContainer.Chart.SummaryFunctions[functionName];
				if (desc == null)
					if (chartContainer.DesignMode)
						return new List<ISeriesPoint>();
					else {
						string message = String.Format(ChartLocalizer.GetString(ChartStringId.MsgSummaryFunctionIsNotRegistered), functionName);
						throw new InvalidOperationException(message);
					}
				bindingProc = new SummarySeriesBindingProcedure(this, dataContext, dataSource, dataMember, desc.Function);
			}
			else
				bindingProc = new SimpleSeriesBindingProcedure(this, dataContext, dataSource, dataMember);
			return bindingProc.CreateBindingPoints();
		}
		internal bool ShouldRefreshData() {
			return dataSource != null && !(dataSource is IBindingList);
		}
		internal bool IsBoundDataChanged() {
			if (CanUseBoundPoints) {
				IList<ISeriesPoint> newPoints = GetBoundPoints();
				if (newPoints != null)
					return !points.ArePointEquals(newPoints);
			}
			return false;
		}
		internal void UpdateBoundPoints() {
			if (CanUseBoundPoints) {
				Chart.Container.LockChangeService();
				points.Unlock = true;
				points.BeginUpdate(true, true);
				UpdateScaleFromDataSource();
				try {
					IList<ISeriesPoint> boundPoints = GetBoundPoints();
					if (boundPoints == null) {
						if (!unboundMode) {
							points.Clear();
							unboundMode = true;
						}
					}
					else {
						unboundMode = true;
						points.Clear();
						foreach (SeriesPoint point in boundPoints)
							points.Add(point);
						unboundMode = false;
					}
				}
				finally {
					points.EndUpdate(true);
					points.Unlock = false;
					Chart.Container.UnlockChangeService();
				}
			}
		}
		internal void DataSnapshot() {
			autocreated = false;
			unboundMode = true;
			SetDataSource(null);
			ArgumentDataMember = String.Empty;
			for (int i = 0; i < ValueDataMembers.Count; i++)
				ValueDataMembers[i] = String.Empty;
			DataFilters.Clear();
		}
		internal void ClearAnnotations() {
			foreach (SeriesPoint point in Points)
				point.ClearAnnotations();
		}
		internal void RenderLegendItem(IRenderer renderer, Rectangle bounds, DrawOptions seriesPointDrawOptions, DrawOptions seriesDrawOptions, RefinedSeriesData seriesData, RefinedPointData pointData) {
			HitTestController hitTestController = HitTestController;
			if (View.HitTestingSupportedForLegendMarker)
				renderer.ProcessHitTestRegion(hitTestController, this, pointData != null ? pointData.RefinedPoint : null, new HitRegion(bounds));
			if (hitTestController.PointToolTipEnabled(this) && pointData != null)
				renderer.ProcessHitTestRegion(hitTestController, this, new ChartFocusedArea(pointData.SeriesPoint), new HitRegion(bounds), true);
			else if (hitTestController.SeriesToolTipEnabled(this))
				renderer.ProcessHitTestRegion(hitTestController, this, new ChartFocusedArea(this), new HitRegion(bounds), true);
			View.RenderLegendMarker(renderer, bounds, seriesPointDrawOptions, seriesDrawOptions, seriesData, pointData);
		}
		internal void UpdateArgumentDateTimeFormat(DateTimeMeasureUnit measurementUnit) {
			DateTimeFormat dateTimeFormat;
			string formatString = String.Empty;
			DateTimeFormatUtils.GetDateTimeFormat(measurementUnit, out dateTimeFormat, out formatString);
			if (Label != null) {
				Label.TextPattern = PointOptions.ChangeArgumentDateTimeFormat(Label.ActualTextPattern, dateTimeFormat, formatString);
				LegendTextPattern = PointOptions.ChangeArgumentDateTimeFormat(ActualLegendTextPattern, dateTimeFormat, formatString);
			}
		}
		internal void BeginPointsUpdate() {
			pointsUpdateInProcess = true;
		}
		internal void EndPointsUpdate() {
			pointsUpdateInProcess = false;
			RaiseControlChanged(new ReprocessPointsUpdate(this));
		}
		protected override void Dispose(bool disposing) {
			if (disposing) {
				UnsubscribeDataSourceEvent();
				if (points != null)
					points.Dispose();
			}
			base.Dispose(disposing);
		}
		protected override ChartElement CreateObjectForClone() {
			return new Series();
		}
		protected override void ProcessSetView() {
			base.ProcessSetView();
			AlignPointsToView();
		}
		protected override void SetValueScaleType(ScaleType valueScaleType) {
			SetValueScaleTypeField(valueScaleType);
			foreach (SeriesPoint point in points)
				if (point.IsEmpty)
					point.ValueScaleType = valueScaleType;
		}
		protected override ScaleType GetActualArgumentScaleType() {
			ScaleType dataSourceScale = GetScaleFromDataSource();
			if (dataSourceScale != ScaleType.Auto)
				return dataSourceScale;
			return argumentScaleByCore;
		}
		protected internal override void SetArgumentScaleType(ScaleType argumentScaleType) {
			ValidateArguments(argumentScaleType);
			base.SetArgumentScaleType(argumentScaleType);
		}
		protected internal override void ValidatePointsByArgumentScaleType(ScaleType argumentScaleType) {
			base.ValidatePointsByArgumentScaleType(argumentScaleType);
			ValidateArguments(argumentScaleType);
		}
		protected internal override void ValidatePointsByValueScaleType(ScaleType valueScaleType) {
			base.ValidatePointsByValueScaleType(valueScaleType);
			foreach (SeriesPoint point in points)
				if (!point.CheckValueScaleType(valueScaleType)) {
					string message = String.Format(ChartLocalizer.GetString(ChartStringId.MsgIncompatiblePointType),
						point.Argument, GetScaleTypeName(valueScaleType));
					throw new ArgumentException(message);
				}
		}
		protected internal override void OnEndLoading(bool validatePointsId) {
			AlignPointsToView();
			Points.OnEndLoading(validatePointsId);
			base.OnEndLoading(validatePointsId);
		}
		protected internal override void BindingChanged(ChartUpdateInfoBase changeInfo) {
			if (DataContainer != null)
				DataContainer.UpdateBinding(false, true);
		}
		protected override bool ProcessChanged(ChartElement sender, ChartUpdateInfoBase changeInfo) {
			if (Label != null)
				Label.UpdateTextPattern(sender);
			UpdateLegendTextPattern(sender);
			if (Owner != null)
				Owner.DataChanged(changeInfo);
			return false;
		}
		protected internal override void ChildCollectionChanged(ChartCollectionBase collection, ChartUpdateInfoBase changeInfo) {
			if (Owner != null) {
				if (collection == points)
					Owner.DataChanged(changeInfo);
			}
		}
		protected internal bool IsNegative() {
			IRefinedSeries refinedSeries = Chart.ViewController.FindRefinedSeries(this);
			if (refinedSeries != null) {
				foreach (IValuePoint point in refinedSeries.Points) {
					if (point.Value < 0)
						return true;
					else if (point.Value > 0)
						break;
				}
			}
			return false;
		}
		public override string ToString() {
			if (!Visible)
				return ChartLocalizer.GetString(ChartStringId.InvisibleSeriesView) + " " + name;
			return ((Chart != null) && Chart.ViewController.SeriesIncompatibilityStatistics.IsSeriesIncompatible(this)) ?
				(ChartLocalizer.GetString(ChartStringId.IncompatibleSeriesView) + " " + name) : name;
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			Series series = obj as Series;
			if (series != null) {
				name = series.name;
				toolTipImage.Assign(series.toolTipImage);
				points.Assign(series.points);
				View.SynchronizePoints();
				autocreated = series.autocreated;
				unboundMode = series.unboundMode;
				SetDataSource(series.dataSource);
				if (!Loading)
					View.OnEndLoading();
			}
		}
		public bool IsCompatible(SeriesPoint point) {
			if (point == null)
				throw new ArgumentNullException("point");
			return AxisScaleTypeMap.CheckArgumentScaleType(point, (Scale)ArgumentScaleType) && point.CheckValueScaleType(ValueScaleType);
		}
		public void BindToData(object dataSource, string argumentDataMember, params string[] valueDataMembers) {
			if (dataSource != DataSource)
				CheckBindingRuntime(dataSource);
			object data = dataSource != null ? dataSource :
				Owner != null ? Owner.DataSource : null;
			if (data == null)
				return;
			if (argumentDataMember != ArgumentDataMember)
				CheckDataMember(data, argumentDataMember, ArgumentScaleType);
			foreach (string valueDataMember in valueDataMembers)
				CheckDataMember(data, valueDataMember, ValueScaleType);
			SendNotification(new ElementWillChangeNotification(this));
			SetDataSource(dataSource);
			SetArgumentDataMember(argumentDataMember);
			ValueDataMembers.Full(valueDataMembers);
			BindingChanged();
		}
	}
}
