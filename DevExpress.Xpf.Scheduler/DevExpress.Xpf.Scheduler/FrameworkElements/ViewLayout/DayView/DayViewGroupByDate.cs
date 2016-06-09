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
using System;
using System.Collections.ObjectModel;
using DevExpress.XtraScheduler.Drawing;
using System.Windows.Media;
using System.Windows.Controls;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler;
using System.Collections.Generic;
using System.ComponentModel;
#if SL
using DevExpress.Xpf.Core.WPFCompatibility;
#endif
namespace DevExpress.Xpf.Scheduler.Drawing {
	public class VisualDayViewGroupByDate : VisualIntervalsBasedViewInfo, IVisualDayView {
		public const string DayViewScrollViewerName = "PART_DayViewScrollViewer";
		ScrollViewer dayViewScrollViewer;
		static VisualDayViewGroupByDate() {
		}
		public VisualDayViewGroupByDate() {
			DefaultStyleKey = typeof(VisualDayViewGroupByDate);
		}
		#region ShowTimeRulerHeader
		public static readonly DependencyProperty ShowTimeRulerHeaderProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualDayViewGroupByDate, bool>("ShowTimeRulerHeader", true);
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("VisualDayViewGroupByDateShowTimeRulerHeader")]
#endif
		public bool ShowTimeRulerHeader { get { return (bool)GetValue(ShowTimeRulerHeaderProperty); } set { SetValue(ShowTimeRulerHeaderProperty, value); } }
		#endregion
		#region TimeRulers
		static readonly DependencyPropertyKey TimeRulersPropertyKey = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterReadOnlyProperty<VisualDayViewGroupByDate, VisualTimeRulerCollection>("TimeRulers", null);
		public static readonly DependencyProperty TimeRulersProperty = TimeRulersPropertyKey.DependencyProperty;
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("VisualDayViewGroupByDateTimeRulers")]
#endif
		public VisualTimeRulerCollection TimeRulers { get { return (VisualTimeRulerCollection)GetValue(TimeRulersProperty); } protected set { this.SetValue(TimeRulersPropertyKey, value); } }
		#endregion
		#region ShowTimeRulers
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("VisualDayViewGroupByDateShowTimeRulers")]
#endif
public bool ShowTimeRulers {
			get { return (bool)GetValue(ShowTimeRulersProperty); }
			private set { this.SetValue(ShowTimeRulersPropertyKey, value); }
		}
		internal static readonly DependencyPropertyKey ShowTimeRulersPropertyKey = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterReadOnlyProperty<VisualDayViewGroupByDate, bool>("ShowTimeRulers", false);
		public static readonly DependencyProperty ShowTimeRulersProperty = ShowTimeRulersPropertyKey.DependencyProperty;
		#endregion        
		#region AllDayAreaContainerGroups
		public VisualDayAllDayAreaContainerCollection AllDayAreaContainerGroups {
			get { return (VisualDayAllDayAreaContainerCollection)GetValue(AllDayAreaContainerGroupsProperty); }
			set { SetValue(AllDayAreaContainerGroupsProperty, value); }
		}
		public static readonly DependencyProperty AllDayAreaContainerGroupsProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualDayViewGroupByDate, VisualDayAllDayAreaContainerCollection>("AllDayAreaContainerGroups", null);
		#endregion
		#region MoreButtonsVisibility
		static readonly DependencyPropertyKey MoreButtonsVisibilityPropertyKey = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterReadOnlyProperty<VisualDayViewGroupByDate, Visibility>("MoreButtonsVisibility", Visibility.Collapsed);
		public static readonly DependencyProperty MoreButtonsVisibilityProperty = MoreButtonsVisibilityPropertyKey.DependencyProperty;
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("VisualDayViewGroupByDateMoreButtonsVisibility")]
#endif
public Visibility MoreButtonsVisibility { get { return (Visibility)GetValue(MoreButtonsVisibilityProperty); } protected set { this.SetValue(MoreButtonsVisibilityPropertyKey, value); } }
		#endregion
		#region ShowDayHeaders
		public static readonly DependencyProperty ShowDayHeadersProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualDayViewGroupByDate, bool>("ShowDayHeaders", true);
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("VisualDayViewGroupByDateShowDayHeaders")]
#endif
public bool ShowDayHeaders { get { return (bool)GetValue(ShowDayHeadersProperty); } set { SetValue(ShowDayHeadersProperty, value); } }
		#endregion
		#region TimeIndicatorVisibility
		public static readonly DependencyProperty TimeIndicatorVisibilityProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualDayViewGroupByDate, TimeIndicatorVisibility>("TimeIndicatorVisibility", TimeIndicatorVisibility.TodayView, (o, e) => o.OnTimeIndicatorVisibilityChanged(e.OldValue, e.NewValue));
		public TimeIndicatorVisibility TimeIndicatorVisibility {
			get { return (TimeIndicatorVisibility)GetValue(TimeIndicatorVisibilityProperty); }
			set { SetValue(TimeIndicatorVisibilityProperty, value); }
		}
		void OnTimeIndicatorVisibilityChanged(TimeIndicatorVisibility oldValue, TimeIndicatorVisibility newValue) {
			UpdateVisualInterval();
		}
		#endregion
		#region IVisualDayView Members
		ScrollViewer IVisualDayView.DayViewScrollViewer { get { return dayViewScrollViewer; } }
		#endregion
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			ScrollViewer dayViewScrollViewer = GetTemplateChild(DayViewScrollViewerName) as ScrollViewer;
			if (dayViewScrollViewer != null) {
				this.dayViewScrollViewer = dayViewScrollViewer;
			}
		}
		protected override VisualInterval CreateVisualInterval() {
			return new VisualDayViewInterval();
		}
		protected virtual VisualResourceAllDayArea CreateResourceAllDayArea() {
			return new VisualResourceAllDayArea();
		}
		protected virtual VisualDayAllDayAreaContainer CreatDayAllDayAreaContainer() {
			return new VisualDayAllDayAreaContainer();
		}
		protected override void CopyFromCore(ISchedulerViewInfoBase source) {
			base.CopyFromCore(source);
			DayViewGroupByDate groupByDate = source as DayViewGroupByDate;
			if (TimeRulers == null)
				TimeRulers = new VisualTimeRulerCollection();
			CollectionCopyHelper.Copy(TimeRulers, groupByDate.TimeRulers);
			if (Intervals == null)
				Intervals = new VisualIntervalsCollection();
			CollectionCopyHelper.Copy(Intervals, groupByDate.DaysContainers, CreateVisualInterval);
			if (AllDayAreaContainerGroups == null)
				AllDayAreaContainerGroups = new VisualDayAllDayAreaContainerCollection();
			CollectionCopyHelper.Copy(AllDayAreaContainerGroups, groupByDate.DaysContainers, CreatDayAllDayAreaContainer);
			MoreButtonsVisibility = groupByDate.MoreButtonsVisibility;
			ShowDayHeaders = groupByDate.ShowDayHeaders;
			ShowTimeRulers = groupByDate.ShowTimeRulers;
			TimeIndicatorVisibility = groupByDate.TimeIndicatorVisibility;
			UpdateVisualInterval();
		}		
		protected override void CopyAppointmentsViewInfoCore(ISchedulerViewInfoBase source) {
			base.CopyAppointmentsViewInfoCore(source);
			CopyVerticalAppointments(source);
			CopyHorizontalAppointments(source);
		}
		protected virtual void CopyVerticalAppointments(ISchedulerViewInfoBase source) {
			if (Intervals == null)
				return;
			DayViewGroupByDate groupBy = source as DayViewGroupByDate;
			int count = groupBy.DaysContainers.Count;
			for (int i = 0; i < count; i++)
				Intervals[i].CopyAppointmentsFrom(groupBy.DaysContainers[i]);
		}
		protected virtual void CopyHorizontalAppointments(ISchedulerViewInfoBase source) {
			if (AllDayAreaContainerGroups == null || AllDayAreaContainerGroups.Count <= 0)
				return;
			DayViewGroupByDate groupBy = source as DayViewGroupByDate;
			int count = groupBy.DaysContainers.Count;
			for (int i = 0; i < count; i++) {
				VisualDayAllDayAreaContainer allDayAreaGroup = AllDayAreaContainerGroups[i];
				VisualResourceAllDayAreaCollection allDayAreaContainers = allDayAreaGroup.AllDayAreaContainers;
				AssignableCollection<SingleResourceViewInfo> singleResourceViewInfoCollection = groupBy.DaysContainers[i].SingleResourceViewInfoCollection;
				int allDayAreaContainersCount = allDayAreaContainers.Count;
				for (int j = 0; j < allDayAreaContainersCount; j++) {					
					DayBasedSingleResourceViewInfo singleResourceViewInfo = (DayBasedSingleResourceViewInfo)singleResourceViewInfoCollection[j];
					allDayAreaContainers[j].CopyAppointmentsFrom(singleResourceViewInfo.HorizontalCellContainer);
				}
			}
		}		
		void UpdateVisualInterval() {
			if (Intervals == null)
				return;
			int count = Intervals.Count;
			for (int i = 0; i < count; i++) {
				VisualDayViewInterval interval = Intervals[i] as VisualDayViewInterval;
				interval.TimeIndicatorVisibility = TimeIndicatorVisibility;
			}
		}
	}
	public class DayViewGroupByDateHeaderTemplateSelector : DataTemplateSelector {
		public DataTemplate ShowDayHeadersTemplate { get; set; }
		public override DataTemplate SelectTemplate(object item, DependencyObject container) {
			VisualDayViewGroupByDate groupByDate = item as VisualDayViewGroupByDate;
			if (groupByDate.ShowDayHeaders)
				return ShowDayHeadersTemplate;
			return null;
		}
	}
}
