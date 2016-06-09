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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Map.Native {
	public class ViewportAnimationProgress : DependencyObject {
		public static readonly DependencyProperty OriginProgressProperty = DependencyPropertyManager.Register("OriginProgress",
			typeof(double), typeof(ViewportAnimationProgress), new PropertyMetadata(1.0, ProgressPropertyChanged));
		public static readonly DependencyProperty SizeProgressProperty = DependencyPropertyManager.Register("SizeProgress",
			typeof(double), typeof(ViewportAnimationProgress), new PropertyMetadata(1.0, ProgressPropertyChanged));
		public double OriginProgress {
			get { return (double)GetValue(OriginProgressProperty); }
			set { SetValue(OriginProgressProperty, value); }
		}
		public double SizeProgress {
			get { return (double)GetValue(SizeProgressProperty); }
			set { SetValue(SizeProgressProperty, value); }
		}
		static void ProgressPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			ViewportAnimationProgress animationProgress = d as ViewportAnimationProgress;
			if (animationProgress != null)
				animationProgress.ProcessProgress(e.Property);
		}
		readonly IViewportAnimatableElement animationTarget;
		bool needRaiseSize = true;
		public const double ProgressLimit = 0.999;
		bool OriginInProgress {
			get { return animationTarget != null ? animationTarget.OriginInProgress : false; }
		}
		bool SizeInProgress {
			get { return animationTarget != null ? animationTarget.SizeInProgress : false; }
		}
		public double ActualOriginProgress {
			get {
				if (OriginInProgress)
					return OriginProgress;
				return 1.0;
			}
		}
		public double ActualSizeProgress {
			get {
				if (SizeInProgress)
					return SizeProgress;
				return 1.0;
			}
		}
		public ViewportAnimationProgress(IViewportAnimatableElement animationTarget) {
			this.animationTarget = animationTarget;
		}
		void ProcessProgress(DependencyProperty property) {
			RaiseProgressChanged();
			if(property == SizeProgressProperty)
				ProcessSizeProgress();
		}
		void ProcessSizeProgress() {
			if(SizeProgress == 0.0)
				needRaiseSize = true;
			if(SizeProgress > ProgressLimit && needRaiseSize) {
				if(animationTarget != null)
					animationTarget.BeforeSizeProgressCompleting();
				needRaiseSize = false;
			}
		}
		protected void RaiseProgressChanged() {
			if(animationTarget != null)
				animationTarget.ProgressChanged();
		}
		public void StartOriginAnimation() {
			OriginProgress = 0.0;
			RaiseProgressChanged();
		}
		public void StartSizeAnimation() {
			SizeProgress = 0.0;
			RaiseProgressChanged();
		}
		public void FinishOriginAnimation() {
			OriginProgress = 1.0;
		}
		public void FinishSizeAnimation() {
			SizeProgress = 1.0;
		}
	}
	public class AnimationHelper {
		static Timeline CreateTimeline() {
			DoubleAnimation timeline = new DoubleAnimation();
			timeline.From = 0;
			timeline.To = 1.0;
			timeline.Duration = new Duration(TimeSpan.FromSeconds(2.0));
			timeline.BeginTime = TimeSpan.Zero;
			PowerEase easingFunction = new PowerEase();
			easingFunction.EasingMode = EasingMode.EaseOut;
			easingFunction.Power = 8.0;
			timeline.EasingFunction = easingFunction;
			return timeline;
		}
		public static void PrepareStoryboard(Storyboard storyboard, ViewportAnimationProgress progress, DependencyProperty targetProperty) {
			Timeline timeline = CreateTimeline();
			Storyboard.SetTarget(timeline, progress);
			Storyboard.SetTargetProperty(timeline, new PropertyPath(targetProperty.GetName()));
			storyboard.Children.Add(timeline);
			storyboard.BeginTime = TimeSpan.Zero;
		}
		public static void AddStoryboard(Control owner, Storyboard storyboard, int resourceKey) {
			if (storyboard != null && !owner.Resources.Contains(resourceKey.ToString()))
				owner.Resources.Add(resourceKey.ToString(), storyboard);
		}
	}
}
