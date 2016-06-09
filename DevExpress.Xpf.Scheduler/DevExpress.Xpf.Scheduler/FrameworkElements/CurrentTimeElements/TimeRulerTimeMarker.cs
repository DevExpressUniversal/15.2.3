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
using DependencyPropertyHelper = DevExpress.Xpf.Core.Native.DependencyPropertyHelper;
using DevExpress.XtraScheduler;
using DevExpress.Xpf.Scheduler.Drawing.Native;
using DevExpress.Xpf.Scheduler.Native;
namespace DevExpress.Xpf.Scheduler.Drawing {
	public class TimeRulerTimeMarker : Decorator {
		const double RightIndent = 1;
		public TimeRulerTimeMarker() {
			this.Loaded += OnLoaded;
			this.Unloaded += OnUnloaded;
		}
		#region TimeRuler
		public static readonly DependencyProperty TimeRulerProperty = CreateTimeRulerProperty();
		static DependencyProperty CreateTimeRulerProperty() {
			return DependencyPropertyHelper.RegisterProperty<TimeRulerTimeMarker, VisualTimeRuler>("TimeRuler", null, (d, e) => d.OnTimeRulerChanged(e.NewValue));
		}
		protected internal virtual void OnTimeRulerChanged(VisualTimeRuler newValue) {
			newValue.TimeZoneChanged -= new EventHandler(OnTimeZoneChanged);
			newValue.TimeZoneChanged += new EventHandler(OnTimeZoneChanged);
		}
		public VisualTimeRuler TimeRuler {
			get { return (VisualTimeRuler)GetValue(TimeRulerProperty); }
			set { SetValue(TimeRulerProperty, value); }
		}
		#endregion
		DateTime Now { get; set; }
		protected internal virtual void OnTimeZoneChanged(object sender, EventArgs e) {
			InvalidateArrange();
		}
		void OnTimerTick(object sender, TimeIndicatorTickEventArgs e) {
			Now = e.Now;
			InvalidateArrange();
		}
		protected internal virtual void OnLoaded(object sender, RoutedEventArgs e) {
			TimeRuler.TimeZoneChanged -= new EventHandler(OnTimeZoneChanged);
			TimeRuler.TimeZoneChanged += new EventHandler(OnTimeZoneChanged);
			Now = TimeIndicatorTimerService.Instance.Now;
			InvalidateArrange();
			TimeIndicatorTimerService.Instance.TimerTick += OnTimerTick;
		}
		protected internal virtual void OnUnloaded(object sender, RoutedEventArgs e) {
			TimeRuler.TimeZoneChanged -= new EventHandler(OnTimeZoneChanged);
			TimeIndicatorTimerService.Instance.TimerTick -= OnTimerTick;
		}
		protected override Size ArrangeOverride(Size arrangeBounds) {
			if (Child != null && TimeRuler != null) {
				double childHeight = Child.DesiredSize.Height;
				double childWidth = Child.DesiredSize.Width;
				double x = arrangeBounds.Width - childWidth - RightIndent;
				double pos = CalcIndicatorPosition(arrangeBounds);
				double y = pos - childHeight / 2;
				Child.Arrange(new Rect(new Point(x, y), Child.DesiredSize));
			}
			return arrangeBounds;
		}
		protected internal virtual double CalcIndicatorPosition(Size arrangeBounds) {
			TimeSpan clientNowTimeOfDay = CalcTimeOfDay(GetClientNowTime());
			TimeOfDayInterval timeOfDayInterval = new TimeOfDayInterval(TimeRuler.IntervalStart, TimeRuler.IntervalEnd);
			return SchedulerRectUtils.CalcDateY(clientNowTimeOfDay, timeOfDayInterval, arrangeBounds);
		}
		DateTime GetClientNowTime() {
			DateTime nowTime = Now;
			TimeZoneHelper timeZoneEngine = TimeRuler.GetTimeZoneHelper();
			if (timeZoneEngine == null)
				return nowTime;
			return TimeZoneInfo.ConvertTime(nowTime, TimeZoneInfo.Local, timeZoneEngine.ClientTimeZone);
		}
		protected internal virtual TimeSpan CalcTimeOfDay(DateTime date) {
			TimeSpan result = date.TimeOfDay;
			if (result < TimeRuler.IntervalStart)
				result += DevExpress.XtraScheduler.Native.DateTimeHelper.DaySpan;
			return result;
		}
	}	
}
