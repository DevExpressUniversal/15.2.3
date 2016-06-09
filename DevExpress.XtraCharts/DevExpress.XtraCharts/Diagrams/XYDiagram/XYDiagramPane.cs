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
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Drawing.Design;
using DevExpress.Charts.Native;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.Utils.Serializing;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Localization;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts {
	[
	TypeConverter(typeof(EnumTypeConverter)),
	ResourceFinder(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile),
	]
	public enum PaneSizeMode {
		UseWeight,
		UseSizeInPixels
	}
	[DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")]
	public abstract class XYDiagramPaneBase : ChartElementNamed, IZoomablePane, IBackground, IDockTarget, ISupportTextAntialiasing {
		const PaneSizeMode DefaultSizeMode = PaneSizeMode.UseWeight;
		const double DefaultWeight = 1.0;
		readonly AnnotationCollection annotations;
		readonly RectangleFillStyle fillStyle;
		readonly BackgroundImage backImage;
		readonly Shadow shadow;
		readonly ScrollBarOptions scrollBarOptions;
		bool visible = true;
		PaneSizeMode sizeMode = DefaultSizeMode;
		double weight = DefaultWeight;
		int sizeInPixels;
		Color backColor = Color.Empty;
		bool borderVisible = true;
		Color borderColor = Color.Empty;
		Rectangle? lastMappingBounds = null;
		XYDiagramMappingList mappingList = null;
		ResolveOverlappingCache resolveOverlappingCache;
		DefaultBoolean enableAxisXScrolling = DefaultBoolean.Default;
		DefaultBoolean enableAxisYScrolling = DefaultBoolean.Default;
		DefaultBoolean enableAxisXZooming = DefaultBoolean.Default;
		DefaultBoolean enableAxisYZooming = DefaultBoolean.Default;
		ZoomRectangle zoomRectangle;
		Dictionary<XYDiagramMappingBase, Region> seriesClipRegions;
		Chart Chart { get { return Diagram != null ? Diagram.Chart : null; } }
		Color ActualBorderColor { get { return borderColor.IsEmpty ? Diagram.Appearance.BorderColor : borderColor; } }
		Color ActualBackColor { get { return backColor.IsEmpty ? Diagram.Appearance.BackColor : backColor; } }
		RectangleFillStyle ActualFillStyle {
			get { return fillStyle.FillMode == FillMode.Empty ? (RectangleFillStyle)Diagram.Appearance.FillStyle : fillStyle; }
		}
		protected abstract int PaneIndex { get; }
		internal XYDiagram2D Diagram { get { return (XYDiagram2D)Owner; } }
		internal bool Fixed { get { return sizeMode == PaneSizeMode.UseSizeInPixels; } }
		internal XYDiagramMappingList MappingList {
			get {
				if (Chart.CacheToMemory) {
					XYDiagramCoordinatesConversionCache cache = (XYDiagramCoordinatesConversionCache)Chart.GetCoordinatesConversionCache();
					if (cache != null) {
						PaneCoordinatesConversionCache paneCache = cache.GetPaneCoordinatesConversionCache(this);
						if (paneCache != null)
							return paneCache.MappingList;
					}
				}
				return mappingList;
			}
			set { mappingList = value; }
		}
		internal Rectangle? LastMappingBounds {
			get {
				Chart chart = Chart;
				if (chart.CacheToMemory) {
					XYDiagramCoordinatesConversionCache cache = chart.GetCoordinatesConversionCache() as XYDiagramCoordinatesConversionCache;
					if (cache != null) {
						PaneCoordinatesConversionCache paneCache = cache.GetPaneCoordinatesConversionCache(this);
						if (paneCache != null)
							return paneCache.MappingBounds;
					}
				}
				return lastMappingBounds;
			}
			set { lastMappingBounds = value; }
		}
		internal ResolveOverlappingCache ResolveOverlappingCache {
			get { return resolveOverlappingCache; }
			set { resolveOverlappingCache = value; }
		}
		internal Dictionary<XYDiagramMappingBase, Region> SeriesClipRegions {
			get { return seriesClipRegions; }
			set { seriesClipRegions = value; }
		}
		protected internal abstract int ActualPaneID { get; }
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public bool ActualEnableAxisXScrolling {
			get {
				XYDiagram2D diagram = Diagram;
				return ((Chart != null) && (Chart.IsRangeControlClient)) || (DefaultBooleanUtils.ToBoolean(EnableAxisXScrolling, diagram != null && diagram.EnableAxisXScrolling));
			}
		}
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public bool ActualEnableAxisYScrolling {
			get {
				XYDiagram2D diagram = Diagram;
				return DefaultBooleanUtils.ToBoolean(EnableAxisYScrolling, diagram != null && diagram.EnableAxisYScrolling);
			}
		}
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public bool ActualEnableAxisXZooming {
			get {
				XYDiagram2D diagram = Diagram;
				return DefaultBooleanUtils.ToBoolean(EnableAxisXZooming, diagram != null && diagram.EnableAxisXZooming);
			}
		}
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public bool ActualEnableAxisYZooming {
			get {
				XYDiagram2D diagram = Diagram;
				return DefaultBooleanUtils.ToBoolean(EnableAxisYZooming, diagram != null && diagram.EnableAxisYZooming);
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("XYDiagramPaneBaseAnnotations"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.XYDiagramPaneBase.Annotations"),
		Editor("DevExpress.XtraCharts.Design.AnnotationCollectionEditor," + AssemblyInfo.SRAssemblyChartsWizard, typeof(UITypeEditor)),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		TypeConverter(typeof(CollectionTypeConverter))
		]
		public AnnotationCollection Annotations {
			get {
				annotations.Update();
				return annotations;
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("XYDiagramPaneBaseWeight"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.XYDiagramPaneBase.Weight"),
		Category(Categories.Appearance),
		NonTestableProperty,
		XtraSerializableProperty
		]
		public double Weight {
			get { return weight; }
			set {
				if (value == weight)
					return;
				if (value <= 0)
					throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectPaneWeight));
				SendNotification(new ElementWillChangeNotification(this));
				weight = value;
				RaiseControlChanged();
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("XYDiagramPaneBaseSizeMode"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.XYDiagramPaneBase.SizeMode"),
		Category(Categories.Appearance),
		XtraSerializableProperty,
		RefreshProperties(RefreshProperties.All)
		]
		public PaneSizeMode SizeMode {
			get { return sizeMode; }
			set {
				if (value != sizeMode) {
					SendNotification(new ElementWillChangeNotification(this));
					sizeMode = value;
					if (sizeMode == PaneSizeMode.UseWeight && !Loading)
						CalculateDynamicWeight();
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("XYDiagramPaneBaseSizeInPixels"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.XYDiagramPaneBase.SizeInPixels"),
		Category(Categories.Appearance),
		NonTestableProperty,
		XtraSerializableProperty
		]
		public int SizeInPixels {
			get { return sizeInPixels; }
			set {
				if (value < 0)
					throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgInvalidPaneSizeInPixels));
				if (value != sizeInPixels) {
					SendNotification(new ElementWillChangeNotification(this));
					sizeInPixels = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("XYDiagramPaneBaseBorderColor"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.XYDiagramPaneBase.BorderColor"),
		Category(Categories.Appearance),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty
		]
		public Color BorderColor {
			get { return borderColor; }
			set {
				if (value != borderColor) {
					SendNotification(new ElementWillChangeNotification(this));
					borderColor = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("XYDiagramPaneBaseBorderVisible"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.XYDiagramPaneBase.BorderVisible"),
		TypeConverter(typeof(BooleanTypeConverter)),
		Category(Categories.Appearance),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty
		]
		public bool BorderVisible {
			get { return borderVisible; }
			set {
				if (value != borderVisible) {
					SendNotification(new ElementWillChangeNotification(this));
					borderVisible = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("XYDiagramPaneBaseBackColor"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.XYDiagramPaneBase.BackColor"),
		Category(Categories.Appearance),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty
		]
		public Color BackColor {
			get { return backColor; }
			set {
				if (value != backColor) {
					SendNotification(new ElementWillChangeNotification(this));
					backColor = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("XYDiagramPaneBaseBackImage"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.XYDiagramPaneBase.BackImage"),
		Category(Categories.Appearance),
		TypeConverter(typeof(ExpandableObjectConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NestedTagProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Content)
		]
		public BackgroundImage BackImage { get { return backImage; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("XYDiagramPaneBaseFillStyle"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.XYDiagramPaneBase.FillStyle"),
		Category(Categories.Appearance),
		TypeConverter(typeof(ExpandableObjectConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NestedTagProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Content),
		]
		public RectangleFillStyle FillStyle { get { return fillStyle; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("XYDiagramPaneBaseShadow"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.XYDiagramPaneBase.Shadow"),
		Category(Categories.Appearance),
		TypeConverter(typeof(ExpandableObjectConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NestedTagProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Content)
		]
		public Shadow Shadow { get { return shadow; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("XYDiagramPaneBaseScrollBarOptions"),
#endif
		DXDisplayNameIgnore,
		Category(Categories.Appearance),
		TypeConverter(typeof(ExpandableObjectConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NestedTagProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Content)
		]
		public ScrollBarOptions ScrollBarOptions { get { return scrollBarOptions; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("XYDiagramPaneBaseVisible"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.XYDiagramPaneBase.Visible"),
		TypeConverter(typeof(BooleanTypeConverter)),
		Category(Categories.Appearance),
		XtraSerializableProperty
		]
		public bool Visible {
			get { return visible; }
			set {
				if (value != visible) {
					SendNotification(new ElementWillChangeNotification(this));
					visible = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("XYDiagramPaneBaseEnableAxisXScrolling"),
#endif
		DXDisplayNameIgnore,
		TypeConverter(typeof(DefaultBooleanConverter)),
		Category(Categories.Behavior),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty
		]
		public DefaultBoolean EnableAxisXScrolling {
			get { return enableAxisXScrolling; }
			set {
				if (value != enableAxisXScrolling) {
					SendNotification(new ElementWillChangeNotification(this));
					enableAxisXScrolling = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("XYDiagramPaneBaseEnableAxisYScrolling"),
#endif
		DXDisplayNameIgnore,
		TypeConverter(typeof(DefaultBooleanConverter)),
		Category(Categories.Behavior),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty
		]
		public DefaultBoolean EnableAxisYScrolling {
			get { return enableAxisYScrolling; }
			set {
				if (value != enableAxisYScrolling) {
					SendNotification(new ElementWillChangeNotification(this));
					enableAxisYScrolling = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("XYDiagramPaneBaseEnableAxisXZooming"),
#endif
		DXDisplayNameIgnore,
		TypeConverter(typeof(DefaultBooleanConverter)),
		Category(Categories.Behavior),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty
		]
		public DefaultBoolean EnableAxisXZooming {
			get { return enableAxisXZooming; }
			set {
				if (value != enableAxisXZooming) {
					SendNotification(new ElementWillChangeNotification(this));
					enableAxisXZooming = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("XYDiagramPaneBaseEnableAxisYZooming"),
#endif
		DXDisplayNameIgnore,
		TypeConverter(typeof(DefaultBooleanConverter)),
		Category(Categories.Behavior),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty
		]
		public DefaultBoolean EnableAxisYZooming {
			get { return enableAxisYZooming; }
			set {
				if (value != enableAxisYZooming) {
					SendNotification(new ElementWillChangeNotification(this));
					enableAxisYZooming = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("XYDiagramPaneBaseZoomRectangle"),
#endif
		DXDisplayNameIgnore,
		TypeConverter(typeof(ExpandableObjectConverter)),
		Category(Categories.Appearance),
		NestedTagProperty,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content)
		]
		public ZoomRectangle ZoomRectangle { get { return zoomRectangle; } }
		protected XYDiagramPaneBase(string name, XYDiagram2D diagram) : base(name, diagram) {
			annotations = new AnnotationCollection(new AnnotationCollectionPaneBehavior(this));
			fillStyle = new RectangleFillStyle(this);
			backImage = new BackgroundImage(this);
			shadow = new Shadow(this);
			scrollBarOptions = new ScrollBarOptions(this);
			zoomRectangle = new ZoomRectangle(this);
		}
		#region IPane implementation
		int IPane.PaneIndex { get { return PaneIndex; } }
		GRealRect2D? IPane.MappingBounds {
			get {
				if (lastMappingBounds.HasValue)
					return new GRealRect2D(lastMappingBounds.Value.X, lastMappingBounds.Value.Y, lastMappingBounds.Value.Width, lastMappingBounds.Value.Height);
				return null;
			}
		}
		#endregion
		#region IBackground implementation
		FillStyleBase IBackground.FillStyle { get { return FillStyle; } }
		bool IBackground.BackImageSupported { get { return true; } }
		#endregion
		#region IZoomablePane implementation
		bool IZoomablePane.Rotated { get { return Diagram.ActualRotated; } }
		bool IZoomablePane.ScrollingByXEnabled { get { return ActualEnableAxisXScrolling; } }
		bool IZoomablePane.ScrollingByYEnabled { get { return ActualEnableAxisYScrolling; } }
		bool IZoomablePane.ZoomingByXEnabled { get { return ActualEnableAxisXZooming; } }
		bool IZoomablePane.ZoomingByYEnabled { get { return ActualEnableAxisYZooming; } }
		double IZoomablePane.AxisXMaxZoomPercent { get { return Diagram.ZoomingOptions.AxisXMaxZoomPercent; } }
		double IZoomablePane.AxisYMaxZoomPercent { get { return Diagram.ZoomingOptions.AxisYMaxZoomPercent; } }
		ZoomCacheEx IZoomablePane.ZoomCacheEx { get { return Diagram.ZoomCacheEx; } }
		GRect2D IZoomablePane.Bounds {
			get { return lastMappingBounds.HasValue ? GraphicUtils.ConvertRect(lastMappingBounds.Value) : GRect2D.Empty; }
		}
		void IZoomablePane.RangeLimitsUpdated() {
		}
		void IZoomablePane.BeginZooming() {
			Diagram.BeginZooming();
		}
		void IZoomablePane.EndZooming(NavigationType navigationType, RangesSnapshot oldRange, RangesSnapshot newRange) {
			Diagram.EndZooming((ChartZoomEventType)navigationType, oldRange, newRange, newRange.AxisX as AxisBase, newRange.AxisY as AxisBase);
		}
		void IZoomablePane.EndScrolling(ScrollingOrientation orientation, NavigationType navigationType, RangesSnapshot oldRange, RangesSnapshot newRange) {
			Diagram.EndScrolling((ChartScrollOrientation)orientation, (ChartScrollEventType)navigationType, oldRange, newRange, newRange.AxisX as AxisBase, newRange.AxisY as AxisBase);
		}
		#endregion
		#region ISupportTextAntialiasing implementation
		DefaultBoolean ISupportTextAntialiasing.EnableAntialiasing { get { return DefaultBoolean.Default; } }
		bool ISupportTextAntialiasing.DefaultAntialiasing { get { return true; } }
		Color ISupportTextAntialiasing.TextBackColor { get { return ActualBackColor; } }
		bool ISupportTextAntialiasing.Rotated { get { return false; } }
		RectangleFillStyle ISupportTextAntialiasing.TextBackFillStyle { get { return ActualFillStyle; } }
		ChartElement ISupportTextAntialiasing.BackElement { get { return this; } }
		#endregion
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			switch (propertyName) {
				case "Weight":
					return ShouldSerializeWeight();
				case "SizeMode":
					return ShouldSerializeSizeMode();
				case "SizeInPixels":
					return ShouldSerializeSizeInPixels();
				case "BorderColor":
					return ShouldSerializeBorderColor();
				case "BorderVisible":
					return ShouldSerializeBorderVisible();
				case "BackColor":
					return ShouldSerializeBackColor();
				case "BackImage":
					return ShouldSerializeBackImage();
				case "FillStyle":
					return ShouldSerializeFillStyle();
				case "Shadow":
					return ShouldSerializeShadow();
				case "ScrollBarOptions":
					return ShouldSerializeScrollBarOptions();
				case "Visible":
					return ShouldSerializeVisible();
				case "EnableAxisXScrolling":
					return ShouldSerializeEnableAxisXScrolling();
				case "EnableAxisYScrolling":
					return ShouldSerializeEnableAxisYScrolling();
				case "EnableAxisXZooming":
					return ShouldSerializeEnableAxisXZooming();
				case "EnableAxisYZooming":
					return ShouldSerializeEnableAxisYZooming();
				case "ZoomRectangle":
					return ShouldSerializeZoomRectangle();
				default:
					return base.XtraShouldSerialize(propertyName);
			}
		}
		#endregion
		#region ShouldSerialize & Reset
		bool ShouldSerializeScrollingZoomingProperties() {
			IChartContainer chartContainer = ChartContainer;
			return chartContainer == null || chartContainer.ControlType != ChartContainerType.WebControl;
		}
		bool ShouldSerializeWeight() {
			return sizeMode == PaneSizeMode.UseWeight && weight != DefaultWeight;
		}
		void ResetWeight() {
			Weight = DefaultWeight;
		}
		bool ShouldSerializeSizeMode() {
			return sizeMode != DefaultSizeMode;
		}
		void ResetSizeMode() {
			SizeMode = DefaultSizeMode;
		}
		bool ShouldSerializeSizeInPixels() {
			return sizeMode == PaneSizeMode.UseSizeInPixels;
		}
		void ResetSizeInPixels() {
			SizeInPixels = 0;
		}
		bool ShouldSerializeBorderColor() {
			return !this.borderColor.IsEmpty;
		}
		void ResetBorderColor() {
			BorderColor = Color.Empty;
		}
		bool ShouldSerializeBorderVisible() {
			return !this.borderVisible;
		}
		void ResetBorderVisible() {
			BorderVisible = true;
		}
		bool ShouldSerializeBackColor() {
			return !this.backColor.IsEmpty;
		}
		void ResetBackColor() {
			BackColor = Color.Empty;
		}
		bool ShouldSerializeBackImage() {
			return backImage.ShouldSerialize();
		}
		bool ShouldSerializeFillStyle() {
			return fillStyle.ShouldSerialize();
		}
		bool ShouldSerializeShadow() {
			return shadow.ShouldSerialize();
		}
		bool ShouldSerializeScrollBarOptions() {
			return scrollBarOptions.ShouldSerialize();
		}
		bool ShouldSerializeVisible() {
			return !visible;
		}
		void ResetVisible() {
			Visible = true;
		}
		bool ShouldSerializeEnableAxisXScrolling() {
			return ShouldSerializeScrollingZoomingProperties() && enableAxisXScrolling != DefaultBoolean.Default;
		}
		void ResetEnableAxisXScrolling() {
			EnableAxisXScrolling = DefaultBoolean.Default;
		}
		bool ShouldSerializeEnableAxisYScrolling() {
			return ShouldSerializeScrollingZoomingProperties() && enableAxisYScrolling != DefaultBoolean.Default;
		}
		void ResetEnableAxisYScrolling() {
			EnableAxisYScrolling = DefaultBoolean.Default;
		}
		bool ShouldSerializeEnableAxisXZooming() {
			return ShouldSerializeScrollingZoomingProperties() && enableAxisXZooming != DefaultBoolean.Default;
		}
		void ResetEnableAxisXZooming() {
			EnableAxisXZooming = DefaultBoolean.Default;
		}
		bool ShouldSerializeEnableAxisYZooming() {
			return ShouldSerializeScrollingZoomingProperties() && enableAxisYZooming != DefaultBoolean.Default;
		}
		void ResetEnableAxisYZooming() {
			EnableAxisYZooming = DefaultBoolean.Default;
		}
		bool ShouldSerializeZoomRectangle() {
			return zoomRectangle.ShouldSerialize();
		}
		protected internal override bool ShouldSerialize() {
			return
				base.ShouldSerialize() ||
				ShouldSerializeWeight() ||
				ShouldSerializeSizeMode() ||
				ShouldSerializeSizeInPixels() ||
				ShouldSerializeBorderColor() ||
				ShouldSerializeBorderVisible() ||
				ShouldSerializeBackColor() ||
				ShouldSerializeBackImage() ||
				ShouldSerializeFillStyle() ||
				ShouldSerializeShadow() ||
				ShouldSerializeScrollBarOptions() ||
				ShouldSerializeVisible() ||
				ShouldSerializeEnableAxisXScrolling() ||
				ShouldSerializeEnableAxisYScrolling() ||
				ShouldSerializeEnableAxisXZooming() ||
				ShouldSerializeEnableAxisYZooming() ||
				ShouldSerializeZoomRectangle();
		}
		#endregion
		void CalculateDynamicWeight() {
			if (!Visible)
				return;
			XYDiagram diagram = Owner as XYDiagram;
			if (diagram == null)
				return;
			IList<IPane> actualPanes = diagram.ActualPanes;
			double weightSum = 0.0;
			double minWeight = double.MaxValue;
			int sizeInPixelsSum = 0;
			foreach (XYDiagramPaneBase pane in actualPanes) {
				if (!pane.Fixed && pane != this) {
					weightSum += pane.Weight;
					minWeight = Math.Min(minWeight, pane.Weight);
					sizeInPixelsSum += pane.SizeInPixels;
				}
			}
			if (weightSum == 0.0)
				return;
			if (sizeInPixelsSum > 0) {
				if (this.sizeInPixels == 0)
					return;
				this.weight = weightSum * (double)this.sizeInPixels / sizeInPixelsSum;
			} else {
				ChartDebug.Assert(minWeight != double.MaxValue);
				this.weight = minWeight;
			}
		}
		void RenderInnerBorder(IRenderer renderer, XYDiagramPaneArea paneArea) {
			paneArea.RenderBorder(renderer, 1, ActualBorderColor, true);
		}
		void RenderOuterBorder(IRenderer renderer, XYDiagramPaneArea paneArea) {
			HitTestState hitState = (this is IHitTest) ? ((IHitTest)this).State : ((IHitTest)Diagram).State;
			int borderThickness = ThicknessUtils.CorrectThicknessByHitState(borderVisible ? 1 : 0, hitState);
			Color borderColor = GraphicUtils.GetColor(ActualBorderColor, hitState);
			paneArea.RenderBorder(renderer, borderThickness, borderColor, false);
		}
		protected override void Dispose(bool disposing) {
			if (disposing) {
				annotations.Dispose();
				fillStyle.Dispose();
				backImage.Dispose();
				shadow.Dispose();
				scrollBarOptions.Dispose();
			}
			base.Dispose(disposing);
		}
		internal void SetSizeInPixels(int sizeInPixels) {
			if (sizeInPixels >= 0)
				this.sizeInPixels = sizeInPixels;
		}
		internal bool ContainsPoint(Point point) {
			if (!lastMappingBounds.HasValue)
				return false;
			Rectangle bounds = lastMappingBounds.Value;
			bounds.Width++;
			bounds.Height++;
			return bounds.Contains(point);
		}
		internal void RenderBeforeContent(IRenderer renderer, Rectangle mappingBounds, Rectangle maxBounds, Region clipRegion) {
			HitTestController hitTestController = Chart.HitTestController;
			renderer.ProcessHitTestRegion(hitTestController, Diagram, null, new HitRegion(mappingBounds));
			IHitTest hitTest = this as IHitTest;
			if (hitTest != null)
				renderer.ProcessHitTestRegion(hitTestController, hitTest, null, new HitRegion(mappingBounds));
			if (maxBounds.AreWidthAndHeightPositive())
				renderer.ProcessHitTestRegion(hitTestController, Chart, new ChartFocusedArea(this), new HitRegion(maxBounds), true);
			shadow.Render(renderer, mappingBounds);
			if (clipRegion != null)
				renderer.SetClipping(clipRegion);
			if ((backImage == null) || (!backImage.Render(renderer, mappingBounds)))
				renderer.FillRectangle(mappingBounds, ActualBackColor, ActualFillStyle);
			if (clipRegion != null)
				renderer.RestoreClipping();
		}
		internal void RenderBorder(IRenderer renderer, Rectangle bounds) {
			RenderBorder(renderer, new XYDiagramPaneArea(Diagram.RaggedGeometry, Diagram.WavedGeometry, bounds));
		}
		internal void RenderBorder(IRenderer renderer, XYDiagramPaneArea paneArea) {
			RenderInnerBorder(renderer, paneArea);
			RenderOuterBorder(renderer, paneArea);
		}
		internal void UpdateAnnotationRepository() {
			annotations.UpdateAnnotationRepository();
		}
		internal void ClearAnnotations() {
			Annotations.ClearAnnotations();
		}
		internal void UpdateAxisIntervalBoundsCache(Axis2D axis) {
			AxisIntervalsLayoutRepository repository = new AxisIntervalsLayoutRepository(this, LastMappingBounds.Value);
			repository.GetIntervalsLayout(axis);
		}
		internal void DrawZoomRectangleHorizontal(Graphics gr, Rectangle rect) {
			Rectangle mappingBounds = lastMappingBounds ?? rect;
			rect.Location = new Point(mappingBounds.Left, rect.Location.Y);
			rect.Width = mappingBounds.Width;
			zoomRectangle.FillZoomRectangle(gr, rect);
			if (zoomRectangle.BorderVisible) {
				rect = zoomRectangle.DecreaseBorderRect(rect);
				using (Pen pen = zoomRectangle.CreateBorderPen()) {
					gr.DrawLine(pen, new Point(rect.Left, rect.Top), new Point(rect.Right, rect.Top));
					gr.DrawLine(pen, new Point(rect.Left, rect.Bottom), new Point(rect.Right, rect.Bottom));
				}
			}
		}
		internal void DrawZoomRectangleVertical(Graphics gr, Rectangle rect) {
			Rectangle mappingBounds = lastMappingBounds ?? rect;
			rect.Location = new Point(rect.Location.X, mappingBounds.Top);
			rect.Height = mappingBounds.Height;
			zoomRectangle.FillZoomRectangle(gr, rect);
			if (zoomRectangle.BorderVisible) {
				rect = zoomRectangle.DecreaseBorderRect(rect);
				using (Pen pen = zoomRectangle.CreateBorderPen()) {
					gr.DrawLine(pen, new Point(rect.Left, rect.Top), new Point(rect.Left, rect.Bottom));
					gr.DrawLine(pen, new Point(rect.Right, rect.Top), new Point(rect.Right, rect.Bottom));
				}
			}
		}
		internal void DrawZoomRectangle(Graphics gr, Rectangle rect) {
			zoomRectangle.FillZoomRectangle(gr, rect);
			if (zoomRectangle.BorderVisible) {
				rect = zoomRectangle.DecreaseBorderRect(rect);
				using (Pen pen = zoomRectangle.CreateBorderPen())
					gr.DrawRectangle(pen, rect);
			}
		}
		public void ResetZoom() {
			PaneAxesContainer paneAxesData = Diagram.GetPaneAxesData(this);
			if (paneAxesData != null)
				paneAxesData.ResetZoom();
			Diagram.UpdateAutomaticMeasurement(this);
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			XYDiagramPaneBase pane = obj as XYDiagramPaneBase;
			if (pane == null)
				return;
			weight = pane.weight;
			sizeMode = pane.sizeMode;
			sizeInPixels = pane.sizeInPixels;
			borderColor = pane.borderColor;
			borderVisible = pane.borderVisible;
			backColor = pane.backColor;
			backImage.Assign(pane.backImage);
			fillStyle.Assign(pane.fillStyle);
			zoomRectangle.Assign(pane.zoomRectangle);
			shadow.Assign(pane.shadow);
			scrollBarOptions.Assign(pane.scrollBarOptions);
			visible = pane.visible;
			if (Diagram == null || Diagram.Chart == null || Diagram.Chart.SupportScrollingAndZooming) {
				enableAxisXScrolling = pane.enableAxisXScrolling;
				enableAxisYScrolling = pane.enableAxisYScrolling;
				enableAxisXZooming = pane.enableAxisXZooming;
				enableAxisYZooming = pane.enableAxisYZooming;
			} else {
				enableAxisXScrolling = DefaultBoolean.False;
				enableAxisYScrolling = DefaultBoolean.False;
				enableAxisXZooming = DefaultBoolean.False;
				enableAxisYZooming = DefaultBoolean.False;
			}
		}
	}
	[
	TypeConverter(typeof(XYDiagramDefaultPaneTypeConverter)),
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public sealed class XYDiagramDefaultPane : XYDiagramPaneBase {
		static readonly string name = ChartLocalizer.GetString(ChartStringId.DefaultPaneName);
		protected override int PaneIndex { get { return -1; } }
		protected internal override int ActualPaneID { get { return -1; } }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		NonTestableProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public new string Name {
			get { return base.Name; }
			set { }
		}
		internal XYDiagramDefaultPane(XYDiagram2D diagram)
			: base(name, diagram) {
		}
		protected override ChartElement CreateObjectForClone() {
			return new XYDiagramDefaultPane(null);
		}
		public override string ToString() {
			return "(" + name + ")";
		}
	}
	[
	TypeConverter(typeof(XYDiagramPaneTypeConverter)),
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class XYDiagramPane : XYDiagramPaneBase, ISupportInitialize, IXtraSerializable, IHitTest, ISupportID {
		bool loading;
		int id = -1;
		protected override bool AllowEmptyName { get { return false; } }
		protected override string EmptyNameExceptionText { get { return ChartLocalizer.GetString(ChartStringId.MsgEmptyPaneName); } }
		protected override int PaneIndex {
			get {
				XYDiagram2D diagram = Diagram;
				return diagram == null ? -1 : diagram.Panes.IndexOf(this);
			}
		}
		protected internal override bool Loading { get { return loading || base.Loading; } }
		protected internal override int ActualPaneID { get { return PaneID; } }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty
		]
		public int PaneID {
			get { return id; }
			set {
				if (!Loading)
					throw new NotSupportedException(ChartLocalizer.GetString(ChartStringId.MsgInternalPropertyChangeError));
				id = value;
			}
		}
		public XYDiagramPane()
			: base("", null) {
		}
		public XYDiagramPane(string name)
			: base(name, null) {
			CheckName();
		}
		#region ISupportInitialize implementation
		void ISupportInitialize.BeginInit() {
			this.loading = true;
		}
		void ISupportInitialize.EndInit() {
			this.loading = false;
		}
		#endregion
		#region IHitTest implementation
		HitTestState hitTestState = new HitTestState();
		object IHitTest.Object { get { return this; } }
		HitTestState IHitTest.State { get { return hitTestState; } }
		#endregion
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
		#region XtraSerializing
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
		protected override ChartElement CreateObjectForClone() {
			return new XYDiagramPane();
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			XYDiagramPane pane = obj as XYDiagramPane;
			if (pane != null)
				id = pane.id;
		}
	}
	[
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartCollectionSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class XYDiagramPaneCollection : ChartElementNamedCollection {
		protected override string NamePrefix { get { return ChartLocalizer.GetString(ChartStringId.XYDiagramPanePrefix); } }
		public XYDiagramPane this[int index] { get { return (XYDiagramPane)List[index]; } }
		internal XYDiagramPaneCollection(XYDiagram2D diagram)
			: base(diagram) {
		}
		internal int AddPaneWithoutChanged(XYDiagramPane pane) {
			return base.AddWithoutChanged(pane);
		}
		internal int IndexOf(XYDiagramPane pane) {
			return base.IndexOf(pane);
		}
		protected override void OnClear() {
			foreach (XYDiagramPane pane in this)
				pane.ClearAnnotations();
			base.OnClear();
		}
		protected override void OnRemove(int index, object value) {
			XYDiagramPane pane = value as XYDiagramPane;
			if (pane != null)
				pane.ClearAnnotations();
			base.OnRemove(index, value);
		}
		protected override void ChangeOwnerForItem(ChartElement item) {
			base.ChangeOwnerForItem(item);
			XYDiagramPane pane = item as XYDiagramPane;
			if (pane != null)
				pane.UpdateAnnotationRepository();
		}
		public int Add(XYDiagramPane pane) {
			return base.Add(pane);
		}
		public void AddRange(XYDiagramPane[] coll) {
			base.AddRange(coll);
		}
		public void Insert(int index, XYDiagramPane pane) {
			base.Insert(index, pane);
		}
		public void Remove(XYDiagramPane pane) {
			base.Remove(pane);
		}
		public bool Contains(XYDiagramPane pane) {
			return base.Contains(pane);
		}
		public XYDiagramPane GetPaneByName(string name) {
			return (XYDiagramPane)base.GetElementByName(name);
		}
		public new XYDiagramPane[] ToArray() {
			return (XYDiagramPane[])InnerList.ToArray(typeof(XYDiagramPane));
		}
	}
	[DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")]
	public class ZoomRectangle : ChartElement {
		const bool DefaultBorderVisible = true;
		bool borderVisible = DefaultBorderVisible;
		Color color;
		Color borderColor;
		LineStyle borderLineStyle;
		XYDiagramPaneBase Pane { get { return (XYDiagramPaneBase)Owner; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("ZoomRectangleColor"),
#endif
		TypeConverter(typeof(ColorConverter)),
		XtraSerializableProperty
		]
		public Color Color {
			get { return color; }
			set {
				if (color != value) {
					SendNotification(new ElementWillChangeNotification(this));
					color = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("ZoomRectangleBorderColor"),
#endif
		TypeConverter(typeof(ColorConverter)),
		XtraSerializableProperty
		]
		public Color BorderColor {
			get { return borderColor; }
			set {
				if (borderColor != value) {
					SendNotification(new ElementWillChangeNotification(this));
					borderColor = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("ZoomRectangleBorderLineStyle"),
#endif
		TypeConverter(typeof(ExpandableObjectConverter)),
		NestedTagProperty,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content)
		]
		public LineStyle BorderLineStyle { get { return borderLineStyle; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("ZoomRectangleBorderVisible"),
#endif
		TypeConverter(typeof(BooleanTypeConverter)),
		XtraSerializableProperty
		]
		public bool BorderVisible {
			get { return borderVisible; }
			set {
				if (borderVisible != value) {
					SendNotification(new ElementWillChangeNotification(this));
					borderVisible = value;
					RaiseControlChanged();
				}
			}
		}
		[Browsable(false)]
		public Color ActualColor { get { return Color.IsEmpty ? Pane.Diagram.Appearance.ZoomRectangleColor : Color; } }
		[Browsable(false)]
		public Color ActualBorderColor { get { return BorderColor.IsEmpty ? Pane.Diagram.Appearance.ZoomRectangleBorderColor : BorderColor; } }
		internal ZoomRectangle(ChartElement owner)
			: base(owner) {
			borderLineStyle = new LineStyle(this);
		}
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			switch (propertyName) {
				case "Color":
					return ShouldSerializeColor();
				case "BorderColor":
					return ShouldSerializeBorderColor();
				case "BorderLineStyle":
					return ShouldSerializeBorderLineStyle();
				case "BorderVisible":
					return ShouldSerializeBorderVisible();
				default:
					return base.XtraShouldSerialize(propertyName);
			}
		}
		#endregion
		#region ShouldSerialize & Reset
		bool ShouldSerializeColor() {
			return !color.IsEmpty;
		}
		void ResetColor() {
			Color = Color.Empty;
		}
		bool ShouldSerializeBorderColor() {
			return !borderColor.IsEmpty;
		}
		void ResetBorderColor() {
			BorderColor = Color.Empty;
		}
		bool ShouldSerializeBorderLineStyle() {
			return borderLineStyle.ShouldSerialize();
		}
		bool ShouldSerializeBorderVisible() {
			return borderVisible != DefaultBorderVisible;
		}
		void ResetBorderVisible() {
			BorderVisible = DefaultBorderVisible;
		}
		protected internal override bool ShouldSerialize() {
			return base.ShouldSerialize() || ShouldSerializeBorderColor() || ShouldSerializeColor()
				|| ShouldSerializeBorderLineStyle() || ShouldSerializeBorderVisible();
		}
		#endregion
		protected override ChartElement CreateObjectForClone() {
			return new ZoomRectangle(null);
		}
		internal Pen CreateBorderPen() {
			Pen pen = new Pen(ActualBorderColor, BorderLineStyle.Thickness);
			DashStyleHelper.ApplyDashStyle(pen, BorderLineStyle.DashStyle);
			return pen;
		}
		internal Rectangle DecreaseBorderRect(Rectangle rect) {
			int decrease = BorderLineStyle.Thickness / 2;
			if (decrease > 0)
				rect.Inflate(-decrease, -decrease);
			return rect;
		}
		internal void FillZoomRectangle(Graphics gr, Rectangle rect) {
			using (Brush brush = new SolidBrush(ActualColor))
				gr.FillRectangle(brush, rect);
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			ZoomRectangle rect = obj as ZoomRectangle;
			if (rect != null) {
				color = rect.color;
				borderColor = rect.borderColor;
				borderLineStyle.Assign(rect.borderLineStyle);
				borderVisible = rect.borderVisible;
			}
		}
	}
}
