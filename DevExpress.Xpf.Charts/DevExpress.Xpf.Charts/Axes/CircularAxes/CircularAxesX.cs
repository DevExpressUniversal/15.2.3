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
using DevExpress.Charts.Native;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Charts.Design;
using DevExpress.Xpf.Charts.Localization;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Charts {
	public abstract class CircularAxisX2D : AxisBase {
		public static readonly DependencyProperty NumericScaleOptionsProperty = DependencyPropertyManager.Register("NumericScaleOptions", typeof(NumericScaleOptionsBase),
			typeof(CircularAxisX2D), new PropertyMetadata(NumericScaleOptionsPropertyChanged));
		[
		Category(Categories.Behavior),
		XtraSerializableProperty(XtraSerializationVisibility.Content, true)
		]
		public NumericScaleOptionsBase NumericScaleOptions {
			get { return (NumericScaleOptionsBase)GetValue(NumericScaleOptionsProperty); }
			set { SetValue(NumericScaleOptionsProperty, value); }
		}
		readonly ContinuousNumericScaleOptions defaultNumericScaleOptions = new ContinuousNumericScaleOptions();
		internal CircularDiagram2D CircularDiagram { get { return ((IChartElement)this).Owner as CircularDiagram2D; } }
		CircularDiagramRotationDirection RotationDirection { get { return CircularDiagram != null ? CircularDiagram.RotationDirection : CircularDiagramRotationDirection.Clockwise; } }
		CircularDiagramShapeStyle ShapeStyle { get { return CircularDiagram != null ? CircularDiagram.ShapeStyle : CircularDiagramShapeStyle.Circle; } }
		protected GridAndTextDataEx GridAndTextData { get { return CircularDiagram != null ? CircularDiagram.GetGridAndTextData(this) : null; } }
		protected override NumericScaleOptionsBase DefaultNumericScaleOptions { get { return defaultNumericScaleOptions; } }
		protected internal override bool IsVertical { get { return false; } }
		protected internal override bool IsValuesAxis { get { return false; } }
		protected internal override bool IsReversed { get { return false; } }
		protected override int GridSpacingFactor { get { return 45; } }
		protected internal override GridLineGeometry CreateGridLineGeometry(Rect axisBounds, IAxisMapping mapping, double axisValue, int thickness) {
			CircularAxisMapping circularMapping = mapping as CircularAxisMapping;
			if (circularMapping != null) {
				Point center = new Point(0.5 * axisBounds.Width, 0.5 * axisBounds.Height);
				Point anchor = circularMapping.GetPointOnCircularAxis(axisValue);
				return new GridLineGeometry(GridLineType.Polyline, new List<Point>() { anchor, center });
			}
			return null;
		}
		protected internal override InterlaceGeometry CreateInterlaceGeometry(Rect axisBounds, IAxisMapping mapping, double nearAxisValue, double farAxisValue) {
			CircularAxisMapping circularMapping = mapping as CircularAxisMapping;
			if (circularMapping != null) {
				Point center = new Point(0.5 * axisBounds.Width, 0.5 * axisBounds.Height);
				Point p1 = circularMapping.GetPointOnCircularAxis(nearAxisValue);
				Point p2 = circularMapping.GetPointOnCircularAxis(farAxisValue);
				InterlaceType type;
				if (ShapeStyle == CircularDiagramShapeStyle.Circle)
					type = RotationDirection == CircularDiagramRotationDirection.Clockwise ? InterlaceType.ClockwisePie : InterlaceType.CounterclockwisePie;
				else
					type = InterlaceType.Polygon;
				return new InterlaceGeometry(type, new List<Point>() { p1, p2, center }, new List<Point>(), new Rect(0, 0, axisBounds.Width, axisBounds.Height));
			}
			return null;
		}
	}
	public sealed class RadarAxisX2D : CircularAxisX2D, ILogarithmic {
		[Obsolete(ObsoleteMessages.AxisDateTimeMeasureUnitProperty)]
		public static readonly DependencyProperty DateTimeMeasureUnitProperty = DependencyPropertyManager.Register("DateTimeMeasureUnit", typeof(DateTimeMeasurementUnit),
			typeof(RadarAxisX2D), new FrameworkPropertyMetadata(DateTimeMeasurementUnit.Day, FrameworkPropertyMetadataOptions.AffectsRender, DateTimeMeasureUnitPropertyChanged));
		[Obsolete(ObsoleteMessages.AxisDateTimeGridAlignmentProperty)]
		public static readonly DependencyProperty DateTimeGridAlignmentProperty = DependencyPropertyManager.Register("DateTimeGridAlignment", typeof(DateTimeMeasurementUnit),
			typeof(RadarAxisX2D), new FrameworkPropertyMetadata(DateTimeMeasurementUnit.Day, FrameworkPropertyMetadataOptions.AffectsRender, DateTimeGridAlignmentPropertyChanged));
		[Obsolete(ObsoleteMessages.RadarAxisX2DDateTimeOptionsProperty)]
		public static readonly DependencyProperty DateTimeOptionsProperty = DependencyPropertyManager.Register("DateTimeOptions", typeof(DateTimeOptions),
			typeof(RadarAxisX2D), new PropertyMetadata(DateTimeOptionsPropertyChanged));
		[Obsolete(ObsoleteMessages.RangeProperty)]
		public static readonly DependencyProperty RangeProperty;
		public static readonly DependencyProperty WholeRangeProperty = DependencyPropertyManager.Register("WholeRange", typeof(Range), typeof(RadarAxisX2D), new PropertyMetadata(WholeRangePropertyChanged));
		public static readonly DependencyProperty LogarithmicProperty = DependencyPropertyManager.Register("Logarithmic", typeof(bool),
			typeof(RadarAxisX2D), new FrameworkPropertyMetadata(false, LogarithmicPropertyChanged));
		public static readonly DependencyProperty LogarithmicBaseProperty = DependencyPropertyManager.Register("LogarithmicBase", typeof(double),
			typeof(RadarAxisX2D), new FrameworkPropertyMetadata(10.0, LogarithmicPropertyChanged, CoercyLogarithmicBase));
		public static readonly DependencyProperty DateTimeScaleOptionsProperty = DependencyPropertyManager.Register("DateTimeScaleOptions", typeof(DateTimeScaleOptionsBase),
			typeof(RadarAxisX2D), new PropertyMetadata(DateTimeScaleOptionsPropertyChanged));
		static object CoercyLogarithmicBase(DependencyObject d, object value) {
			if ((double)value <= 1)
				throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgInvalidLogarithmicBase));
			return value;
		}
		static void DateTimeMeasureUnitPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			RadarAxisX2D axis = d as RadarAxisX2D;
			if (axis != null)
				axis.ActualDateTimeMeasureUnit = (DateTimeMeasurementUnit)e.NewValue;
			ChartElementHelper.Update(d, e);
		}
		static void DateTimeGridAlignmentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			RadarAxisX2D axis = d as RadarAxisX2D;
			if (axis != null)
				axis.ActualGridAlignment = (DateTimeMeasurementUnit)e.NewValue;
			ChartElementHelper.Update(d, e);
		}
		static void DateTimeOptionsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			RadarAxisX2D axis = d as RadarAxisX2D;
			if (axis != null) {
				axis.dateTimeOptions = e.NewValue as DateTimeOptions;
				axis.UpdateLabelTextPattern();
				CommonUtils.SubscribePropertyChangedWeakEvent(e.OldValue as DateTimeOptions, e.NewValue as DateTimeOptions, axis);
			}
			ChartElementHelper.Update(d, e);
		}
		[Obsolete]
		static void RangePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			RadarAxisX2D axis = d as RadarAxisX2D;
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
		static void LogarithmicPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			RadarAxisX2D axis = d as RadarAxisX2D;
			if (axis != null)
				axis.ScaleMap.BuildTransformation(axis);
			ChartElementHelper.Update(d, e, ChartElementChange.ClearDiagramCache);
		}
		static void WholeRangePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			RadarAxisX2D axis = d as RadarAxisX2D;
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
		Obsolete(ObsoleteMessages.RadarAxisX2DDateTimeOptionsProperty),
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
		EditorBrowsable(EditorBrowsableState.Never),
		Obsolete(ObsoleteMessages.RangePropertyCircularAxis)
		]
		public AxisRange Range {
			get { return (AxisRange)GetValue(RangeProperty); }
			set { SetValue(RangeProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("RadarAxisX2DWholeRange"),
#endif
		Category(Categories.Common),
		XtraSerializableProperty(XtraSerializationVisibility.Content, true)
		]
		public Range WholeRange {
			get { return (Range)GetValue(WholeRangeProperty); }
			set { SetValue(WholeRangeProperty, value); }
		}
		[
		Category(Categories.Behavior),
		XtraSerializableProperty(XtraSerializationVisibility.Content, true)
		]
		public DateTimeScaleOptionsBase DateTimeScaleOptions {
			get { return (DateTimeScaleOptionsBase)GetValue(DateTimeScaleOptionsProperty); }
			set { SetValue(DateTimeScaleOptionsProperty, value); }
		}
		readonly ManualDateTimeScaleOptions defaultDateTimeScaleOptions = new ManualDateTimeScaleOptions();
		DateTimeOptions dateTimeOptions;
		protected override DateTimeScaleOptionsBase DefaultDateTimeScaleOptions { get { return defaultDateTimeScaleOptions; } }
		protected internal override DateTimeOptions DateTimeOptionsImpl { get { return dateTimeOptions; } }
		[Obsolete]
		static RadarAxisX2D() {
			RangeProperty = DependencyPropertyManager.Register("Range", typeof(AxisRange), typeof(RadarAxisX2D), new PropertyMetadata(RangePropertyChanged));
		}
		public RadarAxisX2D() {
			DefaultStyleKey = typeof(RadarAxisX2D);
		}
		#region ILogarithmic implementation
		bool ILogarithmic.Enabled { get { return Logarithmic; } }
		double ILogarithmic.Base { get { return LogarithmicBase; } }
		#endregion
		protected internal override IAxisMapping CreateMapping(Rect bounds) {
			IList<double> values = GridAndTextData != null ? GridAndTextData.GridData.Items.VisibleValues : new List<double>();
			return new CircularAxisMapping(this, Math.Min(bounds.Height / 2.0, bounds.Width / 2.0), values, false);
		}
	}
	public sealed class PolarAxisX2D : CircularAxisX2D {
		protected internal override DateTimeOptions DateTimeOptionsImpl { get { return null; } }
		protected override bool IsFixedRange { get { return true; } }
		public PolarAxisX2D() {
			DefaultStyleKey = typeof(PolarAxisX2D);
			((IAxisData)this).WholeRange.UpdateRange(0.0, 360.0, 0.0, 360.0);
			((IAxisData)this).VisualRange.UpdateRange(0.0, 360.0, 0.0, 360.0);
		}
		protected internal override IAxisMapping CreateMapping(Rect bounds) {
			IList<double> values = GridAndTextData != null ? GridAndTextData.GridData.Items.VisibleValues : new List<double>();
			return new CircularAxisMapping(this, Math.Min(bounds.Height / 2.0, bounds.Width / 2.0), values, true);
		}
	}
}
