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
using DevExpress.Utils;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Gauges.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Gauges {
	public enum ArcScaleLayoutMode {
		Circle = 0,
		Ellipse = 1,
		Auto = 2,
		HalfTop = 3,
		QuarterTopLeft = 4,
		QuarterTopRight = 5,
		ThreeQuarters = 6
	}
	public class ArcScale : Scale {
		const double halfTopStartAngleLimit1 = 180.0;
		const double halfTopStartAngleLimit2 = 225.0;
		const double halfTopEndAngleLimit1 = 315.0;
		const double halfTopEndAngleLimit2 = 360.0;
		const double quarterTopLeftStartAngleLimit1 = 180.0;
		const double quarterTopLeftStartAngleLimit2 = 270.0;
		const double quarterTopLeftEndAngleLimit1 = 180.0;
		const double quarterTopLeftEndAngleLimit2 = 270.0;
		const double quarterTopRightStartAngleLimit1 = 270.0;
		const double quarterTopRightStartAngleLimit2 = 360.0;
		const double quarterTopRightEndAngleLimit1 = 270.0;
		const double quarterTopRightEndAngleLimit2 = 360.0;
		const double threeQuartersStartAngleLimit1 = 135.0;
		const double threeQuartersStartAngleLimit2 = 180.0;
		const double threeQuartersEndAngleLimit1 = 0.0;
		const double threeQuartersEndAngleLimit2 = 45.0;
		public static readonly DependencyProperty SpindleCapPresentationProperty = DependencyPropertyManager.Register("SpindleCapPresentation",
			typeof(SpindleCapPresentation), typeof(ArcScale), new PropertyMetadata(null, PresentationPropertyChanged));
		public static readonly DependencyProperty LayoutModeProperty = DependencyPropertyManager.Register("LayoutMode",
			typeof(ArcScaleLayoutMode), typeof(ArcScale), new PropertyMetadata(ArcScaleLayoutMode.Auto, LayoutModePropertyChanged));
		public static readonly DependencyProperty StartAngleProperty = DependencyPropertyManager.Register("StartAngle",
			typeof(double), typeof(ArcScale), new PropertyMetadata(-240.0, AnglesPropertyChanged));
		public static readonly DependencyProperty EndAngleProperty = DependencyPropertyManager.Register("EndAngle",
			typeof(double), typeof(ArcScale), new PropertyMetadata(60.0, AnglesPropertyChanged));
		public static readonly DependencyProperty LabelOptionsProperty = DependencyPropertyManager.Register("LabelOptions",
			typeof(ArcScaleLabelOptions), typeof(ArcScale), new PropertyMetadata(null, LabelOptionsPropertyChanged));
		internal static readonly DependencyPropertyKey NeedlesPropertyKey = DependencyPropertyManager.RegisterReadOnly("Needles",
			typeof(ArcScaleNeedleCollection), typeof(ArcScale), new PropertyMetadata());
		public static readonly DependencyProperty NeedlesProperty = NeedlesPropertyKey.DependencyProperty;
		internal static readonly DependencyPropertyKey RangeBarsPropertyKey = DependencyPropertyManager.RegisterReadOnly("RangeBars",
			typeof(ArcScaleRangeBarCollection), typeof(ArcScale), new PropertyMetadata());
		public static readonly DependencyProperty RangeBarsProperty = RangeBarsPropertyKey.DependencyProperty;
		internal static readonly DependencyPropertyKey MarkersPropertyKey = DependencyPropertyManager.RegisterReadOnly("Markers",
			typeof(ArcScaleMarkerCollection), typeof(ArcScale), new PropertyMetadata());
		public static readonly DependencyProperty MarkersProperty = MarkersPropertyKey.DependencyProperty;
		internal static readonly DependencyPropertyKey LayersPropertyKey = DependencyPropertyManager.RegisterReadOnly("Layers",
			typeof(ArcScaleLayerCollection), typeof(ArcScale), new PropertyMetadata());
		public static readonly DependencyProperty LayersProperty = LayersPropertyKey.DependencyProperty;
		internal static readonly DependencyPropertyKey RangesPropertyKey = DependencyPropertyManager.RegisterReadOnly("Ranges",
			typeof(ArcScaleRangeCollection), typeof(ArcScale), new PropertyMetadata());
		public static readonly DependencyProperty RangesProperty = RangesPropertyKey.DependencyProperty;
		public static readonly DependencyProperty ShowSpindleCapProperty = DependencyPropertyManager.Register("ShowSpindleCap",
			typeof(DefaultBoolean), typeof(ArcScale), new PropertyMetadata(DefaultBoolean.Default, InvalidateLayout));
		public static readonly DependencyProperty SpindleCapOptionsProperty = DependencyPropertyManager.Register("SpindleCapOptions",
			typeof(SpindleCapOptions), typeof(ArcScale), new PropertyMetadata(null, SpindleCapOptionsPropertyChanged));
		public static readonly DependencyProperty LinePresentationProperty = DependencyPropertyManager.Register("LinePresentation",
			typeof(ArcScaleLinePresentation), typeof(ArcScale), new PropertyMetadata(null, PresentationPropertyChanged));
		[
#if !SL
	DevExpressXpfGaugesLocalizedDescription("ArcScaleSpindleCapPresentation"),
#endif
		Category(Categories.Presentation)
		]
		public SpindleCapPresentation SpindleCapPresentation {
			get { return (SpindleCapPresentation)GetValue(SpindleCapPresentationProperty); }
			set { SetValue(SpindleCapPresentationProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGaugesLocalizedDescription("ArcScaleLayoutMode"),
#endif
		Category(Categories.Layout)
		]
		public ArcScaleLayoutMode LayoutMode {
			get { return (ArcScaleLayoutMode)GetValue(LayoutModeProperty); }
			set { SetValue(LayoutModeProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGaugesLocalizedDescription("ArcScaleStartAngle"),
#endif
		Category(Categories.Presentation)
		]
		public double StartAngle {
			get { return (double)GetValue(StartAngleProperty); }
			set { SetValue(StartAngleProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGaugesLocalizedDescription("ArcScaleEndAngle"),
#endif
		Category(Categories.Presentation)
		]
		public double EndAngle {
			get { return (double)GetValue(EndAngleProperty); }
			set { SetValue(EndAngleProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGaugesLocalizedDescription("ArcScaleNeedles"),
#endif
		Category(Categories.Elements)
		]
		public ArcScaleNeedleCollection Needles {
			get { return (ArcScaleNeedleCollection)GetValue(NeedlesProperty); }
		}
		[
#if !SL
	DevExpressXpfGaugesLocalizedDescription("ArcScaleRangeBars"),
#endif
		Category(Categories.Elements)
		]
		public ArcScaleRangeBarCollection RangeBars {
			get { return (ArcScaleRangeBarCollection)GetValue(RangeBarsProperty); }
		}
		[
#if !SL
	DevExpressXpfGaugesLocalizedDescription("ArcScaleMarkers"),
#endif
		Category(Categories.Elements)
		]
		public ArcScaleMarkerCollection Markers {
			get { return (ArcScaleMarkerCollection)GetValue(MarkersProperty); }
		}
		[
#if !SL
	DevExpressXpfGaugesLocalizedDescription("ArcScaleLabelOptions"),
#endif
		Category(Categories.Presentation)
		]
		public ArcScaleLabelOptions LabelOptions {
			get { return (ArcScaleLabelOptions)GetValue(LabelOptionsProperty); }
			set { SetValue(LabelOptionsProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGaugesLocalizedDescription("ArcScaleLayers"),
#endif
		Category(Categories.Elements)
		]
		public ArcScaleLayerCollection Layers {
			get { return (ArcScaleLayerCollection)GetValue(LayersProperty); }
		}
		[
#if !SL
	DevExpressXpfGaugesLocalizedDescription("ArcScaleRanges"),
#endif
		Category(Categories.Elements)
		]
		public ArcScaleRangeCollection Ranges {
			get { return (ArcScaleRangeCollection)GetValue(RangesProperty); }
		}
		[
#if !SL
	DevExpressXpfGaugesLocalizedDescription("ArcScaleShowSpindleCap"),
#endif
		Category(Categories.Behavior)
		]
		public DefaultBoolean ShowSpindleCap {
			get { return (DefaultBoolean)GetValue(ShowSpindleCapProperty); }
			set { SetValue(ShowSpindleCapProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGaugesLocalizedDescription("ArcScaleSpindleCapOptions"),
#endif
		Category(Categories.Presentation)
		]
		public SpindleCapOptions SpindleCapOptions {
			get { return (SpindleCapOptions)GetValue(SpindleCapOptionsProperty); }
			set { SetValue(SpindleCapOptionsProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGaugesLocalizedDescription("ArcScaleLinePresentation"),
#endif
		Category(Categories.Presentation)
		]
		public ArcScaleLinePresentation LinePresentation {
			get { return (ArcScaleLinePresentation)GetValue(LinePresentationProperty); }
			set { SetValue(LinePresentationProperty, value); }
		}
		static void LayoutModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			ArcScale scale = d as ArcScale;
			if (scale != null) {
				scale.CalculateActualLayoutMode();
				scale.UpdateModel();
				scale.Invalidate();
			}
		}
		static void AnglesPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			ArcScale scale = d as ArcScale;
			if (scale != null) {
				if (scale.LayoutMode == ArcScaleLayoutMode.Auto) {
					scale.CalculateActualLayoutMode();
					scale.UpdateModel();
				}
				scale.Invalidate();
			}
		}
		static void SpindleCapOptionsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			ArcScale scale = d as ArcScale;
			if(scale != null) {
				CommonUtils.SubscribePropertyChangedWeakEvent(e.OldValue as SpindleCapOptions, e.NewValue as SpindleCapOptions, scale);
				scale.OnSpindleCapOptionsChanged();
			}
		}
#if !SL
	[DevExpressXpfGaugesLocalizedDescription("ArcScalePredefinedSpindleCapPresentations")]
#endif
		public static List<PredefinedElementKind> PredefinedSpindleCapPresentations {
			get { return DevExpress.Xpf.Gauges.Native.PredefinedSpindleCapPresentations.PresentationKinds; }
		}
#if !SL
	[DevExpressXpfGaugesLocalizedDescription("ArcScalePredefinedLinePresentations")]
#endif
		public static List<PredefinedElementKind> PredefinedLinePresentations {
			get { return PredefinedArcScaleLinePresentations.PresentationKinds; }
		}
		readonly SpindleCap spindleCap;
		ArcScaleLayoutMode actualLayoutMode;
		internal override ScaleLinePresentation ActualLinePresentation {
			get {
				if(LinePresentation != null)
					return LinePresentation;
				if(Model != null && ((ArcScaleModel)Model).LinePresentation != null)
					return ((ArcScaleModel)Model).LinePresentation;
				return new DefaultArcScaleLinePresentation();
			}
		}
		public bool ActualShowSpindleCap {
			get {
				if(ShowSpindleCap == DefaultBoolean.Default && Model != null)
					return ((ArcScaleModel)Model).ShowSpindleCap;
				return ShowSpindleCap != DefaultBoolean.False;
			}
		}
		protected override ScaleModel Model { get { return Gauge != null && Gauge.ActualModel != null ? Gauge.ActualModel.GetScaleModel(ActualLayoutMode, Gauge.Scales.IndexOf(this)) : null; } }
		internal SpindleCapPresentation ActualSpindleCapPresentation {
			get {
				if(SpindleCapPresentation != null)
					return SpindleCapPresentation;
				if(Model != null && ((ArcScaleModel)Model).SpindleCapPresentation != null)
					return ((ArcScaleModel)Model).SpindleCapPresentation;
				return new DefaultSpindleCapPresentation();
			}
		}
		protected internal override ScaleLabelOptions ActualLabelOptions {
			get {
				if(LabelOptions != null)
					return LabelOptions;
				if(Model != null && ((ArcScaleModel)Model).LabelOptions != null)
					return ((ArcScaleModel)Model).LabelOptions;
				return new ArcScaleLabelOptions();
			}
		}
		protected internal override IEnumerable<IElementInfo> Elements {
			get {
				foreach(ArcScaleLayer layer in Layers)
					yield return layer.ElementInfo;
				foreach(ValueIndicatorBase indicator in Indicators)
					yield return indicator.ElementInfo;
				foreach(ArcScaleRange range in Ranges)
					yield return range.ElementInfo;
				yield return LineInfo;
				yield return MinorTickmarksInfo;
				yield return MajorTickmarksInfo;
				yield return LabelsInfo;
				foreach (ScaleCustomLabel label in CustomLabels)
					yield return label;
				foreach (ScaleCustomElement element in CustomElements)
					yield return element;
				yield return SpindeleCapInfo;
			}
		}
		protected internal override IEnumerable<ValueIndicatorBase> Indicators {
			get {
				if(Needles != null)
					foreach(ArcScaleIndicator indicator in Needles)
						yield return indicator;
				if(Markers != null)
					foreach(ArcScaleIndicator indicator in Markers)
						yield return indicator;
				if(RangeBars != null)
					foreach(ArcScaleIndicator indicator in RangeBars)
						yield return indicator;
			}
		}
		internal SpindleCapOptions ActualSpindleCapOptions {
			get {
				if(SpindleCapOptions != null)
					return SpindleCapOptions;
				if(Model != null && ((ArcScaleModel)Model).SpindleCapOptions != null)
					return ((ArcScaleModel)Model).SpindleCapOptions;
				return new SpindleCapOptions();
			}
		}
		internal LayerInfo SpindeleCapInfo { get { return spindleCap.ElementInfo; } }
		new internal ArcScaleMapping Mapping { get { return base.Mapping as ArcScaleMapping; } }
		internal new CircularGaugeControl Gauge { get { return base.Gauge as CircularGaugeControl; } }
		internal ArcScaleLayoutMode ActualLayoutMode { get { return actualLayoutMode; } }
		public ArcScale() {
			DefaultStyleKey = typeof(ArcScale);
			spindleCap = new SpindleCap(this);
			this.SetValue(NeedlesPropertyKey, new ArcScaleNeedleCollection(this));
			this.SetValue(RangeBarsPropertyKey, new ArcScaleRangeBarCollection(this));
			this.SetValue(MarkersPropertyKey, new ArcScaleMarkerCollection(this));
			this.SetValue(LayersPropertyKey, new ArcScaleLayerCollection(this));
			this.SetValue(RangesPropertyKey, new ArcScaleRangeCollection(this));
			UpdateElementsInfo();
		}
		protected override void UpdateModel() {
			base.UpdateModel();
			OnSpindleCapOptionsChanged();
			foreach(ArcScaleLayer layer in Layers)
				((IModelSupported)layer).UpdateModel();
			foreach(ArcScaleRange range in Ranges)
				((IModelSupported)range).UpdateModel();
		}
		protected override bool PerformWeakEvent(Type managerType, object sender, EventArgs e) {
			bool success = base.PerformWeakEvent(managerType, sender, e);
			if(managerType == typeof(PropertyChangedWeakEventManager)) {
				if(sender is SpindleCapOptions)
					OnSpindleCapOptionsChanged();
				success = true;
			}
			return success;
		}
		void OnSpindleCapOptionsChanged() {
			if(spindleCap.ElementInfo != null) {
				spindleCap.UpdateModel();
				Invalidate();
			}
		}
		internal void CalculateActualLayoutMode() {
			double normalizedStartAngle = MathUtils.NormalizeDegree(StartAngle);
			double normalizedEndAngle = MathUtils.NormalizeDegree(EndAngle);
			double arcAngle = Math.Abs(EndAngle - StartAngle);
			if (LayoutMode != ArcScaleLayoutMode.Auto)
				actualLayoutMode = LayoutMode;
			else {
				actualLayoutMode = ArcScaleLayoutMode.Auto;
				if (IsAnglesInRanges(normalizedStartAngle, normalizedEndAngle, halfTopStartAngleLimit1, halfTopStartAngleLimit2, halfTopEndAngleLimit1, halfTopEndAngleLimit2)
					&& arcAngle <= 180 && MathUtils.NormalizeDegree((StartAngle + EndAngle) / 2) > 180)
					actualLayoutMode = ArcScaleLayoutMode.HalfTop;
				if (IsAnglesInRanges(normalizedStartAngle, normalizedEndAngle, quarterTopLeftStartAngleLimit1, quarterTopLeftStartAngleLimit2, quarterTopLeftEndAngleLimit1, quarterTopLeftEndAngleLimit2)
					&& arcAngle <= 90)
					actualLayoutMode = ArcScaleLayoutMode.QuarterTopLeft;
				if (IsAnglesInRanges(normalizedStartAngle, normalizedEndAngle, quarterTopRightStartAngleLimit1, quarterTopRightStartAngleLimit2, quarterTopRightEndAngleLimit1, quarterTopRightEndAngleLimit2)
					&& arcAngle <= 90)
					actualLayoutMode = ArcScaleLayoutMode.QuarterTopRight;
				if (IsAnglesInRanges(normalizedStartAngle, normalizedEndAngle, threeQuartersStartAngleLimit1, threeQuartersStartAngleLimit2, threeQuartersEndAngleLimit1, threeQuartersEndAngleLimit2)
					&& arcAngle > 180)
					actualLayoutMode = ArcScaleLayoutMode.ThreeQuarters;
				if (arcAngle >= 360 || actualLayoutMode == ArcScaleLayoutMode.Auto)
				actualLayoutMode = ArcScaleLayoutMode.Circle;
			}
		}
		bool IsAngleInRange(double angle, double startAngle, double endAngle) {
			if (endAngle != 360)
				return angle >= startAngle && angle <= endAngle;
			else
				return (angle >= startAngle && angle < endAngle) || angle == 0;
		}
		bool IsAnglesInRanges(double startAngle, double endAngle, double startAngleLimit1, double startAngleLimit2, double endAngleLimit1, double endAngleLimit2) {
			if (IsAngleInRange(startAngle, startAngleLimit1, startAngleLimit2) &&
				IsAngleInRange(endAngle, endAngleLimit1, endAngleLimit2) ||
				IsAngleInRange(endAngle, startAngleLimit1, startAngleLimit2) &&
				IsAngleInRange(startAngle, endAngleLimit1, endAngleLimit2))
				return true;
			else
				return false;
		}
		protected override ScaleElementLayout CalculateLineLayout() {
			if(!ActualShowLine || Mapping.Layout.IsEmpty)
				return null;
			Geometry clip = ArcSegmentCalculator.CalculateGeometry(Mapping, Mapping.Scale.StartAngle, Mapping.Scale.EndAngle - Mapping.Scale.StartAngle, ActualLineOptions.Offset, ActualLineOptions.Thickness);
			Size size = new Size(Math.Max(Mapping.Layout.EllipseWidth + 2.0 * ActualLineOptions.Offset + ActualLineOptions.Thickness, 0), Math.Max(Mapping.Layout.EllipseHeight + 2.0 * ActualLineOptions.Offset + ActualLineOptions.Thickness, 0));
			clip.Transform = new TranslateTransform() { X = size.Width / 2 - Mapping.Layout.EllipseCenter.X, Y = size.Height / 2 - Mapping.Layout.EllipseCenter.Y };
			return new ScaleElementLayout(Mapping.Layout.EllipseCenter, size, clip);
		}
		protected override ScaleMapping CalculateMapping(Size constraint) {
			return new ArcScaleMapping(this, new Rect(0, 0, constraint.Width, constraint.Height));
		}
		protected internal override void UpdateElementsInfo() {
			base.UpdateElementsInfo();
		}
		protected internal override void CheckIndicatorEnterLeaveRange(ValueIndicatorBase indicator, double oldValue, double newValue) {
			foreach(ArcScaleRange range in Ranges)
				range.OnIndicatorEnterLeave(indicator, oldValue, newValue);
		}
	}
	public class ArcScaleCollection : ScaleCollection<ArcScale> {
		public ArcScaleCollection(CircularGaugeControl gauge)
			: base(gauge) {
		}
	}
}
