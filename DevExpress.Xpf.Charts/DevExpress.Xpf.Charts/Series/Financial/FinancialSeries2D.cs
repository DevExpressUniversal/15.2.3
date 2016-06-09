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
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Charts {
	public abstract class FinancialSeries2D : XYSeries2D, IBarSeriesView, IFinancialSeriesView {
		public static readonly DependencyProperty LowValueProperty = DependencyPropertyManager.RegisterAttached("LowValue", 
			typeof(double), typeof(FinancialSeries2D), new PropertyMetadata(Double.NaN, SeriesPoint.Update));
		public static readonly DependencyProperty HighValueProperty = DependencyPropertyManager.RegisterAttached("HighValue", 
			typeof(double), typeof(FinancialSeries2D), new PropertyMetadata(Double.NaN, SeriesPoint.Update));
		public static readonly DependencyProperty OpenValueProperty = DependencyPropertyManager.RegisterAttached("OpenValue", 
			typeof(double), typeof(FinancialSeries2D), new PropertyMetadata(Double.NaN, SeriesPoint.Update));
		public static readonly DependencyProperty CloseValueProperty = DependencyPropertyManager.RegisterAttached("CloseValue", 
			typeof(double), typeof(FinancialSeries2D), new PropertyMetadata(Double.NaN, SeriesPoint.Update));
		public static readonly DependencyProperty ValueToDisplayProperty = DependencyPropertyManager.RegisterAttached("ValueToDisplay",
			typeof(StockLevel), typeof(FinancialSeries2D), new PropertyMetadata(StockLevel.CloseValue, PointOptions.ValueToDisplayPropertyChanged));
		public static readonly DependencyProperty LowValueDataMemberProperty = DependencyPropertyManager.Register("LowValueDataMember",
			typeof(string), typeof(FinancialSeries2D), new PropertyMetadata(String.Empty, OnBindingChanged));
		public static readonly DependencyProperty HighValueDataMemberProperty = DependencyPropertyManager.Register("HighValueDataMember", 
			typeof(string), typeof(FinancialSeries2D), new PropertyMetadata(String.Empty, OnBindingChanged));
		public static readonly DependencyProperty OpenValueDataMemberProperty = DependencyPropertyManager.Register("OpenValueDataMember", 
			typeof(string), typeof(FinancialSeries2D), new PropertyMetadata(String.Empty, OnBindingChanged));
		public static readonly DependencyProperty CloseValueDataMemberProperty = DependencyPropertyManager.Register("CloseValueDataMember", 
			typeof(string), typeof(FinancialSeries2D), new PropertyMetadata(String.Empty, OnBindingChanged));
		public static readonly DependencyProperty ReductionOptionsProperty = DependencyPropertyManager.Register("ReductionOptions", 
			typeof(ReductionStockOptions), typeof(FinancialSeries2D), new PropertyMetadata(ReductionStockOptionsPropertyChanged));
		public static readonly DependencyProperty PointAnimationProperty = DependencyPropertyManager.Register("PointAnimation",
			typeof(Stock2DAnimationBase), typeof(FinancialSeries2D), new PropertyMetadata(null, PointAnimationPropertyChanged));
		static void ReductionStockOptionsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			FinancialSeries2D series = d as FinancialSeries2D;
			if (series != null)
				CommonUtils.SubscribePropertyChangedWeakEvent(e.OldValue as ReductionStockOptions, e.NewValue as ReductionStockOptions, series);
			ChartElementHelper.Update(d, e);
		}
		[
		Category(Categories.Data),
		XtraSerializableProperty
		]
		public static double GetLowValue(SeriesPoint point) {
			return (double)point.GetValue(LowValueProperty);
		}
		public static void SetLowValue(SeriesPoint point, double value) {
			point.SetValue(LowValueProperty, value);
		}
		[
		Category(Categories.Data),
		XtraSerializableProperty
		]
		public static double GetHighValue(SeriesPoint point) {
			return (double)point.GetValue(HighValueProperty);
		}
		public static void SetHighValue(SeriesPoint point, double value) {
			point.SetValue(HighValueProperty, value);
		}
		[
		Category(Categories.Data),
		XtraSerializableProperty
		]
		public static double GetOpenValue(SeriesPoint point) {
			return (double)point.GetValue(OpenValueProperty);
		}
		public static void SetOpenValue(SeriesPoint point, double value) {
			point.SetValue(OpenValueProperty, value);
		}
		[
		Category(Categories.Data),
		XtraSerializableProperty
		]
		public static double GetCloseValue(SeriesPoint point) {
			return (double)point.GetValue(CloseValueProperty);
		}
		public static void SetCloseValue(SeriesPoint point, double value) {
			point.SetValue(CloseValueProperty, value);
		}
		[Obsolete(ObsoleteMessages.ValueToDisplayProperty)]
		public static StockLevel GetValueToDisplay(PointOptions options) {
			return (StockLevel)options.GetValue(ValueToDisplayProperty);
		}
		[Obsolete(ObsoleteMessages.ValueToDisplayProperty)]
		public static void SetValueToDisplay(PointOptions options, StockLevel valueToDisplay) {
			options.SetValue(ValueToDisplayProperty, valueToDisplay);
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("FinancialSeries2DLowValueDataMember"),
#endif
		Category(Categories.Data),
		XtraSerializableProperty
		]
		public string LowValueDataMember {
			get { return (string)GetValue(LowValueDataMemberProperty); }
			set { SetValue(LowValueDataMemberProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("FinancialSeries2DHighValueDataMember"),
#endif
		Category(Categories.Data),
		XtraSerializableProperty
		]
		public string HighValueDataMember {
			get { return (string)GetValue(HighValueDataMemberProperty); }
			set { SetValue(HighValueDataMemberProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("FinancialSeries2DOpenValueDataMember"),
#endif
		Category(Categories.Data),
		XtraSerializableProperty
		]
		public string OpenValueDataMember {
			get { return (string)GetValue(OpenValueDataMemberProperty); }
			set { SetValue(OpenValueDataMemberProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("FinancialSeries2DCloseValueDataMember"),
#endif
		Category(Categories.Data),
		XtraSerializableProperty
		]
		public string CloseValueDataMember {
			get { return (string)GetValue(CloseValueDataMemberProperty); }
			set { SetValue(CloseValueDataMemberProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("FinancialSeries2DReductionOptions"),
#endif
		Category(Categories.Behavior),
		XtraSerializableProperty(XtraSerializationVisibility.Content, true)
		]
		public ReductionStockOptions ReductionOptions {
			get { return (ReductionStockOptions)GetValue(ReductionOptionsProperty); }
			set { SetValue(ReductionOptionsProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("FinancialSeries2DPointAnimation"),
#endif
		Category(Categories.Animation),
		XtraSerializableProperty(XtraSerializationVisibility.Content, true)
		]
		public Stock2DAnimationBase PointAnimation {
			get { return (Stock2DAnimationBase)GetValue(PointAnimationProperty); }
			set { SetValue(PointAnimationProperty, value); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new bool ColorEach {
			get { return base.ColorEach; }
			set { base.ColorEach = value; }
		}
		internal Color ActualColor { get { return Brush == null ? Colors.Black : Brush.Color; } }
		internal ReductionStockOptions ActualReductionOptions { 
			get { 
				ReductionStockOptions reductionOptions = ReductionOptions;
				return reductionOptions == null ? new ReductionStockOptions() : reductionOptions; 
			} 
		}
		protected internal override ResolveOverlappingMode LabelsResolveOverlappingMode {
			get {
				if (ActualLabel.ResolveOverlappingMode == ResolveOverlappingMode.JustifyAllAroundPoint ||
					ActualLabel.ResolveOverlappingMode == ResolveOverlappingMode.JustifyAroundPoint)
					return ResolveOverlappingMode.Default;
				return ActualLabel.ResolveOverlappingMode;
			}
		}
		protected internal override Type SupportedDiagramType { get { return typeof(XYDiagram2D); } }
		protected internal override VisualSelectionType SupportedSelectionType { get { return VisualSelectionType.Brightness; } }
		protected internal override bool ActualColorEach { get { return false; } }
		protected internal override bool IsLabelConnectorItemVisible { get { return false; } }
		protected internal override bool LabelsResolveOverlappingSupported { get { return true; } }
		protected internal override bool LabelConnectorSupported { get { return false; } }
		protected internal override bool ArePointsVisible { get { return true; } }
		protected internal override Color BaseColor { get { return ActualColor; } }
		protected override int PointDimension { get { return 4; } }
		protected override string DefaultLegendTextPattern { get { return "{" + PatternUtils.CloseValuePlaceholder + "}"; } }
		protected abstract double BarWidth { get; }
		protected internal override ToolTipPointDataToStringConverter ToolTipPointValuesConverter { get { return new ToolTipFinancialValueToStringConverter(this); } }
		protected override Type PointInterfaceType {
			get { return typeof(IFinancialPoint); }
		}
		protected override bool NeedFilterVisiblePoints { get { return false; } }
		protected override int PixelsPerArgument { get { return 20; } }
		double GetReductionValue(IFinancialPoint refinedPoint) {
			switch (ActualReductionOptions.Level) {
				case StockLevel.LowValue:
					return refinedPoint.Low;
				case StockLevel.HighValue:
					return refinedPoint.High;
				case StockLevel.OpenValue:
					return refinedPoint.Open;
				case StockLevel.CloseValue:
					return refinedPoint.Close;
				default:
					ChartDebug.Fail("Unknown StockLevel value.");
					return 0;
			}
		}
		protected override IEnumerable<double> GetCrosshairValues(RefinedPoint refinedPoint) {
			IFinancialPoint financialPoint = (IFinancialPoint)refinedPoint;
			yield return financialPoint.Low;
			yield return financialPoint.High;
			yield return financialPoint.Open;
			yield return financialPoint.Close;
		}
		protected override ISeriesPoint CreateSeriesPoint(object argument, double internalArgument, object[] values, double[] internalValues, object tag, object hint, object color) {
			SolidColorBrush pointBrush = ColorizerController.GetSeriesPointBrush(argument, values, color);
			SeriesPoint point = new SeriesPoint(argument, internalArgument, Double.NaN, DateTime.MinValue, internalValues, tag, hint, pointBrush);
			SetHighValue(point, (double)values[0]);
			SetLowValue(point, (double)values[1]);
			SetOpenValue(point, (double)values[2]);
			SetCloseValue(point, (double)values[3]);
			return point;
		}
		protected override void AssignForBinding(Series series) {
			base.AssignForBinding(series);
			FinancialSeries2D financialSeries2D = series as FinancialSeries2D;
			if (financialSeries2D != null) {
				CopyPropertyValueHelper.CopyPropertyValue(this, financialSeries2D, LowValueDataMemberProperty);
				CopyPropertyValueHelper.CopyPropertyValue(this, financialSeries2D, HighValueDataMemberProperty);
				CopyPropertyValueHelper.CopyPropertyValue(this, financialSeries2D, OpenValueDataMemberProperty);
				CopyPropertyValueHelper.CopyPropertyValue(this, financialSeries2D, CloseValueDataMemberProperty);
				if (CopyPropertyValueHelper.IsValueSet(financialSeries2D, ReductionOptionsProperty))
					if (CopyPropertyValueHelper.VerifyValues(this, financialSeries2D, ReductionOptionsProperty)) {
						ReductionOptions = new ReductionStockOptions();
						ReductionOptions.Assign(financialSeries2D.ReductionOptions);
					}
				if (CopyPropertyValueHelper.IsValueSet(financialSeries2D, PointAnimationProperty))
					if (CopyPropertyValueHelper.VerifyValues(this, financialSeries2D, PointAnimationProperty))
						PointAnimation = financialSeries2D.PointAnimation.CloneAnimation() as Stock2DAnimationBase;
			}
		}
		protected override bool PerformWeakEvent(Type managerType, object sender, EventArgs e) {
			bool success = false;
			if (managerType == typeof(PropertyChangedWeakEventManager) && sender is ReductionStockOptions) {
				ChartElementHelper.Update(this);
				success = true;
			}
			return success || base.PerformWeakEvent(managerType, sender, e);
		}
		protected internal override double[] GetAnimatedPointValues(SeriesPoint point) {
			return GetPointValues(point);
		}
		protected internal override double[] GetPointValues(SeriesPoint point) {
			return new double[] { GetLowValue(point), GetHighValue(point), GetOpenValue(point), GetCloseValue(point) };
		}
		protected internal override void SetPointValues(SeriesPoint seriesPoint, double[] values, DateTime[] dateTimeValues) {
			if (values != null && values.Length > 3) {
				SetLowValue(seriesPoint, values[0]);
				SetHighValue(seriesPoint, values[1]);
				SetOpenValue(seriesPoint, values[2]);
				SetCloseValue(seriesPoint, values[3]);
			}
		}
		protected override IList<string> GetValueDataMembers() {
			return new string[] { HighValueDataMember, LowValueDataMember, OpenValueDataMember, CloseValueDataMember };
		}
		protected internal override Color GetPointOriginalColorForCustomDraw(IRefinedSeries refinedSeries, int pointIndex, Color seriesColor) {
			if (pointIndex == 0)
				return seriesColor;
			RefinedPoint refinedPoint = ((IRefinedSeries)refinedSeries).Points[pointIndex];
			if (refinedPoint == null || refinedPoint.IsEmpty)
				return seriesColor;
			if(ActualReductionOptions.Enabled)
				while (--pointIndex >= 0) {
					RefinedPoint previousRefinedPoint = ((IRefinedSeries)refinedSeries).Points[pointIndex];
					if (previousRefinedPoint != null && !previousRefinedPoint.IsEmpty)
						return GetReductionValue(refinedPoint) < GetReductionValue(previousRefinedPoint) ? ActualReductionOptions.ActualBrush.Color : seriesColor;
				}
			return seriesColor;
		}
		protected internal override Brush GetPenBrush(SolidColorBrush brush) {
			return null;
		}
		protected override XYSeriesLabel2DLayout CreateSeriesLabelLayout(SeriesLabelItem labelItem, PaneMapping mapping, Transform transform) {
			IFinancialPoint financialPointInfo = labelItem.RefinedPoint;
			if (financialPointInfo == null)
				return null;
			double value = Math.Max(Math.Max(financialPointInfo.Low, financialPointInfo.High), Math.Max(financialPointInfo.Open, financialPointInfo.Close));
			Point anchorPoint = transform.Transform(mapping.GetRoundedDiagramPoint(financialPointInfo.Argument, value));
			IAxisData axisY = ActualAxisY;
			bool reversed = axisY != null && axisY.Reverse;
			Point centerPoint = anchorPoint;
			if (mapping.Rotated)
				centerPoint.X += (reversed ? -1 : 1) * (ActualLabel.Indent + labelItem.LabelSize.Width / 2); 
			else
				centerPoint.Y += (reversed ? 1 : -1) * (ActualLabel.Indent + labelItem.LabelSize.Height / 2);
			return new XYSeriesLabel2DLayout(labelItem, anchorPoint, mapping, centerPoint);
		}
		protected abstract int CalculateSeriesPointWidth(IAxisMapping mapping);
		protected abstract void CorrectOpenClosePositions(ref double openPosition, ref double closePosition);
		double GetFinancialValuePortion(double value, double bottomValue, double topValue) {
			double height = topValue - bottomValue;
			double valuePortion = height != 0 ? (value -  bottomValue) / height : 0.0;
			return IsAxisYReversed ? 1 - valuePortion : valuePortion;
		}
		protected abstract FinancialSeries2DPointLayout CreateFinancialSeriesPointLayout(IFinancialPoint refinedPoint, Rect viewport, Rect bounds, FinancialLayoutPortions layoutPortions);
		protected override SeriesPointLayout CreateSeriesPointLayout(PaneMapping mapping, SeriesPointItem pointItem) {
			return CreatePointItemLayout(mapping, pointItem);
		}
		protected internal override void CompletePointLayout(SeriesPointItem pointItem) {
			FinancialSeries2DPointLayout layout = pointItem.Layout as FinancialSeries2DPointLayout;
			XYDiagram2D diagram = Diagram as XYDiagram2D;
			if (layout != null && diagram != null) {
				Rect stockBounds = layout.InitialBounds;
				Stock2DAnimationBase animation = GetActualPointAnimation() as Stock2DAnimationBase;
				if(animation != null) {
					double progress = pointItem.PointProgress.ActualProgress;
					stockBounds = animation.CreateAnimatedStockBounds(stockBounds, layout.Viewport, IsAxisXReversed, IsAxisYReversed, diagram.Rotated, progress);
				}
				bool shouldFlipSeriesPoint = IsAxisYReversed;
				Transform seriesPointTransform = null;
				if (shouldFlipSeriesPoint)
					seriesPointTransform = new ScaleTransform() { CenterY = stockBounds.Height / 2, ScaleY = -1 };
				else
					seriesPointTransform = new MatrixTransform() { Matrix = Matrix.Identity };
				layout.Complete(stockBounds, seriesPointTransform);
			}
		}
		protected internal override Point CalculateToolTipPoint(SeriesPointItem pointItem, PaneMapping mapping, Transform transform, bool inLabel) {
			IFinancialPoint financialPointInfo = pointItem.RefinedPoint as IFinancialPoint;
			double value = Math.Max(Math.Max(financialPointInfo.Low, financialPointInfo.High), Math.Max(financialPointInfo.Open, financialPointInfo.Close));
			Point anchorPoint = transform.Transform(mapping.GetRoundedDiagramPoint(financialPointInfo.Argument, value));
			return anchorPoint;
		}
		protected internal override SeriesPointAnimationBase CreateDefaultPointAnimation() {
			return new Stock2DSlideFromRightAnimation();
		}
		protected override void FillPredefinedPointAnimationKinds(List<AnimationKind> animationKinds) {
			animationKinds.Add(new AnimationKind(typeof(Stock2DSlideFromLeftAnimation)));
			animationKinds.Add(new AnimationKind(typeof(Stock2DSlideFromRightAnimation)));
			animationKinds.Add(new AnimationKind(typeof(Stock2DSlideFromTopAnimation)));
			animationKinds.Add(new AnimationKind(typeof(Stock2DSlideFromBottomAnimation)));
			animationKinds.Add(new AnimationKind(typeof(Stock2DExpandAnimation)));
			animationKinds.Add(new AnimationKind(typeof(Stock2DFadeInAnimation)));
		}
		protected override SeriesPointLayout CreatePointItemLayout(PaneMapping mapping, SeriesPointItem pointItem) {
			IFinancialPoint financialPointInfo = pointItem.RefinedPoint;
			if (financialPointInfo == null)
				return null;
			IAxisMapping valueMapping = mapping.AxisYMapping;
			double high = valueMapping.GetRoundedAxisValue(financialPointInfo.High);
			double low = valueMapping.GetRoundedAxisValue(financialPointInfo.Low);
			Render2DHelper.CorrectBounds(ref high, ref low);
			double open = valueMapping.GetRoundedAxisValue(financialPointInfo.Open);
			double close = valueMapping.GetRoundedAxisValue(financialPointInfo.Close);
			CorrectOpenClosePositions(ref open, ref close);
			double top = Math.Max(Math.Max(low, high), Math.Max(open, close));
			double bottom = Math.Min(Math.Min(low, high), Math.Min(open, close));
			double lowPortion = GetFinancialValuePortion(low, bottom, top);
			double highPortion = GetFinancialValuePortion(high, bottom, top);
			double openPortion = GetFinancialValuePortion(open, bottom, top);
			double closePortion = GetFinancialValuePortion(close, bottom, top);
			int candleWidth = CalculateSeriesPointWidth(mapping.AxisXMapping);
			double horizontalCenter = mapping.AxisXMapping.GetRoundedAxisValue(financialPointInfo.Argument);
			if (MathUtils.StrongRound(candleWidth) % 2 != 0)
				horizontalCenter += 0.5;
			Rect viewport = new Rect(0, 0, mapping.Viewport.Width, mapping.Viewport.Height);
			Rect bounds = new Rect(new Point(horizontalCenter - (double)candleWidth / 2, top), new Point(horizontalCenter + (double)candleWidth / 2, bottom));
			bounds.Height = Math.Max(bounds.Height, 1.0);
			return CreateFinancialSeriesPointLayout(financialPointInfo, viewport, bounds, new FinancialLayoutPortions(lowPortion, highPortion, openPortion, closePortion));
		}
		protected override double GetRefinedPointMax(RefinedPoint point) {
			IFinancialPoint financialPoint = (IFinancialPoint)point;
			return Math.Max(Math.Max(financialPoint.Low, financialPoint.High), Math.Max(financialPoint.Open, financialPoint.Close));
		}
		protected override double GetRefinedPointMin(RefinedPoint point) {
			IFinancialPoint financialPoint = (IFinancialPoint)point;
			return Math.Min(Math.Min(financialPoint.Low, financialPoint.High), Math.Min(financialPoint.Open, financialPoint.Close));
		}
		protected override double GetRefinedPointAbsMin(RefinedPoint point) {
			IFinancialPoint financialPoint = (IFinancialPoint)point;
			return Math.Min(Math.Min(Math.Abs(financialPoint.Low), Math.Abs(financialPoint.High)), Math.Min(Math.Abs(financialPoint.Open), Math.Abs(financialPoint.Close)));
		}
		protected override bool IsCorrectValueLevel(ValueLevelInternal valueLevel) {
			return valueLevel == ValueLevelInternal.High || valueLevel == ValueLevelInternal.Low || valueLevel ==ValueLevelInternal.Open || valueLevel == ValueLevelInternal.Close;
		}
		protected internal override string ConstructValuePattern(PointOptionsContainerBase pointOptionsContainer, ScaleType valueScaleType) {
			StockLevel stockLevel = (StockLevel)pointOptionsContainer.PointOptions.GetValue(ValueToDisplayProperty);
			string valueFormat = pointOptionsContainer.ConstructValueFormat(valueScaleType);
			string valuePattern = PatternUtils.ValuePlaceholder;
			switch (stockLevel) {
				case StockLevel.LowValue:
					valuePattern = PatternUtils.LowValuePlaceholder;
					break;
				case StockLevel.HighValue:
					valuePattern = PatternUtils.HighValuePlaceholder;
					break;
				case StockLevel.OpenValue:
					valuePattern = PatternUtils.OpenValuePlaceholder;
					break;
				case StockLevel.CloseValue:
					valuePattern = PatternUtils.CloseValuePlaceholder;
					break;
			}
			return "{" + valuePattern + valueFormat + "}";
		}
		protected internal override string[] GetAvailablePointPatternPlaceholders() {
			return ToolTipPatternUtils.FinancialViewPointPatterns;
		}
		public override SeriesPointAnimationBase GetPointAnimation() { return PointAnimation; }
		public override void SetPointAnimation(SeriesPointAnimationBase value) {
			if (value != null && !(value is Stock2DAnimationBase))
				return;
			PointAnimation = value as Stock2DAnimationBase;
		}
		double IBarSeriesView.BarWidth {
			get {
				return BarWidth;
			}
			set {
				throw new NotImplementedException();
			}
		}
	}
	public class FinancialLayoutPortions {
		readonly double low;
		readonly double high;
		readonly double open;
		readonly double close;
		public double Low { get { return low; } }
		public double High { get { return high; } }
		public double Open { get { return open; } }
		public double Close { get { return close; } }
		public FinancialLayoutPortions(double lowPortion, double highPortion, double openPortion, double closePortion) {
			this.low = lowPortion;
			this.high = highPortion;
			this.open = openPortion;
			this.close = closePortion;
		}
	}
}
namespace DevExpress.Xpf.Charts.Native {
	public class FinancialValueToStringConverter : ValueToStringConverter {
		readonly StockLevel valueLevel;
		public FinancialValueToStringConverter(INumericOptions numericOptions, IDateTimeOptions dateTimeOptions, StockLevel valueLevel)
			: base(numericOptions, dateTimeOptions) {
			this.valueLevel = valueLevel;
		}
		protected override object GetValue(object[] values) {
			switch (valueLevel) {
				case StockLevel.LowValue:
					return values[0];
				case StockLevel.HighValue:
					return values[1];
				case StockLevel.OpenValue:
					return values[2];
				case StockLevel.CloseValue:
					return values[3];
				default:
					ChartDebug.Fail("Unknown StockLevel value.");
					return 0;
			}
		}
	}
}
