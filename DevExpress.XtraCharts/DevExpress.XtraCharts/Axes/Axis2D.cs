﻿#region Copyright (c) 2000-2015 Developer Express Inc.
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
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Design;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.Charts.Native;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Localization;
namespace DevExpress.XtraCharts {
	[ResourceFinder(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile)]
	public enum AxisAlignment {
		Near,
		Far,
		Zero
	}
	[ResourceFinder(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile)]
	public enum AxisLabelVisibilityMode {
		Default,
		AutoGeneratedAndCustom
	}
	public abstract class Axis2D : AxisBase, IXtraSupportDeserializeCollectionItem, ICrosshairAxis, IResolveLabelsOverlappingAxis, IIntervalContainer, ISupportVisibilityControlElement {
		const int DefaultThickness = 1;
		const bool DefaultVisible = true;
		const DefaultBoolean DefaultCrosshairLabelVisibility = DefaultBoolean.Default;
		const DefaultBoolean DefaultVisibility = DefaultBoolean.Default;
		static readonly Color DefaultColor = Color.Empty;
		static NearTextPosition Opposite(NearTextPosition position) {
			switch (position) {
				case NearTextPosition.Left:
					return NearTextPosition.Right;
				case NearTextPosition.Right:
					return NearTextPosition.Left;
				case NearTextPosition.Bottom:
					return NearTextPosition.Top;
				case NearTextPosition.Top:
					return NearTextPosition.Bottom;
				default:
					ChartDebug.Fail("Unknown near text position");
					return NearTextPosition.Left;
			}
		}
		readonly RectangleFillStyle interlacedFillStyle;
		readonly Tickmarks tickmarks;
		readonly AxisTitle title;
		readonly StripCollection strips;
		readonly ConstantLineCollection constantLines;
		readonly CustomAxisLabelCollection customLabels;
		readonly VisibleInPanesBehavior visibleInPanesBehavior;
		readonly AxisIntervalLayoutCache intervalBoundsCache;
		readonly Dictionary<IPane, Rectangle> labelBounds;
		bool automaticVisibility = true;
		int thickness = DefaultThickness;
		AxisAlignment alignment;
		Color color = DefaultColor;
		IList<AxisInterval> intervals;
		CrosshairAxisLabelOptions crosshairAxisLabelOptions;
		DefaultBoolean visibility = DefaultVisibility;
		AxisLabelVisibilityMode labelVisibilityMode = AxisLabelVisibilityMode.Default;
		XYDiagramAppearance DiagramAppearance { get { return CommonUtils.GetActualAppearance(this).XYDiagramAppearance; } }
		internal bool AutomaticVisibility { get { return ChartContainer != null && !ChartContainer.Chart.AutoLayout ? DefaultVisible : automaticVisibility; } }
		internal Color ActualColor {
			get { return GraphicUtils.GetColor(color == Color.Empty ? DiagramAppearance.AxisColor : color, ((IHitTest)this).State); }
		}
		internal Color ActualInterlacedColor {
			get {
				Color interlacedColor = InterlacedColor;
				return interlacedColor.IsEmpty ? DiagramAppearance.InterlacedColor : interlacedColor;
			}
		}
		internal RectangleFillStyle ActualInterlacedFillStyle {
			get {
				return interlacedFillStyle.FillMode == FillMode.Empty ? (RectangleFillStyle)DiagramAppearance.InterlacedFillStyle : interlacedFillStyle;
			}
		}
		internal XYDiagram2D XYDiagram2D { get { return (XYDiagram2D)Owner; } }
		internal AxisIntervalLayoutCache IntervalBoundsCache { get { return intervalBoundsCache; } }
		internal Dictionary<IPane, Rectangle> LabelBounds { get { return labelBounds; } }
		internal Rectangle MaxLabelRectCache { get; set; }
		internal IList<AxisInterval> Intervals {
			get {
				if (intervals == null || intervals.Count == 0) {
					ChartDebug.Fail("At least one axis interval should be defined.");
					intervals = new AxisInterval[] { new AxisInterval((IMinMaxValues)VisualRangeData, XYDiagram2D.IsScrollingEnabled ? (IMinMaxValues)WholeRangeData : (IMinMaxValues)VisualRangeData) };
				}
				return intervals;
			}
		}
		internal AxisLabelResolveOverlappingCache OverlappingCache { get; set; }
		internal GRealRect2D Bounds { get; set; }
		protected abstract AxisAlignment DefaultAlignment { get; }
		protected abstract ChartElementVisibilityPriority Priority { get; }
		protected override IEnumerable<IStrip> StripsEnumeration { get { return strips; } }
		protected override IEnumerable<IConstantLine> ConstantLinesEnumeration { get { return constantLines; } }
		protected override IEnumerable<ICustomAxisLabel> CustomLabelsEnumeration { get { return customLabels; } }
		protected override AxisVisibilityInPanes ActualVisibilityInPanes { get { return visibleInPanesBehavior; } }
		protected override bool CanShowCustomWithAutoLabels { get { return labelVisibilityMode == AxisLabelVisibilityMode.AutoGeneratedAndCustom; } }
		protected internal bool ActualVisibility { get { return DefaultBooleanUtils.ToBoolean(Visibility, AutomaticVisibility); } }
		protected internal virtual int IntervalsDistance { get { return 0; } }
		protected internal virtual ScaleBreakOptions ActualScaleBreakOptions { get { return null; } }
		protected internal virtual int ActualAxisID { get { return -1; } }
		protected internal virtual bool ShouldFilterZeroAlignment { get { return false; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("Axis2DInterlacedFillStyle"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.Axis2D.InterlacedFillStyle"),
		Category(Categories.Appearance),
		TypeConverter(typeof(ExpandableObjectConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NestedTagProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Content)
		]
		public RectangleFillStyle InterlacedFillStyle { get { return interlacedFillStyle; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("Axis2DTickmarks"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.Axis2D.Tickmarks"),
		Category(Categories.Elements),
		TypeConverter(typeof(ExpandableObjectConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NestedTagProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Content)
		]
		public Tickmarks Tickmarks { get { return tickmarks; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("Axis2DTitle"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.Axis2D.Title"),
		Category(Categories.Elements),
		TypeConverter(typeof(AxisTitleTypeConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content)
		]
		public AxisTitle Title { get { return title; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("Axis2DStrips"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.Axis2D.Strips"),
		TypeConverter(typeof(CollectionTypeConverter)),
		Category(Categories.Elements),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		Editor("DevExpress.XtraCharts.Design.StripEditor," + AssemblyInfo.SRAssemblyChartsWizard, typeof(UITypeEditor)),
		NestedTagProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Collection, true)
		]
		public StripCollection Strips { get { return strips; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("Axis2DConstantLines"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.Axis2D.ConstantLines"),
		TypeConverter(typeof(CollectionTypeConverter)),
		Category(Categories.Elements),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		Editor("DevExpress.XtraCharts.Design.ConstantLineEditor," + AssemblyInfo.SRAssemblyChartsWizard, typeof(UITypeEditor)),
		NestedTagProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Collection, true)
		]
		public ConstantLineCollection ConstantLines { get { return constantLines; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("Axis2DCustomLabels"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.Axis2D.CustomLabels"),
		TypeConverter(typeof(CollectionTypeConverter)),
		Category(Categories.Elements),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		Editor("DevExpress.XtraCharts.Design.CustomAxisLabelEditor," + AssemblyInfo.SRAssemblyChartsWizard, typeof(UITypeEditor)),
		NestedTagProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Collection, true)
		]
		public CustomAxisLabelCollection CustomLabels { get { return customLabels; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("Axis2DLabel"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.Axis2D.Label"),
		Category(Categories.Elements),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NestedTagProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Content)
		]
		public new AxisLabel Label { get { return ActualLabel; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("Axis2DLabelVisibilityMode"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.Axis2D.LabelVisibilityMode"),
		TypeConverter(typeof(EnumTypeConverter)),
		Category(Categories.Elements),
		XtraSerializableProperty
		]
		public AxisLabelVisibilityMode LabelVisibilityMode {
			get { return labelVisibilityMode; }
			set {
				if (labelVisibilityMode != value) {
					SendNotification(new ElementWillChangeNotification(this));
					labelVisibilityMode = value;
					RaiseControlChanged();
				}
			}
		}
		[
		Obsolete("This property is obsolete now. Use the Visibility property instead."),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty
		]
		public bool Visible {
			get { return ActualVisibility; }
			set { Visibility = DefaultBooleanUtils.ToDefaultBoolean(value); }
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("Axis2DVisibility"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.Axis2D.Visibility"),
		Category(Categories.Behavior),
		TypeConverter(typeof(DefaultBooleanConverter)),
		XtraSerializableProperty
		]
		public DefaultBoolean Visibility {
			get { return visibility; }
			set {
				if (visibility != value) {
					SendNotification(new ElementWillChangeNotification(this));
					visibility = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("Axis2DAlignment"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.Axis2D.Alignment"),
		Category(Categories.Behavior),
		TypeConverter(typeof(AxisAlignmentTypeConverter)),
		Localizable(true),
		XtraSerializableProperty
		]
		public AxisAlignment Alignment {
			get { return alignment; }
			set {
				if (value == AxisAlignment.Zero && ShouldFilterZeroAlignment)
					throw new InvalidAxisAlignmentException(ChartLocalizer.GetString(ChartStringId.MsgInvalidZeroAxisAlignment));
				if (value != alignment) {
					SendNotification(new ElementWillChangeNotification(this));
					alignment = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("Axis2DThickness"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.Axis2D.Thickness"),
		Category(Categories.Appearance),
		XtraSerializableProperty
		]
		public int Thickness {
			get { return thickness; }
			set {
				if (value <= 0)
					throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectAxisThickness));
				if (value != thickness) {
					SendNotification(new ElementWillChangeNotification(this));
					thickness = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("Axis2DColor"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.Axis2D.Color"),
		Category(Categories.Appearance),
		XtraSerializableProperty
		]
		public Color Color {
			get { return color; }
			set {
				if (value != color) {
					SendNotification(new ElementWillChangeNotification(this));
					color = value;
					RaiseControlChanged();
				}
			}
		}
		[
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.Axis2D.VisibilityInPanes"),
		Category(Categories.Behavior),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		NonTestableProperty,
		Editor("DevExpress.XtraCharts.Design.AxisVisibilityInPanesEditor," + AssemblyInfo.SRAssemblyChartsWizard, typeof(UITypeEditor)),
		]
		public IDictionary VisibilityInPanes {
			get {
				if (ChartContainer == null || !ChartContainer.DesignMode)
					throw new MemberAccessException(ChartLocalizer.GetString(ChartStringId.MsgDesignTimeOnlySetting));
				return visibleInPanesBehavior.Visibility;
			}
		}
		[
		Obsolete("This property is now obsolete. Use the CrosshairAxisLabelOptions.Visibility property instead."),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public DefaultBoolean CrosshairLabelVisibility {
			get {
				return crosshairAxisLabelOptions.Visibility;
			}
			set {
				if (value != crosshairAxisLabelOptions.Visibility) {
					SendNotification(new ElementWillChangeNotification(this));
					crosshairAxisLabelOptions.Visibility = value;
					RaiseControlChanged();
				}
			}
		}
		[
		Obsolete("This property is now obsolete. Use the CrosshairAxisLabelOptions.Pattern property instead."),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public string CrosshairLabelPattern {
			get {
				return crosshairAxisLabelOptions.Pattern;
			}
			set {
				if (value != crosshairAxisLabelOptions.Pattern) {
					SendNotification(new ElementWillChangeNotification(this));
					crosshairAxisLabelOptions.Pattern = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("Axis2DCrosshairAxisLabelOptions"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.Axis2D.CrosshairAxisLabelOptions"),
		Category(Categories.Behavior),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NestedTagProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Content)
		]
		public CrosshairAxisLabelOptions CrosshairAxisLabelOptions { get { return crosshairAxisLabelOptions; } }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		NonTestableProperty,
		XtraSerializableProperty
		]
		public string VisibleInPanesSerializable {
			get { return visibleInPanesBehavior.VisibleInPanesSerializable; }
			set { visibleInPanesBehavior.VisibleInPanesSerializable = value; }
		}
		protected Axis2D(string name, XYDiagram2D diagram)
			: base(name, diagram) {
			interlacedFillStyle = new RectangleFillStyle(this);
			tickmarks = CreateTickmarks();
			title = CreateAxisTitle();
			strips = new StripCollection(this);
			constantLines = new ConstantLineCollection(this);
			customLabels = new CustomAxisLabelCollection(this);
			visibleInPanesBehavior = new VisibleInPanesBehavior(this);
			intervalBoundsCache = new AxisIntervalLayoutCache();
			labelBounds = new Dictionary<IPane, Rectangle>();
			alignment = DefaultAlignment;
			crosshairAxisLabelOptions = new CrosshairAxisLabelOptions(this);
		}
		#region ShouldSerialize & Reset
		bool ShouldSerializeInterlacedFillStyle() {
			return interlacedFillStyle.ShouldSerialize();
		}
		bool ShouldSerializeTickmarks() {
			return tickmarks.ShouldSerialize();
		}
		bool ShouldSerializeTitle() {
			return title.ShouldSerialize();
		}
		bool ShouldserializeLabel() {
			return Label.ShouldSerialize();
		}
		bool ShouldSerializeVisible() {
			return false;
		}
		bool ShouldSerializeVisibility() {
			return visibility != DefaultVisibility;
		}
		void ResetVisibility() {
			Visibility = DefaultVisibility;
		}
		bool ShouldSerializeAlignment() {
			return alignment != DefaultAlignment;
		}
		void ResetAlignment() {
			Alignment = DefaultAlignment;
		}
		bool ShouldSerializeThickness() {
			return thickness != DefaultThickness;
		}
		void ResetThickness() {
			Thickness = DefaultThickness;
		}
		bool ShouldSerializeColor() {
			return color != DefaultColor;
		}
		void ResetColor() {
			Color = DefaultColor;
		}
		bool ShouldSerializeVisibleInPanesSerializable() {
			return true;
		}
		bool ShouldSerializeCrosshairAxisLabelOptions() {
			return crosshairAxisLabelOptions.ShouldSerialize();
		}
		bool ShouldSerializeLabelVisibilityMode() {
			return labelVisibilityMode != AxisLabelVisibilityMode.Default;
		}
		void ResetLabelVisibilityMode() {
			LabelVisibilityMode = AxisLabelVisibilityMode.Default;
		}
		protected internal override bool ShouldSerialize() {
			return true;
		}
		#endregion
		#region XtraSerializing
		void IXtraSupportDeserializeCollectionItem.SetIndexCollectionItem(string propertyName, XtraSetItemIndexEventArgs e) {
			XtraSetIndexCollectionItem(propertyName, e.Item.Value);
		}
		object IXtraSupportDeserializeCollectionItem.CreateCollectionItem(string propertyName, XtraItemEventArgs e) {
			return XtraCreateCollectionItem(propertyName);
		}
		protected virtual void XtraSetIndexCollectionItem(string propertyName, object item) {
			switch (propertyName) {
				case "Strips":
					strips.Add((Strip)item);
					break;
				case "ConstantLines":
					constantLines.Add((ConstantLine)item);
					break;
				case "CustomLabels":
					customLabels.Add((CustomAxisLabel)item);
					break;
			}
		}
		protected virtual object XtraCreateCollectionItem(string propertyName) {
			switch (propertyName) {
				case "Strips":
					return new Strip();
				case "ConstantLines":
					return new ConstantLine();
				case "CustomLabels":
					return new CustomAxisLabel();
				default:
					return null;
			}
		}
		protected override bool XtraShouldSerialize(string propertyName) {
			switch (propertyName) {
				case "InterlacedFillStyle":
					return ShouldSerializeInterlacedFillStyle();
				case "Tickmarks":
					return ShouldSerializeTickmarks();
				case "Title":
					return ShouldSerializeTitle();
				case "Label":
					return ShouldserializeLabel();
				case "Visible":
					return ShouldSerializeVisible();
				case "Alignment":
					return ShouldSerializeAlignment();
				case "Thickness":
					return ShouldSerializeThickness();
				case "Color":
					return ShouldSerializeColor();
				case "VisibleInPanesSerializable":
					return ShouldSerializeVisibleInPanesSerializable();
				case "CrosshairAxisLabelOptions":
					return ShouldSerializeCrosshairAxisLabelOptions();
				case "Visibility":
					return ShouldSerializeVisibility();
				case "LabelVisibilityMode":
					return ShouldSerializeLabelVisibilityMode();
				default:
					return base.XtraShouldSerialize(propertyName);
			}
		}
		#endregion
		#region ICrosshairAxis
		string ICrosshairAxis.LabelPattern { get { return CrosshairAxisLabelOptions.ActualPattern; } }
		bool ICrosshairAxis.LabelVisible { get { return CrosshairAxisLabelOptions.ActualVisibility; } }
		#endregion
		#region IPatternHolder
		PatternDataProvider IPatternHolder.GetDataProvider(PatternConstants patternConstant) { return new AxisPatternDataProvider(); }
		string IPatternHolder.PointPattern { get { return CrosshairAxisLabelOptions.ActualPattern; } }
		#endregion
		#region IResolveLabelsOverlappingAxis
		AxisLabelResolveOverlappingCache IResolveLabelsOverlappingAxis.OverlappingCache {
			get { return OverlappingCache; }
			set { OverlappingCache = value; }
		}
		#endregion
		#region IIntervalContainer
		IList<AxisInterval> IIntervalContainer.Intervals { get { return this.intervals; } }
		int IIntervalContainer.IntervalsDistance { get { return this.IntervalsDistance; } }
		#endregion
		#region ISupportVisibilityControlElement
		int ISupportVisibilityControlElement.Priority { get { return (int)Priority; } }
		bool ISupportVisibilityControlElement.Visible {
			get { return automaticVisibility; }
			set { automaticVisibility = value; }
		}
		GRealRect2D ISupportVisibilityControlElement.Bounds { get { return Bounds; } }
		VisibilityElementOrientation ISupportVisibilityControlElement.Orientation {
			get { return IsVertical ? VisibilityElementOrientation.Vertical : VisibilityElementOrientation.Horizontal; }
		}
		#endregion
		protected abstract Tickmarks CreateTickmarks();
		protected abstract AxisTitle CreateAxisTitle();
		protected virtual IList<AxisInterval> CreateIntervals(double min, double max) {
			return null;
		}
		protected virtual IList<IMinMaxValues> CreateIntervalLimits(double min, double max) {
			return null;
		}
		protected override void Dispose(bool disposing) {
			if (disposing) {
				interlacedFillStyle.Dispose();
				tickmarks.Dispose();
				title.Dispose();
				strips.Dispose();
				constantLines.Dispose();
				customLabels.Dispose();
				labelBounds.Clear();
			}
			base.Dispose(disposing);
		}
		protected override IList<IMinMaxValues> CalculateRangeLimitsList(double min, double max) {
			return XYDiagram2D.IsScrollingEnabled ? null : CreateIntervalLimits(min, max);
		}
		protected override GRealRect2D GetLabelBounds(IPane pane) {
			if (labelBounds.ContainsKey(pane)) {
				Rectangle rect = labelBounds[pane];
				return new GRealRect2D(rect.X, rect.Y, rect.Width, rect.Height);
			}
			return GRealRect2D.Empty;
		}
		protected internal virtual bool Contains(object obj) {
			ConstantLine constantLine = obj as ConstantLine;
			return constantLine != null && constantLines.Contains(constantLine);
		}
		internal NearTextPosition GetNearTextPosition() {
			NearTextPosition position = IsVertical ? NearTextPosition.Left : NearTextPosition.Bottom;
			return alignment == AxisAlignment.Far ? Opposite(position) : position;
		}
		internal NearTextPosition GetConstantLineTitleNearTextPosition(bool placeUnderLine) {
			NearTextPosition position;
			if (IsVertical)
				position = NearTextPosition.Top;
			else
				position = ActualReverse ? NearTextPosition.Left : NearTextPosition.Right;
			return placeUnderLine ? Opposite(position) : position;
		}
		internal int GetAxisTitleActualAngle() {
			if (IsVertical)
				return alignment == AxisAlignment.Far ? 90 : -90;
			return 0;
		}
		internal int GetConstantLineTitleActualAngle() {
			if (IsVertical)
				return 0;
			return ActualReverse ? -90 : 90;
		}
		internal void UpdateVisibilityInPanes(IList<IPane> newPanes) {
			visibleInPanesBehavior.UpdateVisibilityInPanes(newPanes);
		}
		internal bool VisibleInPane(XYDiagramPaneBase pane) {
			bool visible;
			return visibleInPanesBehavior.Visibility.TryGetValue(pane, out visible) && visible;
		}
		internal void UpdateIntervals() {
			IMinMaxValues visualMinMax = GetVisualMinMax();
			IMinMaxValues wholeMinMax = GetWholeMinMax();
			if (XYDiagram2D.IsScrollingEnabled)
				intervals = new AxisInterval[] { new AxisInterval(visualMinMax, wholeMinMax) };
			else {
				intervals = CreateIntervals(visualMinMax.Min, visualMinMax.Max);
				if (intervals == null)
					intervals = new AxisInterval[] { new AxisInterval(visualMinMax, visualMinMax) };
			}
		}
		internal void UpdateAxisValueContainers() {
			IAxisElementContainer axis = this;
			if (axis.ScaleBreaks != null)
				foreach (IScaleBreak scaleBreak in axis.ScaleBreaks) {
					AxisValueContainerHelper.UpdateAxisValue(scaleBreak.Edge1, ScaleTypeMap);
					AxisValueContainerHelper.UpdateAxisValue(scaleBreak.Edge2, ScaleTypeMap);
					AxisValueContainerHelper.UpdateAxisValueContainer(scaleBreak.Edge1, ScaleTypeMap);
					AxisValueContainerHelper.UpdateAxisValueContainer(scaleBreak.Edge2, ScaleTypeMap);
				}
			if (axis.Strips != null)
				foreach (IStrip strip in axis.Strips) {
					AxisValueContainerHelper.UpdateAxisValue(strip.MinLimit, strip.MaxLimit, ScaleTypeMap);
					AxisValueContainerHelper.UpdateAxisValueContainer(strip.MinLimit, ScaleTypeMap);
					AxisValueContainerHelper.UpdateAxisValueContainer(strip.MaxLimit, ScaleTypeMap);
					strip.CorrectLimits();
				}
			if (axis.ConstantLines != null)
				foreach (IConstantLine line in axis.ConstantLines) {
					AxisValueContainerHelper.UpdateAxisValue(line, ScaleTypeMap);
					AxisValueContainerHelper.UpdateAxisValueContainer(line, ScaleTypeMap);
				}
			if (axis.CustomLabels != null)
				foreach (ICustomAxisLabel label in axis.CustomLabels) {
					AxisValueContainerHelper.UpdateAxisValue(label, ScaleTypeMap);
					AxisValueContainerHelper.UpdateAxisValueContainer(label, ScaleTypeMap);
				}
		}
		internal void ResetScrollLabelSizeCache() {
			MaxLabelRectCache = Rectangle.Empty;
		}
		IMinMaxValues GetVisualMinMax() {
			if (double.IsNaN(VisualRangeData.Min) && double.IsNaN(VisualRangeData.Max))
				VisualRangeData.UpdateRange(VisualRangeData.MinValue, VisualRangeData.MaxValue, 0.0, 1.0);
			return new RangeWrapper(VisualRangeData);
		}
		IMinMaxValues GetWholeMinMax() {
			if (double.IsNaN(WholeRangeData.Min) && double.IsNaN(WholeRangeData.Max))
				WholeRangeData.UpdateRange(WholeRangeData.MinValue, WholeRangeData.MaxValue, 0.0, 1.0);
			return new RangeWrapper(WholeRangeData);
		}
		internal double? CalculateInternalValue(XYDiagramPaneBase pane, int coordinate, int length) {
			if (ActualReverse)
				coordinate = length - coordinate;
			if (XYDiagram2D.Chart.CacheToMemory)
				pane.UpdateAxisIntervalBoundsCache(this);
			return IntervalBoundsCache.GetAxisValue(pane, ((IAxisData)this).AxisScaleTypeMap.Transformation, coordinate);
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			Axis2D axis = obj as Axis2D;
			if (axis != null) {
				interlacedFillStyle.Assign(axis.interlacedFillStyle);
				tickmarks.Assign(axis.tickmarks);
				title.Assign(axis.title);
				strips.Assign(axis.strips);
				constantLines.Assign(axis.constantLines);
				customLabels.Assign(axis.customLabels);
				visibleInPanesBehavior.Assign(axis.visibleInPanesBehavior);
				visibility = axis.visibility;
				alignment = axis.alignment;
				thickness = axis.thickness;
				crosshairAxisLabelOptions.Assign(axis.crosshairAxisLabelOptions);
				color = axis.color;
				labelVisibilityMode = axis.labelVisibilityMode;
			}
		}
		public bool GetVisibilityInPane(XYDiagramPaneBase pane) {
			return visibleInPanesBehavior.GetVisibilityInPane(pane);
		}
		public void SetVisibilityInPane(bool visible, XYDiagramPaneBase pane) {
			visibleInPanesBehavior.SetVisibilityInPane(pane, visible);
		}
	}
}
