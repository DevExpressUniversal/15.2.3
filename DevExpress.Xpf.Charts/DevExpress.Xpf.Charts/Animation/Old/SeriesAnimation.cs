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
using System.ComponentModel;
using System.Windows;
using DevExpress.Utils;
using DevExpress.Charts.Native;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Charts.Native;
namespace DevExpress.Xpf.Charts {
	public sealed class SeriesAnimation : ChartAnimation {
		static readonly DependencyPropertyKey ActionsPropertyKey = DependencyPropertyManager.RegisterReadOnly("Actions", 
			typeof(SeriesAnimationActionCollection), typeof(SeriesAnimation), new PropertyMetadata());
		public static readonly DependencyProperty ActionsProperty = ActionsPropertyKey.DependencyProperty;
		public static readonly DependencyProperty TargetSeriesProperty = DependencyPropertyManager.Register("TargetSeries", 
			typeof(Series), typeof(SeriesAnimation), new PropertyMetadata(null, TargetSeriesChanged));
		static void TargetSeriesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			if (e.NewValue != null) {
				SeriesAnimation animation = (SeriesAnimation)d;
				animation.PerformAnimation(animation.AnimationRecord.Progress);
			}
		}		
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("SeriesAnimationActions"),
#endif
		Category(Categories.Elements),
		]
		public SeriesAnimationActionCollection Actions { get { return (SeriesAnimationActionCollection)GetValue(ActionsProperty); } }
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("SeriesAnimationTargetSeries"),
#endif
		Category(Categories.Behavior)
		]
		public Series TargetSeries {
			get { return (Series)GetValue(TargetSeriesProperty); }
			set { SetValue(TargetSeriesProperty, value); }
		}
		public SeriesAnimation() {
			this.SetValue(ActionsPropertyKey, ChartElementHelper.CreateInstance<SeriesAnimationActionCollection>(this));
		}
		protected internal override void Initialize() {
			PerformAnimation(AnimationRecord.Progress);
		}
		protected internal override void PerformAnimation(double value) {
			foreach (SeriesAnimationAction action in Actions)
				action.PerformAnimation(value);
		}
	}
	public class SeriesAnimationActionCollection : ChartElementCollection<SeriesAnimationAction> { 
	}
	public abstract class SeriesAnimationAction : ChartElement {
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new bool Focusable { get { return base.Focusable; } set { base.Focusable = value; } }
		protected SeriesAnimation Animation { get { return (SeriesAnimation)((IChartElement)this).Owner; } }
		protected Series Series { get { return Animation.TargetSeries; } }
		protected internal abstract void PerformAnimation(double value);
	}
	public sealed class SeriesSeriesPointsAction : SeriesAnimationAction {
		public static readonly DependencyProperty SequentialProperty = DependencyPropertyManager.Register("Sequential", typeof(bool), typeof(SeriesSeriesPointsAction), new PropertyMetadata(false));
		public static readonly DependencyProperty EqualSpeedProperty = DependencyPropertyManager.Register("EqualSpeed", typeof(bool), typeof(SeriesSeriesPointsAction), new PropertyMetadata(false));
		public static readonly DependencyProperty DelayPercentageProperty = DependencyPropertyManager.Register("DelayPercentage", 
			typeof(double), typeof(SeriesSeriesPointsAction), new PropertyMetadata(0.0), ValidateDelayPercentage);
		static bool ValidateDelayPercentage(object value) {
			return (double)value >= 0.0;
		}
		static void PerformSeriesPointAnimation(SeriesPoint point, double value, double maximumValue) { 
			if (value <= 0.0)
				point.SetAnimatedValue(0.0);
			else if (value >= 1.0)
				point.SetAnimatedValue(point.NonAnimatedValue);
			else if (maximumValue == 0.0)
				point.SetAnimatedValue(point.NonAnimatedValue * value);
			else {
				double seriesPointValue = point.NonAnimatedValue;
				double valueToSet;
				IXYSeriesView xySeries = point.Series as IXYSeriesView;
				if (xySeries != null && ((Axis)xySeries.AxisYData).Logarithmic) {
					AxisScaleTypeMap map = ((AxisBase)xySeries.AxisYData).ScaleMap;
					valueToSet = (double)map.InternalToNative(Math.Min(Math.Abs(map.NativeToInternal(seriesPointValue)), map.NativeToInternal(maximumValue) * value));
				}
				else
					valueToSet = Math.Min(Math.Abs(seriesPointValue), maximumValue * value);
				point.SetAnimatedValue(seriesPointValue < 0 ? -valueToSet : valueToSet);
			}
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("SeriesSeriesPointsActionSequential"),
#endif
		Category(Categories.Behavior)
		]
		public bool Sequential {
			get { return (bool)GetValue(SequentialProperty); }
			set { SetValue(SequentialProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("SeriesSeriesPointsActionEqualSpeed"),
#endif
		Category(Categories.Behavior)
		]
		public bool EqualSpeed {
			get { return (bool)GetValue(EqualSpeedProperty); }
			set { SetValue(EqualSpeedProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("SeriesSeriesPointsActionDelayPercentage"),
#endif
		Category(Categories.Behavior)
		]
		public double DelayPercentage {
			get { return (double)GetValue(DelayPercentageProperty); }
			set { SetValue(DelayPercentageProperty, value); }
		}
		protected internal override void PerformAnimation(double value) {
			Series series = Series;
			if (series == null || Double.IsNaN(value))
				return;
			double maximumValue = 0.0;
			if (EqualSpeed) {
				foreach (SeriesPoint point in series.Points) {
					double seriesPointValue = Math.Abs(point.NonAnimatedValue);
					if (seriesPointValue > maximumValue)
						maximumValue = seriesPointValue;
				}
				if (maximumValue == 0.0)
					return;
			}
			if (Sequential) {
				double percentage = DelayPercentage / 100.0;
				value *= (series.Points.Count - 1) * percentage + 1.0;
				foreach (SeriesPoint point in series.Points) {
					PerformSeriesPointAnimation(point, value, maximumValue);
					value -= percentage;
				}
			}
			else
				foreach (SeriesPoint point in series.Points)
					PerformSeriesPointAnimation(point, value, maximumValue);
		}
	}
}
