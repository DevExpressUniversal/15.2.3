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
	public enum PointAnimationOrder {
		Straight,
		Inverted,
		Random
	}
	public abstract class SeriesPointAnimationBase : AnimationBase {
		public static readonly DependencyProperty PointDelayProperty = DependencyPropertyManager.Register("PointDelay",
			typeof(TimeSpan), typeof(SeriesPointAnimationBase), new PropertyMetadata(TimeSpan.FromMilliseconds(100), NotifyPropertyChanged));
		public static readonly DependencyProperty PointOrderProperty = DependencyPropertyManager.Register("PointOrder",
			typeof(PointAnimationOrder), typeof(SeriesPointAnimationBase), new PropertyMetadata(PointAnimationOrder.Straight, NotifyPropertyChanged));
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("SeriesPointAnimationBasePointDelay"),
#endif
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public TimeSpan PointDelay {
			get { return (TimeSpan)GetValue(PointDelayProperty); }
			set { SetValue(PointDelayProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("SeriesPointAnimationBasePointOrder"),
#endif
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public PointAnimationOrder PointOrder {
			get { return (PointAnimationOrder)GetValue(PointOrderProperty); }
			set { SetValue(PointOrderProperty, value); }
		}
		protected internal virtual bool ShouldAnimateSeriesPointLayout { get { return true; } }
		protected virtual double LabelDurationPortion { get { return 0.5; } }
		TimeSpan GetTimelineBeginTime(int pointIndex) {
			return TimeSpan.FromTicks(((TimeSpan)PointDelay).Ticks * pointIndex);
		}
		Timeline CreateLabelTimeline(TimeSpan beginTime) {
			DoubleAnimation timeline = new DoubleAnimation();
			timeline.From = 0.0;
			timeline.To = 1.0;
			TimeSpan labelDuration = TimeSpan.FromTicks((long)Math.Round(Duration.Ticks * LabelDurationPortion));
			TimeSpan labelBeginTime = TimeSpan.FromTicks((long)Math.Round(Duration.Ticks * (1.0 - LabelDurationPortion)));
			timeline.Duration = new Duration(labelDuration);
			timeline.BeginTime = beginTime + labelBeginTime;
			return timeline;
		}
		IEnumerable<SeriesPointData> GetPointDatas(Series series) {
			int count = series.Item.SeriesPointDataList.Count;
			switch (PointOrder) {
				case PointAnimationOrder.Straight:
				for (int i = 0; i < count; i++)
					yield return series.Item.SeriesPointDataList[i];
					break;
				case PointAnimationOrder.Inverted:
					for (int i = count - 1; i >= 0; i--)
						yield return series.Item.SeriesPointDataList[i];
					break;
				case PointAnimationOrder.Random:
					Random rnd = new Random();
					List<int> indexes = new List<int>();
					for (int i = 0; i < count; i++) {
						int index = MathUtils.StrongRound(rnd.NextDouble() * (count - 1));
						for (int j = 0; j < count; j++) {
							if (index + j < count && !indexes.Contains(index + j)) {
								indexes.Add(index + j);
								yield return series.Item.SeriesPointDataList[index + j];
								break;
							}
							else if (index - j >= 0 && !indexes.Contains(index - j)) {
								indexes.Add(index - j);
								yield return series.Item.SeriesPointDataList[index - j];
								break;
							}
						}
					}
					break;
				default:
					ChartDebug.Fail("Unknown PointAnimationOrder.");
					goto case PointAnimationOrder.Straight;
			}
		}
		protected internal virtual bool ShouldAnimateSeriesPointOpacity(Series series) { 
			return false; 
		}
		protected internal virtual bool ShouldAnimateSeriesLabelOpacity(Series series) {
			return true;
		}
		protected internal override void PrepareSeriesStoryboard(Storyboard seriesStoryboard, Series series) {
			Storyboard pointsStoryboard = new Storyboard();
			Storyboard labelsStoryboard = new Storyboard();
			int pointIndex = 0;
			foreach (SeriesPointData seriesPointData in GetPointDatas(series)) {
				TimeSpan beginTime = GetTimelineBeginTime(pointIndex++);
				Timeline pointTimeline = CreateTimeline(beginTime);
				Timeline labelTimeline = CreateLabelTimeline(beginTime);
				Storyboard.SetTarget(pointTimeline, seriesPointData.PointProgress);
				Storyboard.SetTarget(labelTimeline, seriesPointData.LabelProgress);
				Storyboard.SetTargetProperty(pointTimeline, new PropertyPath(AnimationProgress.ProgressProperty.GetName()));
				Storyboard.SetTargetProperty(labelTimeline, new PropertyPath(AnimationProgress.ProgressProperty.GetName()));
				pointTimeline.Freeze(); 
				labelTimeline.Freeze();
				pointsStoryboard.Children.Add(pointTimeline);
				labelsStoryboard.Children.Add(labelTimeline);
			}
			pointsStoryboard.BeginTime = BeginTime;
			labelsStoryboard.BeginTime = BeginTime;
			seriesStoryboard.Children.Add(pointsStoryboard);
			seriesStoryboard.Children.Add(labelsStoryboard);
		}
		protected override void Assign(AnimationBase animation) {
			base.Assign(animation);
			SeriesPointAnimationBase pointAnimation = animation as SeriesPointAnimationBase;
			if (pointAnimation != null) {
				CopyPropertyValueHelper.CopyPropertyValue(this, pointAnimation, PointDelayProperty);
				CopyPropertyValueHelper.CopyPropertyValue(this, pointAnimation, PointOrderProperty);
			}
		}
		internal SeriesPointAnimationBase CloneAnimation() {
			SeriesPointAnimationBase animation = (SeriesPointAnimationBase)Activator.CreateInstance(GetType());
			animation.Assign(this);
			return animation;
		}
	}
}
