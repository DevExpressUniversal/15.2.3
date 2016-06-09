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
using System.Windows.Media.Animation;
using DevExpress.Charts.Native;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Charts {
	public enum AnimationAutoStartMode {
		PlayOnce,
		SetStartState,
		SetFinalState
	}
	public abstract class AnimationBase : ChartDependencyObject {
		public static readonly DependencyProperty DurationProperty = DependencyPropertyManager.Register("Duration",
			typeof(TimeSpan), typeof(AnimationBase), new PropertyMetadata(TimeSpan.FromMilliseconds(800), NotifyPropertyChanged));
		public static readonly DependencyProperty BeginTimeProperty = DependencyPropertyManager.Register("BeginTime",
			typeof(TimeSpan), typeof(AnimationBase), new PropertyMetadata(TimeSpan.Zero, NotifyPropertyChanged));
		public static readonly DependencyProperty EasingFunctionProperty = DependencyPropertyManager.Register("EasingFunction",
			typeof(IEasingFunction), typeof(AnimationBase), new PropertyMetadata(null, NotifyPropertyChanged));
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("AnimationBaseDuration"),
#endif
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public TimeSpan Duration {
			get { return (TimeSpan)GetValue(DurationProperty); }
			set { SetValue(DurationProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("AnimationBaseBeginTime"),
#endif
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public TimeSpan BeginTime {
			get { return (TimeSpan)GetValue(BeginTimeProperty); }
			set { SetValue(BeginTimeProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("AnimationBaseEasingFunction"),
#endif
		Category(Categories.Behavior)
		]
		public IEasingFunction EasingFunction {
			get { return (IEasingFunction)GetValue(EasingFunctionProperty); }
			set { SetValue(EasingFunctionProperty, value); }
		}
		protected internal virtual string AnimationName { get { return "Animation"; } }
		protected Timeline CreateTimeline(TimeSpan beginTime) {
			DoubleAnimation timeline = new DoubleAnimation();
			timeline.From = 0.0;
			timeline.To = 1.0;
			timeline.Duration = Duration;
			timeline.BeginTime = beginTime;
			timeline.EasingFunction = EasingFunction != null ? EasingFunction : GetDefaultEasingFunction();
			return timeline;
		}
		protected virtual IEasingFunction GetDefaultEasingFunction() {
			return null;
		}
		protected internal abstract void PrepareSeriesStoryboard(Storyboard seriesStoryboard, Series series);
		protected virtual void Assign(AnimationBase animation) {
			if (animation != null) {
				CopyPropertyValueHelper.CopyPropertyValue(this, animation, DurationProperty);
				CopyPropertyValueHelper.CopyPropertyValue(this, animation, BeginTimeProperty);
				CopyPropertyValueHelper.CopyPropertyValue(this, animation, EasingFunctionProperty);
			}
		}
	}
}
namespace DevExpress.Xpf.Charts.Native {
	public enum AnimationState {
		BeforeFirst,
		InProgress,
		Completed
	}
	public class AnimationProgress : DependencyObject {
		public static readonly DependencyProperty ProgressProperty = DependencyPropertyManager.Register("Progress",
			typeof(double), typeof(AnimationProgress), new PropertyMetadata(1.0, ProgressPropertyChanged));
		public double Progress {
			get { return (double)GetValue(ProgressProperty); }
			set { SetValue(ProgressProperty, value); }
		}
		static void ProgressPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			AnimationProgress animationProgress = d as AnimationProgress;
			if (animationProgress != null)
				animationProgress.OnProgressChanged();
		}
		readonly IAnimatableElement element;
		public double ActualProgress { 
			get {
				if (element != null) {
					if (element.AnimationMode == ChartAnimationMode.Disabled)
						return 1.0;
					switch (element.AnimationState) {
						case AnimationState.BeforeFirst:
							return element.AnimationAutoStartMode == AnimationAutoStartMode.SetFinalState ? 1.0 : 0.0;
						case AnimationState.InProgress:
							return Progress;
						case AnimationState.Completed:
							return 1.0;
						default:
							ChartDebug.Fail("Unknown AnimationState.");
							goto case AnimationState.Completed;
					}
				}
				return 1.0;
			} 
		}
		public AnimationProgress(IAnimatableElement element) {
			this.element = element;
		}
		void OnProgressChanged() {
			if (element != null)
				element.ProgressChanged(this);
		}
		public void Start() {
			Progress = 0.0;
			OnProgressChanged();
		}
		public void Finish() {
			Progress = 1.0;
			OnProgressChanged();
		}
	}
	public static class AnimationHelper {
		const double delayfactor = 4.0;
		static List<Series> CreateSeriesSubsetBySampleSeries(Series sampleSeries, Diagram diagram) {
			List<Series> subset = new List<Series>();
			foreach (Series series in diagram.Series)
				if (series.GetType().Equals(sampleSeries.GetType()))
					subset.Add(series);
			return subset;
		}
		static void PrepareAnimation(AnimationBase animation, SeriesAnimationBase seriesAnimation, int seriesIndex) {
			animation.BeginTime = TimeSpan.FromMilliseconds(Math.Round((double)seriesAnimation.Duration.TotalMilliseconds / delayfactor) * seriesIndex);
		}
		public static void PrepareDefaultPointAnimation(SeriesPointAnimationBase pointAnimation, SeriesAnimationBase seriesAnimation, Series series, Diagram diagram) {
			if (pointAnimation == null || series == null || diagram == null)
				return;
			List<Series> subset = CreateSeriesSubsetBySampleSeries(series, diagram);
			int seriesIndex = subset.IndexOf(series);
			if (subset.Count > 0 && seriesIndex > -1) {
				if (seriesAnimation != null)
					PrepareAnimation(pointAnimation, seriesAnimation, seriesIndex);
				else {
					pointAnimation.BeginTime = TimeSpan.FromMilliseconds(pointAnimation.PointDelay.TotalMilliseconds * seriesIndex);
					pointAnimation.PointDelay = TimeSpan.FromMilliseconds(pointAnimation.PointDelay.TotalMilliseconds * subset.Count);
				}
			}
		}
		public static void PrepareDefaultSeriesAnimation(SeriesAnimationBase seriesAnimation, Series series, Diagram diagram) {
			if (seriesAnimation == null || series == null || diagram == null)
				return;
			List<Series> subset = CreateSeriesSubsetBySampleSeries(series, diagram);
			int seriesIndex = subset.IndexOf(series);
			if (subset.Count > 0 && seriesIndex > -1)
				PrepareAnimation(seriesAnimation, seriesAnimation, seriesIndex);
		}
		public static Rect CreateSlideFromLeftAnimatedBounds(Rect bounds, bool diagramRotated, double progress) {
			if (progress == 0.0)
				return RectExtensions.Zero;
			if (progress == 1.0)
				return bounds;
			double x, y;
			if (diagramRotated) {
				x = bounds.X;
				y = bounds.Y - (bounds.Y + bounds.Height) * (1.0 - progress);
			}
			else {
				x = bounds.X - (bounds.X + bounds.Width) * (1.0 - progress);
				y = bounds.Y;
			}
			double width = bounds.Width;
			double height = bounds.Height;
			return new Rect(x, y, width, height);
		}
		public static Rect CreateSlideFromRightAnimatedBounds(Rect bounds, Rect viewport, bool diagramRotated, double progress) {
			if (progress == 0.0)
				return RectExtensions.Zero;
			if (progress == 1.0)
				return bounds;
			double x, y;
			if (diagramRotated) {
				x = bounds.X;
				y = bounds.Y + (viewport.Width - bounds.Y) * (1.0 - progress);
			}
			else {
				x = bounds.X + (viewport.Width - bounds.X) * (1.0 - progress);
				y = bounds.Y;
			}
			double width = bounds.Width;
			double height = bounds.Height;
			return new Rect(x, y, width, height);
		}
		public static Rect CreateSlideFromTopAnimatedBounds(Rect bounds, Rect viewport, bool diagramRotated, double progress) {
			if (progress == 0.0)
				return RectExtensions.Zero;
			if (progress == 1.0)
				return bounds;
			double x, y;
			if (diagramRotated) {
				x = bounds.X + (viewport.Height - bounds.X) * (1.0 - progress);
				y = bounds.Y;
			}
			else {
				x = bounds.X;
				y = bounds.Y + (viewport.Height - bounds.Y) * (1.0 - progress);
			}
			double width = bounds.Width;
			double height = bounds.Height;
			return new Rect(x, y, width, height);
		}
		public static Rect CreateSlideFromBottomAnimatedBounds(Rect bounds, bool diagramRotated, double progress) {
			if (progress == 0.0)
				return RectExtensions.Zero;
			if (progress == 1.0)
				return bounds;
			double x, y;
			if (diagramRotated) {
				x = bounds.X - (bounds.X + bounds.Width) * (1.0 - progress);
				y = bounds.Y;
			}
			else {
				x = bounds.X;
				y = bounds.Y - (bounds.Y + bounds.Height) * (1.0 - progress);
			}
			double width = bounds.Width;
			double height = bounds.Height;
			return new Rect(x, y, width, height);
		}
		public static Rect CreateWidenAnimatedMarkerBounds(Rect markerBounds, Rect viewport, bool axisXReverse, bool axisYReverse, bool diagramRotated, double progress) {
			if (progress == 0.0)
				return RectExtensions.Zero;
			if (progress == 1.0)
				return markerBounds;
			double x = markerBounds.X + markerBounds.Width / 2 - markerBounds.Width * progress / 2;
			double y = markerBounds.Y + markerBounds.Height / 2 - markerBounds.Height * progress / 2;
			double width = markerBounds.Width * progress;
			double height = markerBounds.Height * progress;
			return new Rect(x, y, width, height);
		}
		public static bool ContainsAnimation(Storyboard root, DoubleAnimation animation) {
			foreach (Timeline timeline in root.Children) {
				Storyboard storyboard = timeline as Storyboard;
				if (storyboard != null) {
					if (ContainsAnimation(storyboard, animation))
						return true;
				}
				if (Object.ReferenceEquals(timeline as DoubleAnimation, animation))
					return true;
			}
			return false;
		}
	}
}
