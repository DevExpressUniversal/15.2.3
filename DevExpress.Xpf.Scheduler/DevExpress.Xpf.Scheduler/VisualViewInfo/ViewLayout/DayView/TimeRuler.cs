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

using System.Windows;
using System.Collections.ObjectModel;
using System;
using DevExpress.XtraScheduler.Drawing;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Native;
using System.ComponentModel;
using DevExpress.Utils;
using DevExpress.XtraScheduler.Internal.Implementations;
#if SL
using DevExpress.Xpf.Core.WPFCompatibility;
#endif
namespace DevExpress.Xpf.Scheduler {
	public class SchedulerTimeRuler : TimeRuler {
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool AlwaysShowTopRowTime {
			get {
				return base.AlwaysShowTopRowTime;
			}
			set {
				base.AlwaysShowTopRowTime = value;
			}
		}
	}
	public class SchedulerTimeRulerCollection : NotificationCollection<SchedulerTimeRuler> {
	}
}
namespace DevExpress.Xpf.Scheduler.Drawing {
	#region VisualTimeRuler
	public class VisualTimeRuler : DependencyObject, ISupportCopyFrom<TimeRulerViewInfo>, ISupportHitTest, ISelectableIntervalViewInfo {
		TimeZoneHelper timeZoneEngine;
		TimeRuler timeRuler;
		#region Caption
		public static readonly DependencyProperty CaptionProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualTimeRuler, string>("Caption", String.Empty);
		public string Caption { get { return (string)GetValue(CaptionProperty); } set { SetValue(CaptionProperty, value); } }
		#endregion
		#region TimeZone
		public static readonly DependencyProperty TimeZoneProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualTimeRuler, TimeZoneInfo>("TimeZone", null, (d, e) => d.OnTimeZoneChanged());
		public TimeZoneInfo TimeZone { get { return (TimeZoneInfo)GetValue(TimeZoneProperty); } set { SetValue(TimeZoneProperty, value); } }
		private void OnTimeZoneChanged() {
			RaiseTimeZoneChanged(EventArgs.Empty);
		}
		#endregion
		#region IntervalStart
		public static readonly DependencyProperty IntervalStartProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualTimeRuler, TimeSpan>("IntervalStart", TimeSpan.Zero);
		public TimeSpan IntervalStart { get { return (TimeSpan)GetValue(IntervalStartProperty); } set { SetValue(IntervalStartProperty, value); } }
		#endregion
		#region IntervalEnd
		public static readonly DependencyProperty IntervalEndProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualTimeRuler, TimeSpan>("IntervalEnd", TimeSpan.Zero);
		public TimeSpan IntervalEnd { get { return (TimeSpan)GetValue(IntervalEndProperty); } set { SetValue(IntervalEndProperty, value); } }
		#endregion
		#region Elements
		static readonly DependencyPropertyKey ElementsPropertyKey = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterReadOnlyProperty<VisualTimeRuler, VisualTimeRulerElementCollection>("Elements", null);
		public static readonly DependencyProperty ElementsProperty = ElementsPropertyKey.DependencyProperty;
		public VisualTimeRulerElementCollection Elements { get { return (VisualTimeRulerElementCollection)GetValue(ElementsProperty); } protected set { this.SetValue(ElementsPropertyKey, value); } }
		#endregion
		#region AllMinuteItems
		static readonly DependencyPropertyKey AllMinuteItemsPropertyKey = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterReadOnlyProperty<VisualTimeRuler, VisualTimeRulerMinuteItemCollection>("AllMinuteItems", null);
		public static readonly DependencyProperty AllMinuteItemsProperty = AllMinuteItemsPropertyKey.DependencyProperty;
		public VisualTimeRulerMinuteItemCollection AllMinuteItems { get { return (VisualTimeRulerMinuteItemCollection)GetValue(AllMinuteItemsProperty); } protected set { this.SetValue(AllMinuteItemsPropertyKey, value); } }
		#endregion
		#region View
		public DayView View { get { return (DayView)GetValue(ViewProperty); } set { SetValue(ViewProperty, value); } }
		public static readonly DependencyProperty ViewProperty = CreateViewProperty();
		static DependencyProperty CreateViewProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualTimeRuler, DayView>("View", null);
		}
		#endregion
		#region TimeMarkerVisibility
		bool lastSettedTimeMarkerVisibility = true;
		public static readonly DependencyProperty TimeMarkerVisibilityProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualTimeRuler, bool>("TimeMarkerVisibility", true, (d, e) => d.OnTimeMarkerVisibility(e.OldValue, e.NewValue));
		public bool TimeMarkerVisibility {
			get { return (bool)GetValue(TimeMarkerVisibilityProperty); }
			set {
				if (lastSettedTimeMarkerVisibility == value)
					return;
				SetValue(TimeMarkerVisibilityProperty, value);
			}
		}
		void OnTimeMarkerVisibility(bool oldValue, bool newValue) {
			this.lastSettedTimeMarkerVisibility = newValue;
		}
		#endregion
		public virtual SchedulerHitTest HitTestType { get { return SchedulerHitTest.Ruler; } }
		public Style CurrentTimeIndicatorStyle { get { return View.CurrentTimeIndicatorStyle; } }
		#region Events
		#region TimeZoneChanged
		EventHandler onTimeZoneChanged;
		public event EventHandler TimeZoneChanged {
			add { onTimeZoneChanged += value; }
			remove { onTimeZoneChanged -= value; }
		}
		protected virtual void RaiseTimeZoneChanged(EventArgs e) {
			EventHandler handler = onTimeZoneChanged;
			if (handler != null)
				handler(this, e);
		}
		#endregion
		#endregion
		void ISupportCopyFrom<TimeRulerViewInfo>.CopyFrom(TimeRulerViewInfo timeRulerViewInfo) {
			TimeRulerHeader header = (TimeRulerHeader)timeRulerViewInfo.Header;
			Caption = header != null ? header.Caption : string.Empty;
			if (Elements == null)
				Elements = new VisualTimeRulerElementCollection();
			CollectionCopyHelper.Copy(Elements, timeRulerViewInfo.Elements);
			if (AllMinuteItems == null)
				AllMinuteItems = new VisualTimeRulerMinuteItemCollection();
			CollectionCopyHelper.Copy(AllMinuteItems, timeRulerViewInfo.AllMinuteItems);
			IntervalStart = timeRulerViewInfo.Interval.Start;
			IntervalEnd = timeRulerViewInfo.Interval.End;
			this.timeZoneEngine = timeRulerViewInfo.TimeZoneHelper;
			TimeRuler timeRuler = timeRulerViewInfo.TimeRuler;
			this.timeRuler = timeRuler;
			View = timeRulerViewInfo.View;
			TimeZone = timeRulerViewInfo.TimeZoneHelper.ClientTimeZone;
			TimeMarkerVisibility = timeRulerViewInfo.IsTimeMarkerVisibility;
		}
		public TimeZoneHelper GetTimeZoneHelper() {
			return timeZoneEngine;
		}
		public TimeRuler GetTimeRuler() {
			return timeRuler;
		}
		#region ISupportHitTest Members
		public XtraScheduler.Native.ISelectableIntervalViewInfo GetSelectableIntervalViewInfo(SchedulerControl control) {
			return new VisualSelectableIntervalViewInfo(SchedulerHitTest.Ruler, TimeInterval.Empty, ResourceBase.Empty, false);
		}
		#endregion
		#region ISelectableIntervalViewInfo Members
		public TimeInterval Interval {
			get { return TimeInterval.Empty; }
		}
		public Resource Resource {
			get { return ResourceBase.Empty; }
		}
		public bool Selected {
			get { return false; }
		}
		#endregion
	}
	#endregion
	#region VisualTimeRulerCollection
	public class VisualTimeRulerCollection : ObservableCollection<VisualTimeRuler> {
	}
	#endregion
	public abstract class VisualTimeRulerItem : DependencyObject {
		#region Caption
		String lastSettedCaption = String.Empty;
		public static readonly DependencyProperty CaptionProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualTimeRulerItem, string>("Caption", String.Empty, (d, e) => d.OnCaptionChanged(e.OldValue, e.NewValue));
		public string Caption { get { return (string)GetValue(CaptionProperty); } set { if (lastSettedCaption != value) SetValue(CaptionProperty, value); } }
		protected virtual void OnCaptionChanged(string oldCaption, string newCaption) {
			lastSettedCaption = newCaption;
		}
		#endregion
		#region Format
		String lastSettedFormat = String.Empty;
		public static readonly DependencyProperty FormatProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualTimeRulerItem, string>("Format", String.Empty, (d, e) => d.OnFormatChanged(e.OldValue, e.NewValue));
		public string Format { get { return (string)GetValue(FormatProperty); } set { if (lastSettedFormat != value)SetValue(FormatProperty, value); } }
		protected virtual void OnFormatChanged(string oldFormat, string newFormat) {
			lastSettedFormat = newFormat;
		}
		#endregion
		#region Time
		DateTime lastSettedTime = DateTime.MinValue;
		public static readonly DependencyProperty TimeProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualTimeRulerItem, DateTime>("Time", DateTime.MinValue, (d, e) => d.OnTimeChanged(e.OldValue, e.NewValue));
		public DateTime Time { get { return (DateTime)GetValue(TimeProperty); } set { if (lastSettedTime != value)SetValue(TimeProperty, value); } }
		protected virtual void OnTimeChanged(DateTime oldTime, DateTime newTime) {
			lastSettedTime = newTime;
		}
		#endregion
		protected virtual void CopyFromCore(TimeRulerItem source) {
			this.Caption = source.Caption;
			this.Format = source.Format;
			this.Time = source.Time;
		}
	}
	#region VisualTimeRulerElement
	public class VisualTimeRulerElement : VisualTimeRulerItem, ISupportCopyFrom<TimeRulerElement> {
		#region MinuteItems
		static readonly DependencyPropertyKey MinuteItemsPropertyKey = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterReadOnlyProperty<VisualTimeRulerElement, VisualTimeRulerMinuteItemCollection>("MinuteItems", null);
		public static readonly DependencyProperty MinuteItemsProperty = MinuteItemsPropertyKey.DependencyProperty;
		public VisualTimeRulerMinuteItemCollection MinuteItems { get { return (VisualTimeRulerMinuteItemCollection)GetValue(MinuteItemsProperty); } protected set { this.SetValue(MinuteItemsPropertyKey, value); } }
		#endregion
		#region ISupportCopyFrom<TimeRulerElement> Members
		void ISupportCopyFrom<TimeRulerElement>.CopyFrom(TimeRulerElement source) {
			if (MinuteItems == null)
				MinuteItems = new VisualTimeRulerMinuteItemCollection();
			CollectionCopyHelper.Copy(MinuteItems, source.MinuteItems);
			this.Caption = source.HourItem.Caption;
			this.Format = source.HourItem.Format;
			this.Time = source.HourItem.Time;
		}
		#endregion
	}
	#endregion
	#region VisualTimeRulerElementCollection
	public class VisualTimeRulerElementCollection : ObservableCollection<VisualTimeRulerElement> {
	}
	#endregion
	#region VisualTimeRulerMinuteItem
	public class VisualTimeRulerMinuteItem : VisualTimeRulerItem, ISupportCopyFrom<TimeRulerMinuteItem> {
		#region ISupportCopyFrom<TimeRulerMinuteItem> Members
		void ISupportCopyFrom<TimeRulerMinuteItem>.CopyFrom(TimeRulerMinuteItem source) {
			CopyFromCore(source);
		}
		#endregion
	}
	#endregion
	#region VisualTimeRulerMinuteItemCollection
	public class VisualTimeRulerMinuteItemCollection : ObservableCollection<VisualTimeRulerMinuteItem> {
	}
	#endregion
}
