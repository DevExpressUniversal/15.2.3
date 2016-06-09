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
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Threading;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.Editors.RangeControl.Internal {
	public enum SelectionChangesType { Start, End };
	public class StartEndUpdateHelper {
		RangeControl RangeControl { get; set; }
		[IgnoreDependencyPropertiesConsistencyChecker]
		readonly DependencyProperty StartValueProperty;
		[IgnoreDependencyPropertiesConsistencyChecker]
		readonly DependencyProperty EndValueProperty;
		public IComparableObjectWrapper StartValue { get; set; }
		public IComparableObjectWrapper EndValue { get; set; }
		public StartEndUpdateHelper(RangeControl rangeControl, DependencyProperty startProperty, DependencyProperty endProperty) {
			RangeControl = rangeControl;
			StartValueProperty = startProperty;
			EndValueProperty = endProperty;
		}
		public void Update<T>(StartEndUpdateSource updateSource) where T : struct, IComparable {
			if (StartValue != null && !StartValue.IsInfinity && !object.Equals(StartValue.RealValue, RangeControl.GetValue(StartValueProperty)))
				RangeControl.SetCurrentValue(StartValueProperty, StartValue.RealValue);
			if (EndValue != null && !EndValue.IsInfinity && !object.Equals(EndValue.RealValue, RangeControl.GetValue(EndValueProperty)))
				RangeControl.SetCurrentValue(EndValueProperty, EndValue.RealValue);
		}
	}
	public enum StartEndUpdateSource {
		StartChanged,
		EndChanged,
		ISupportInitialize
	}
	public class IComparableObjectWrapper : IComparable {
		public IComparable Value { get; private set; }
		public object RealValue { get; private set; }
		public bool IsInfinity { get; private set; }
		public IComparableObjectWrapper(IComparable value, object realValue, bool isInfinity) {
			Value = value;
			RealValue = realValue;
			IsInfinity = isInfinity;
		}
		#region IComparable Members
		public int CompareTo(object obj) {
			IComparableObjectWrapper wrapper = obj as IComparableObjectWrapper;
			if (wrapper != null)
				return Value.CompareTo(wrapper.Value);
			return Value.CompareTo(obj);
		}
		#endregion
	}
	public static class TransformHelper {
		public static Rect GetElementBounds(FrameworkElement element, FrameworkElement relativeTo) {
			return GetTransform(element, relativeTo).TransformBounds(new Rect(new Point(0, 0), new Size(element.ActualWidth, element.ActualHeight)));
		}
		public static Point GetElementRelativeTopLeft(FrameworkElement element, FrameworkElement relativeTo) {
			return GetTransform(element, relativeTo).Transform(new Point(0, 0));
		}
		public static double GetElementWidth(FrameworkElement element) {
			return element == null ? 0d : element.ActualWidth;
		}
		public static double GetElementCenter(FrameworkElement element, FrameworkElement relativeTo) {
			return GetElementRelativeTopLeft(element, relativeTo).X + GetElementWidth(element) / 2;
		}
		private static GeneralTransform GetTransform(FrameworkElement element, FrameworkElement relativeTo) {
			return element.TransformToVisual(relativeTo);
		}
	}
	public enum AnimationTypes { Selection, Zoom, Stopped, Label, Scroll }
	public class AnimationEventArgs : EventArgs {
		public AnimationEventArgs(AnimationTypes type) {
			AnimationType = type;
		}
		public AnimationTypes AnimationType { get; set; }
		public FrameworkElement Target { get; set; }
	}
	public class RangeControlAnimator {
		public RangeControlAnimator() {
			AnimationDuration = GetDefaultDuration();
		}
		public Duration AnimationDuration { get; set; }
		public event EventHandler<AnimationEventArgs> AnimationCompleted;
		public void ResetDefault() {
			AnimationDuration = GetDefaultDuration();
		}
		public bool CanAnimate { get; set; }
		public bool IsProcessAnimation { get; set; }
		bool CanProcessAnimation { get; set; }
		int frames = 50;
		double Delay { get { return 1000 / frames; } }
		private void RaiseAnimationCompleted(AnimationEventArgs args) {
			if (!IsProcessAnimation) return;
			AnimationDuration = GetDefaultDuration();
			this.AnimationCompleted(this, args);
			IsProcessAnimation = false;
		}
		private Duration GetDefaultDuration() {
			return new Duration(TimeSpan.FromMilliseconds(100));
		}
		public void StopAnimation() {
			if (IsProcessAnimation) {
				CanProcessAnimation = false;
				StopTimer();
				RaiseAnimationCompleted(new AnimationEventArgs(AnimationTypes.Stopped));
			}
		}
		private void StopTimer() {
			if (timer != null)
				timer.Stop();
		}
		public void AnimateScroll(double from, double to, Action<double> action) {
			if (IsProcessAnimation) StopAnimation();
			double time = 250;
			double step = (to - from) / (time / Delay);
			double offset = from;
			int ticks = (int)(time / Delay);
			CreateTimer();
			timer.Tick += (s, e) => {
				ticks--;
				offset += step;
				action.Invoke(offset);
				if (ticks == 0) {
					StopTimer();
					AnimationDuration = GetDefaultDuration();
					action.Invoke(to);
					RaiseAnimationCompleted(new AnimationEventArgs(AnimationTypes.Scroll));
				}
			};
			IsProcessAnimation = true;
			timer.Start();
		}
		public void AnimateSelection(FrameworkElement thumb, FrameworkElement border, double borderPosition, double thumbPosition, RangeControl control) {
			CanProcessAnimation = true;
			Storyboard st = new Storyboard();
			st.FillBehavior = FillBehavior.Stop;
			st.Children.Add(CreateAnimation(border, borderPosition));
			st.Children.Add(CreateAnimation(thumb, thumbPosition));
			st.Completed += (s, e) => {
				thumb.SetValue(Canvas.LeftProperty, thumbPosition);
				border.SetValue(Canvas.LeftProperty, borderPosition);
				RaiseAnimationCompleted(new AnimationEventArgs(AnimationTypes.Selection));
			};
			IsProcessAnimation = true;
			st.Begin();
		}
		private DoubleAnimation CreateAnimation(FrameworkElement element, double position) {
			DoubleAnimation anim = new DoubleAnimation() {
				Duration = AnimationDuration,
				To = position.Round()
			};
			Storyboard.SetTarget(anim, element);
			Storyboard.SetTargetProperty(anim, new PropertyPath("(Canvas.Left)"));
			return anim;
		}
		DispatcherTimer timer;
		public void AnimateDoubleTapZoom(double fromStart, double fromEnd, double toStart, double toEnd, Action<double, double> predicate) {
			StopAnimation();
			CanProcessAnimation = true;
			int time = 400;
			int ticks =(int)(time / Delay);
			double startIncr = (toStart - fromStart) / ticks;
			double endIncr = (toEnd - fromEnd) / ticks;
			double start = fromStart;
			double end = fromEnd;
			SetDuration(time);
			CreateTimer();
			timer.Tick += (s, e) => {
				ticks--;
				start = start + startIncr;
				end = end + endIncr;
				predicate.Invoke(start, end);
				if (ticks == 0) {
					StopTimer();
					AnimationDuration = GetDefaultDuration();
					predicate.Invoke(toStart, toEnd);
					RaiseAnimationCompleted(new AnimationEventArgs(AnimationTypes.Zoom));
				}
			};
			IsProcessAnimation = true;
			timer.Start();
		}
		private void CreateTimer() {
			timer = new DispatcherTimer(DispatcherPriority.Render) { Interval = TimeSpan.FromMilliseconds(Delay) };
		}
		private Duration SetDuration(int time) {
			return AnimationDuration = new Duration(TimeSpan.FromMilliseconds(time));
		}
		public void AnimateShader(GrayScaleEffect effect, double left) {
			DoubleAnimation shaderAnim = new DoubleAnimation() {
				Duration = new Duration(TimeSpan.FromSeconds(2)),
				To = left
			};
			Storyboard.SetTarget(shaderAnim, effect);
			Storyboard.SetTargetProperty(shaderAnim, new PropertyPath(GrayScaleEffect.LeftProperty));
			Storyboard st = new Storyboard();
			st.Children.Add(shaderAnim);
			st.Completed += (s, e) => {
				effect.Left = left;
			};
			st.Begin();
		}
		internal void AnimateLabel(ContentPresenter label, double labelLeft) {
			CanProcessAnimation = true;
			Storyboard st = new Storyboard();
			st.FillBehavior = FillBehavior.Stop;
			st.Children.Add(CreateAnimation(label, labelLeft));
			st.Completed += (s, e) => {
				IsProcessAnimation = true;
				RaiseAnimationCompleted(new AnimationEventArgs(AnimationTypes.Label) { Target = label });
			};
			IsProcessAnimation = true;
			st.Begin();
		}
	}
	public class BoolToOpacityConverter : IValueConverter {
		#region IValueConverter Members
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			return (bool)value ? Double.Parse(parameter.ToString(), CultureInfo.InvariantCulture) : 0;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return ((double)value) > 0;
		}
		#endregion
	}
	public class BoolToGridLengthConverter : IValueConverter {
		#region IValueConverter Members
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			double pixelValue = 0;
			GridLength length;
			if (parameter.ToString() == "Auto")
				length = new GridLength(1, GridUnitType.Auto);
			else {
				Double.TryParse(parameter.ToString(), out pixelValue);
				length = new GridLength(pixelValue);
			}
			return (bool)value ? length : new GridLength(0);
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
		#endregion
	}
}
