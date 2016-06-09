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
using System.Windows;
using System.Windows.Media;
using DevExpress.Charts.Native;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Charts.Design;
using DevExpress.Xpf.Charts.Localization;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Charts {
	public abstract class CircularAxisY2D : AxisBase, ILogarithmic, ITickmarksOwner, ILineOwner, ITransformable {
		[Obsolete(ObsoleteMessages.AxisDateTimeMeasureUnitProperty)]
		public static readonly DependencyProperty DateTimeMeasureUnitProperty = DependencyPropertyManager.Register("DateTimeMeasureUnit", typeof(DateTimeMeasurementUnit),
			typeof(CircularAxisY2D), new FrameworkPropertyMetadata(DateTimeMeasurementUnit.Day, FrameworkPropertyMetadataOptions.AffectsRender, DateTimeMeasureUnitPropertyChanged));
		[Obsolete(ObsoleteMessages.AxisDateTimeGridAlignmentProperty)]
		public static readonly DependencyProperty DateTimeGridAlignmentProperty = DependencyPropertyManager.Register("DateTimeGridAlignment", typeof(DateTimeMeasurementUnit),
			typeof(CircularAxisY2D), new FrameworkPropertyMetadata(DateTimeMeasurementUnit.Day, FrameworkPropertyMetadataOptions.AffectsRender, DateTimeGridAlignmentPropertyChanged));
		[Obsolete(ObsoleteMessages.CircularAxisY2DDateTimeOptionsProperty)]
		public static readonly DependencyProperty DateTimeOptionsProperty = DependencyPropertyManager.Register("DateTimeOptions",
			typeof(DateTimeOptions), typeof(CircularAxisY2D), new PropertyMetadata(DateTimeOptionsPropertyChanged));
		[Obsolete(ObsoleteMessages.RangeProperty)]
		public static readonly DependencyProperty RangeProperty;
		public static readonly DependencyProperty WholeRangeProperty = DependencyPropertyManager.Register("WholeRange", typeof(Range), typeof(CircularAxisY2D), new PropertyMetadata(WholeRangePropertyChanged));
		public static readonly DependencyProperty LogarithmicProperty = DependencyPropertyManager.Register("Logarithmic", typeof(bool),
			typeof(CircularAxisY2D), new FrameworkPropertyMetadata(false, LogarithmicPropertyChanged));
		public static readonly DependencyProperty LogarithmicBaseProperty = DependencyPropertyManager.Register("LogarithmicBase", typeof(double),
			typeof(CircularAxisY2D), new FrameworkPropertyMetadata(10.0, LogarithmicPropertyChanged, CoercyLogarithmicBase));
		public static readonly DependencyProperty ThicknessProperty = DependencyPropertyManager.Register("Thickness",
			typeof(int), typeof(CircularAxisY2D), new PropertyMetadata(1, ChartElementHelper.Update), ValidateThickness);
		public static readonly DependencyProperty BrushProperty = DependencyPropertyManager.Register("Brush",
			typeof(Brush), typeof(CircularAxisY2D));
		public static readonly DependencyProperty TickmarksVisibleProperty = DependencyPropertyManager.Register("TickmarksVisible",
			typeof(bool), typeof(CircularAxisY2D), new PropertyMetadata(true, ChartElementHelper.Update));
		public static readonly DependencyProperty TickmarksMinorVisibleProperty = DependencyPropertyManager.Register("TickmarksMinorVisible",
			typeof(bool), typeof(CircularAxisY2D), new PropertyMetadata(true, ChartElementHelper.Update));
		public static readonly DependencyProperty TickmarksThicknessProperty = DependencyPropertyManager.Register("TickmarksThickness",
			typeof(int), typeof(CircularAxisY2D), new PropertyMetadata(1, ChartElementHelper.Update), ValidateTickmarksThickness);
		public static readonly DependencyProperty TickmarksMinorThicknessProperty = DependencyPropertyManager.Register("TickmarksMinorThickness",
			typeof(int), typeof(CircularAxisY2D), new PropertyMetadata(1, ChartElementHelper.Update), ValidateTickmarksThickness);
		public static readonly DependencyProperty TickmarksLengthProperty = DependencyPropertyManager.Register("TickmarksLength",
			typeof(int), typeof(CircularAxisY2D), new PropertyMetadata(5, ChartElementHelper.Update), ValidateTickmarksLength);
		public static readonly DependencyProperty TickmarksMinorLengthProperty = DependencyPropertyManager.Register("TickmarksMinorLength",
			typeof(int), typeof(CircularAxisY2D), new PropertyMetadata(2, ChartElementHelper.Update), ValidateTickmarksLength);
		public static readonly DependencyProperty TickmarksCrossAxisProperty = DependencyPropertyManager.Register("TickmarksCrossAxis",
			typeof(bool), typeof(CircularAxisY2D), new PropertyMetadata(false, ChartElementHelper.Update));
		public static readonly DependencyProperty VisibleProperty = DependencyPropertyManager.Register("Visible", typeof(bool), typeof(CircularAxisY2D), new PropertyMetadata(true, ChartElementHelper.Update));
		public static readonly DependencyProperty AlwaysShowZeroLevelProperty = DependencyPropertyManager.RegisterAttached("AlwaysShowZeroLevel",
			typeof(bool), typeof(CircularAxisY2D), new PropertyMetadata(true, AlwaysShowZeroLevelPropertyChanged));
		public static readonly DependencyProperty DateTimeScaleOptionsProperty = DependencyPropertyManager.Register("DateTimeScaleOptions",
			typeof(ContinuousDateTimeScaleOptions), typeof(CircularAxisY2D), new PropertyMetadata(DateTimeScaleOptionsPropertyChanged));
		public static readonly DependencyProperty NumericScaleOptionsProperty = DependencyPropertyManager.Register("NumericScaleOptions",
			typeof(ContinuousNumericScaleOptions), typeof(CircularAxisY2D), new PropertyMetadata(NumericScaleOptionsPropertyChanged));
		[
		Category(Categories.Behavior)
		]
		public static bool GetAlwaysShowZeroLevel(ChartNonVisualElement range) {
			return (bool)range.GetValue(AlwaysShowZeroLevelProperty);
		}
		public static void SetAlwaysShowZeroLevel(ChartNonVisualElement range, bool value) {
			range.SetValue(AlwaysShowZeroLevelProperty, value);
		}
		protected internal override bool GetActualAlwaysShowZeroLevel(ChartNonVisualElement range) {
			return GetAlwaysShowZeroLevel(range);
		}
		internal static void AlwaysShowZeroLevelPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
#pragma warning disable 0612
			AlwaysShowZeroLevelPropertyChanged_AxisRange(d, e);
#pragma warning restore 0612
			Range range = d as Range;
			if (range != null)
				DevExpress.Xpf.Charts.Range.AlwaysShowZeroLevelPropertyChanged(d, e);
		}
		[Obsolete]
		internal static void AlwaysShowZeroLevelPropertyChanged_AxisRange(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			AxisRange axisRange = d as AxisRange;
			if (axisRange != null)
				AxisRange.AlwaysShowZeroLevelPropertyChanged(d, e);
		}
		static void DateTimeMeasureUnitPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			CircularAxisY2D axis = d as CircularAxisY2D;
			if (axis != null)
				axis.ActualDateTimeMeasureUnit = (DateTimeMeasurementUnit)e.NewValue;
			ChartElementHelper.Update(d, e);
		}
		static void DateTimeGridAlignmentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			CircularAxisY2D axis = d as CircularAxisY2D;
			if (axis != null)
				axis.ActualGridAlignment = (DateTimeMeasurementUnit)e.NewValue;
			ChartElementHelper.Update(d, e);
		}
		static bool ValidateThickness(object thickness) {
			return (int)thickness >= 0;
		}
		static bool ValidateTickmarksThickness(object thickness) {
			return (int)thickness > 0;
		}
		static bool ValidateTickmarksLength(object length) {
			return (int)length > 0;
		}
		static object CoercyLogarithmicBase(DependencyObject d, object value) {
			if ((double)value <= 1)
				throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgInvalidLogarithmicBase));
			return value;
		}
		static void DateTimeOptionsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			CircularAxisY2D axis = d as CircularAxisY2D;
			if (axis != null) {
				axis.dateTimeOptions = e.NewValue as DateTimeOptions;
				CommonUtils.SubscribePropertyChangedWeakEvent(e.OldValue as DateTimeOptions, e.NewValue as DateTimeOptions, axis);
			}
			ChartElementHelper.Update(d, e);
		}
		[Obsolete]
		static void RangePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			CircularAxisY2D axis = d as CircularAxisY2D;
			if (axis != null) {
				AxisRange range = e.NewValue as AxisRange;
				if (range == null) {
					axis.ActualRange = new AxisRange();
					e = new DependencyPropertyChangedEventArgs(e.Property, e.OldValue, axis.ActualRange);
				}
				else {
					range.UpdateMinMaxValues(axis);
					axis.ActualRange = range;
				}
			}
			ChartElementHelper.ChangeOwnerAndUpdate(d, e, ChartElementChange.ClearDiagramCache);
		}
		static void WholeRangePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			CircularAxisY2D axis = d as CircularAxisY2D;
			if (axis != null) {
				Range range = e.NewValue as Range;
				if (range == null) {
					axis.ActualWholeRange = new Range();
					e = new DependencyPropertyChangedEventArgs(e.Property, e.OldValue, axis.ActualWholeRange);
				}
				else {
					range.SetRangeData(axis.WholeRangeData);
					axis.ActualWholeRange = range;
				}
			}
			ChartElementHelper.ChangeOwnerAndUpdate(d, e, ChartElementChange.ClearDiagramCache);
		}
		static void LogarithmicPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			CircularAxisY2D axis = d as CircularAxisY2D;
			if (axis != null)
				axis.ScaleMap.BuildTransformation(axis);
			ChartElementHelper.Update(d, e, ChartElementChange.ClearDiagramCache);
		}
		[
		Obsolete(ObsoleteMessages.AxisDateTimeMeasureUnitProperty),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public DateTimeMeasurementUnit DateTimeMeasureUnit {
			get { return (DateTimeMeasurementUnit)GetValue(DateTimeMeasureUnitProperty); }
			set { SetValue(DateTimeMeasureUnitProperty, value); }
		}
		[
		Obsolete(ObsoleteMessages.AxisDateTimeGridAlignmentProperty),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public DateTimeMeasurementUnit DateTimeGridAlignment {
			get { return (DateTimeMeasurementUnit)GetValue(DateTimeGridAlignmentProperty); }
			set { SetValue(DateTimeGridAlignmentProperty, value); }
		}
		[
		Obsolete(ObsoleteMessages.CircularAxisY2DDateTimeOptionsProperty),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public DateTimeOptions DateTimeOptions {
			get { return (DateTimeOptions)GetValue(DateTimeOptionsProperty); }
			set { SetValue(DateTimeOptionsProperty, value); }
		}
		[
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public bool Logarithmic {
			get { return (bool)GetValue(LogarithmicProperty); }
			set { SetValue(LogarithmicProperty, value); }
		}
		[
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public double LogarithmicBase {
			get { return (double)GetValue(LogarithmicBaseProperty); }
			set { SetValue(LogarithmicBaseProperty, value); }
		}
		[
		TypeConverter(typeof(AxisRangeTypeConverter)),
		Category(Categories.Common),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden, true),
		Obsolete(ObsoleteMessages.RangePropertyCircularAxis),
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public AxisRange Range {
			get { return (AxisRange)GetValue(RangeProperty); }
			set { SetValue(RangeProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("CircularAxisY2DWholeRange"),
#endif
		Category(Categories.Common),
		XtraSerializableProperty(XtraSerializationVisibility.Content, true)
		]
		public Range WholeRange {
			get { return (Range)GetValue(WholeRangeProperty); }
			set { SetValue(WholeRangeProperty, value); }
		}
		[
		Category(Categories.Presentation),
		XtraSerializableProperty
		]
		public int Thickness {
			get { return (int)GetValue(ThicknessProperty); }
			set { SetValue(ThicknessProperty, value); }
		}
		[
		Category(Categories.Brushes),
		XtraSerializableProperty
		]
		public Brush Brush {
			get { return (Brush)GetValue(BrushProperty); }
			set { SetValue(BrushProperty, value); }
		}
		[
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public bool TickmarksVisible {
			get { return (bool)GetValue(TickmarksVisibleProperty); }
			set { SetValue(TickmarksVisibleProperty, value); }
		}
		[
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public bool TickmarksMinorVisible {
			get { return (bool)GetValue(TickmarksMinorVisibleProperty); }
			set { SetValue(TickmarksMinorVisibleProperty, value); }
		}
		[
		Category(Categories.Presentation),
		XtraSerializableProperty
		]
		public int TickmarksThickness {
			get { return (int)GetValue(TickmarksThicknessProperty); }
			set { SetValue(TickmarksThicknessProperty, value); }
		}
		[
		Category(Categories.Presentation),
		XtraSerializableProperty
		]
		public int TickmarksMinorThickness {
			get { return (int)GetValue(TickmarksMinorThicknessProperty); }
			set { SetValue(TickmarksMinorThicknessProperty, value); }
		}
		[
		Category(Categories.Presentation),
		XtraSerializableProperty
		]
		public int TickmarksLength {
			get { return (int)GetValue(TickmarksLengthProperty); }
			set { SetValue(TickmarksLengthProperty, value); }
		}
		[
		Category(Categories.Presentation),
		XtraSerializableProperty
		]
		public int TickmarksMinorLength {
			get { return (int)GetValue(TickmarksMinorLengthProperty); }
			set { SetValue(TickmarksMinorLengthProperty, value); }
		}
		[
		Category(Categories.Presentation),
		XtraSerializableProperty
		]
		public bool TickmarksCrossAxis {
			get { return (bool)GetValue(TickmarksCrossAxisProperty); }
			set { SetValue(TickmarksCrossAxisProperty, value); }
		}
		[
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public bool Visible {
			get { return (bool)GetValue(VisibleProperty); }
			set { SetValue(VisibleProperty, value); }
		}
		[
		Category(Categories.Behavior),
		XtraSerializableProperty(XtraSerializationVisibility.Content, true)
		]
		public ContinuousDateTimeScaleOptions DateTimeScaleOptions {
			get { return (ContinuousDateTimeScaleOptions)GetValue(DateTimeScaleOptionsProperty); }
			set { SetValue(DateTimeScaleOptionsProperty, value); }
		}
		[
		Category(Categories.Behavior),
		XtraSerializableProperty(XtraSerializationVisibility.Content, true)
		]
		public ContinuousNumericScaleOptions NumericScaleOptions {
			get { return (ContinuousNumericScaleOptions)GetValue(NumericScaleOptionsProperty); }
			set { SetValue(NumericScaleOptionsProperty, value); }
		}
		readonly ContinuousDateTimeScaleOptions defaultDateTimeScaleOptionsImpl = new ContinuousDateTimeScaleOptions();
		readonly ContinuousNumericScaleOptions defaultNumericScaleOptionsImpl = new ContinuousNumericScaleOptions();
		protected override DateTimeScaleOptionsBase DefaultDateTimeScaleOptions { get { return defaultDateTimeScaleOptionsImpl; } }
		protected override NumericScaleOptionsBase DefaultNumericScaleOptions { get { return defaultNumericScaleOptionsImpl; } }
		protected internal override DateTimeOptions DateTimeOptionsImpl { get { return dateTimeOptions; } }
		protected internal override bool IsVertical { get { return true; } }
		protected internal override bool IsValuesAxis { get { return true; } }
		protected internal override bool IsReversed { get { return false; } }
		protected override int GridSpacingFactor { get { return 50; } }
		CircularDiagram2D CircularDiagram { get { return ((IChartElement)this).Owner as CircularDiagram2D; } }
		DateTimeOptions dateTimeOptions;
		[Obsolete]
		static CircularAxisY2D() {
			RangeProperty = DependencyPropertyManager.Register("Range", typeof(AxisRange), typeof(CircularAxisY2D), new PropertyMetadata(RangePropertyChanged));
		}
		#region ILogarithmic implementation
		bool ILogarithmic.Enabled { get { return Logarithmic; } }
		double ILogarithmic.Base { get { return LogarithmicBase; } }
		#endregion
		#region ITransformable Members
		Transform ITransformable.GeometryTransform {
			get {
				RotateTransform rotation = new RotateTransform();
				if (CircularDiagram != null) {
					if (CircularDiagram.RotationDirection == CircularDiagramRotationDirection.Clockwise)
						rotation.Angle = CircularDiagram.StartAngle;
					else
						rotation.Angle = -CircularDiagram.StartAngle;
					rotation.CenterX = 0.5 * Math.Max(TickmarksMinorLength, TickmarksLength);
					rotation.CenterY = CircularDiagram.ActualViewport.Height / 4.0;
				}
				ScaleTransform overturn = new ScaleTransform() { ScaleX = -1, ScaleY = -1 };
				TransformGroup transofrm = new TransformGroup();
				transofrm.Children.Add(overturn);
				transofrm.Children.Add(rotation);
				return transofrm;
			}
		}
		#endregion
		protected internal override GridLineGeometry CreateGridLineGeometry(Rect viewport, IAxisMapping mapping, double axisValue, int thickness) {
			if (CircularDiagram == null)
				return new GridLineGeometry(GridLineType.Polyline, new List<Point>());
			if (CircularDiagram.ShapeStyle == CircularDiagramShapeStyle.Circle)
				return CreateCircularGridLineGeometry(viewport, mapping, axisValue, thickness);
			else
				return CreatePolygonalGridLineGeometry(viewport, mapping, axisValue, thickness);
		}
		protected internal override InterlaceGeometry CreateInterlaceGeometry(Rect viewport, IAxisMapping mapping, double nearAxisValue, double farAxisValue) {
			if (CircularDiagram == null)
				return new InterlaceGeometry(InterlaceType.Ellipse, new List<Point>(), new List<Point>(), RectExtensions.Zero);
			if (CircularDiagram.ShapeStyle == CircularDiagramShapeStyle.Circle)
				return CreateCircularInterlaceGeometry(viewport, mapping, nearAxisValue, farAxisValue);
			else
				return CreatePolygonalInterlaceGeometry(viewport, mapping, nearAxisValue, farAxisValue);
		}
		protected internal override IAxisMapping CreateMapping(Rect bounds) {
			return new AxisMappingEx(this, 0.5 * bounds.Height);
		}
		GridLineGeometry CreateCircularGridLineGeometry(Rect viewport, IAxisMapping mapping, double axisValue, int thickness) {
			double position = Render2DHelper.CorrectLinePosition(mapping.GetRoundedClampedAxisValue(axisValue), thickness);
			Point p1 = new Point(0.5 * viewport.Width - position, 0.5 * viewport.Height - position);
			Point p2 = new Point(p1.X + 2 * position, p1.Y + 2 * position);
			return new GridLineGeometry(GridLineType.Ellipse, new List<Point>() { p1, p2 });
		}
		GridLineGeometry CreatePolygonalGridLineGeometry(Rect viewport, IAxisMapping mapping, double axisValue, int thickness) {
			double circumradius = mapping.GetAxisValue(axisValue);
			CircularAxisMapping axisXMapping = CircularDiagram.AxisXImpl.CreateMapping(viewport) as CircularAxisMapping;
			List<Point> points = axisXMapping.GetMeshPoints(circumradius);
			return new GridLineGeometry(GridLineType.Polyline, points);
		}
		InterlaceGeometry CreatePolygonalInterlaceGeometry(Rect viewport, IAxisMapping mapping, double nearAxisValue, double farAxisValue) {
			double near = mapping.GetClampedAxisValue(nearAxisValue);
			double far = mapping.GetClampedAxisValue(farAxisValue);
			double max = Math.Max(near, far);
			double min = Math.Min(near, far);
			Point center = viewport.CalcRelativeToLeftTopCenter();
			CircularAxisMapping axisXMapping = CircularDiagram.AxisXImpl.CreateMapping(viewport) as CircularAxisMapping;
			List<Point> holePoints = min > 0 ? axisXMapping.GetMeshPoints(min) : new List<Point>();
			List<Point> points = axisXMapping.GetMeshPoints(max);
			return new InterlaceGeometry(InterlaceType.Polygon, points, holePoints, new Rect(center.X - max, center.Y - max, 2 * max, 2 * max));
		}
		InterlaceGeometry CreateCircularInterlaceGeometry(Rect axisBounds, IAxisMapping mapping, double nearAxisValue, double farAxisValue) {
			double near = mapping.GetRoundedClampedAxisValue(nearAxisValue);
			double far = mapping.GetRoundedClampedAxisValue(farAxisValue);
			double max = Math.Max(near, far);
			Point center = axisBounds.CalcRelativeToLeftTopCenter();
			Point p1 = new Point(center.X - max, center.Y - max);
			Point p2 = new Point(center.X + max, center.Y + max);
			double min = Math.Min(near, far);
			List<Point> holePoints = min > 0 ? new List<Point>() { new Point(center.X - min, center.Y - min), new Point(center.X + min, center.Y + min) } : new List<Point>();
			return new InterlaceGeometry(InterlaceType.Ellipse, new List<Point>() { p1, p2 }, holePoints, new Rect(p1, p2));
		}
	}
	public sealed class RadarAxisY2D : CircularAxisY2D {
		public RadarAxisY2D() {
			DefaultStyleKey = typeof(RadarAxisY2D);
		}
	}
	public sealed class PolarAxisY2D : CircularAxisY2D {
		public PolarAxisY2D() {
			DefaultStyleKey = typeof(PolarAxisY2D);
		}
	}
}
