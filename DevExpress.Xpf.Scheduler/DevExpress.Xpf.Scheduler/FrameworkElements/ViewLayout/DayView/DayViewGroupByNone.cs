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
using System.ComponentModel;
#if SL
using DevExpress.Xpf.Core.WPFCompatibility;
#endif
namespace DevExpress.Xpf.Scheduler.Drawing {
	public interface IVisualDayView { 
		ScrollViewer DayViewScrollViewer { get; }
		TimeIndicatorVisibility TimeIndicatorVisibility { get; set; }
	}
	public abstract class VisualDayViewResourcesBasedViewInfo : VisualResourcesBasedViewInfo, IVisualDayView  {
		public const string DayViewScrollViewerName = "PART_DayViewScrollViewer";
		ScrollViewer dayViewScrollViewer;
		static VisualDayViewResourcesBasedViewInfo() {
		}
		protected VisualDayViewResourcesBasedViewInfo() {
			DefaultStyleKey = typeof(VisualDayViewResourcesBasedViewInfo);
		}
		#region ShowTimeRulerHeader
		public static readonly DependencyProperty ShowTimeRulerHeaderProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualDayViewResourcesBasedViewInfo, bool>("ShowTimeRulerHeader", true);
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("VisualDayViewResourcesBasedViewInfoShowTimeRulerHeader")]
#endif
		public bool ShowTimeRulerHeader { get { return (bool)GetValue(ShowTimeRulerHeaderProperty); } set { SetValue(ShowTimeRulerHeaderProperty, value); } }
		#endregion
		#region TimeRulers
		static readonly DependencyPropertyKey TimeRulersPropertyKey = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterReadOnlyProperty<VisualDayViewResourcesBasedViewInfo, VisualTimeRulerCollection>("TimeRulers", null);
		public static readonly DependencyProperty TimeRulersProperty = TimeRulersPropertyKey.DependencyProperty;
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("VisualDayViewResourcesBasedViewInfoTimeRulers")]
#endif
		public VisualTimeRulerCollection TimeRulers { get { return (VisualTimeRulerCollection)GetValue(TimeRulersProperty); } protected set { this.SetValue(TimeRulersPropertyKey, value); } }
		#endregion
		#region ShowTimeRulers
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("VisualDayViewResourcesBasedViewInfoShowTimeRulers")]
#endif
		public bool ShowTimeRulers {
			get { return (bool)GetValue(ShowTimeRulersProperty); }
			private set { this.SetValue(ShowTimeRulersPropertyKey, value); }
		}
		internal static readonly DependencyPropertyKey ShowTimeRulersPropertyKey = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterReadOnlyProperty<VisualDayViewResourcesBasedViewInfo, bool>("ShowTimeRulers", false);
		public static readonly DependencyProperty ShowTimeRulersProperty = ShowTimeRulersPropertyKey.DependencyProperty;
		#endregion
		#region AllDayAreaContainerGroups
		public VisualResourceAllDayAreaContainerGroupCollection AllDayAreaContainerGroups {
			get { return (VisualResourceAllDayAreaContainerGroupCollection)GetValue(AllDayAreaContainerGroupsProperty); }
			set { SetValue(AllDayAreaContainerGroupsProperty, value); }
		}
		public static readonly DependencyProperty AllDayAreaContainerGroupsProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualDayViewResourcesBasedViewInfo, VisualResourceAllDayAreaContainerGroupCollection>("AllDayAreaContainerGroups", null);
		#endregion        
		#region MoreButtonsVisibility
		static readonly DependencyPropertyKey MoreButtonsVisibilityPropertyKey = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterReadOnlyProperty<VisualDayViewResourcesBasedViewInfo, Visibility>("MoreButtonsVisibility", Visibility.Collapsed);
		public static readonly DependencyProperty MoreButtonsVisibilityProperty = MoreButtonsVisibilityPropertyKey.DependencyProperty;
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("VisualDayViewResourcesBasedViewInfoMoreButtonsVisibility")]
#endif
		public Visibility MoreButtonsVisibility { get { return (Visibility)GetValue(MoreButtonsVisibilityProperty); } protected set { this.SetValue(MoreButtonsVisibilityPropertyKey, value); } }
		#endregion
		#region ShowDayHeaders
		public static readonly DependencyProperty ShowDayHeadersProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualDayViewResourcesBasedViewInfo, bool>("ShowDayHeaders", true);
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("VisualDayViewResourcesBasedViewInfoShowDayHeaders")]
#endif
		public bool ShowDayHeaders { get { return (bool)GetValue(ShowDayHeadersProperty); } set { SetValue(ShowDayHeadersProperty, value); } }
		#endregion
		#region TimeIndicatorVisibility
		public static readonly DependencyProperty TimeIndicatorVisibilityProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualDayViewResourcesBasedViewInfo, TimeIndicatorVisibility>("TimeIndicatorVisibility", TimeIndicatorVisibility.TodayView, (o, e) => o.OnTimeIndicatorVisibilityChanged(e.OldValue, e.NewValue));
		void OnTimeIndicatorVisibilityChanged(TimeIndicatorVisibility oldValue, TimeIndicatorVisibility newValue) {
			if (ResourceContainers == null)
				return;
			int count = ResourceContainers.Count;
			for (int i = 0; i < count; i++) {
				VisualDayViewResource visualResource = ResourceContainers[i] as VisualDayViewResource;
				visualResource.TimeIndicatorVisibility = newValue;
			}
		}
		public TimeIndicatorVisibility TimeIndicatorVisibility {
			get { return (TimeIndicatorVisibility)GetValue(TimeIndicatorVisibilityProperty); }
			set { SetValue(TimeIndicatorVisibilityProperty, value); }
		}
		#endregion
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			ScrollViewer dayViewScrollViewer = GetTemplateChild(DayViewScrollViewerName) as ScrollViewer;
			if (dayViewScrollViewer != null) {
				this.dayViewScrollViewer = dayViewScrollViewer;
			}
		}
		protected override VisualResource CreateVisualResource() {
			return new VisualDayViewResource();
		}
		protected override void CopyFromCore(ISchedulerViewInfoBase source) {
			base.CopyFromCore(source);
			DayViewInfoBase dayViewInfo = source as DayViewInfoBase;
			if (TimeRulers == null)
				TimeRulers = new VisualTimeRulerCollection();
			CollectionCopyHelper.Copy(TimeRulers, dayViewInfo.TimeRulers);
			if (ResourceContainers == null)
				ResourceContainers = new VisualResourcesCollection();
			CollectionCopyHelper.Copy(ResourceContainers, dayViewInfo.ResourcesContainers, CreateVisualResource);
			UpdateTimeIndicatorVisibility(ResourceContainers);
			UpdateAllDayAreaContainerGroups(ResourceContainers);			
			MoreButtonsVisibility = dayViewInfo.MoreButtonsVisibility;
			ShowDayHeaders = dayViewInfo.ShowDayHeaders;
			ShowTimeRulers = dayViewInfo.ShowTimeRulers;
			ShowTimeRulerHeader = dayViewInfo.ShowTimeRulerHeader;
			TimeIndicatorVisibility = dayViewInfo.TimeIndicatorVisibility;
		}
		void UpdateAllDayAreaContainerGroups(VisualResourcesCollection resourceContainers) {
			if (AllDayAreaContainerGroups == null)
				AllDayAreaContainerGroups = CreateVisualResourceAllDayAreaContainerGroups();
			VisualResourceAllDayAreaContainerGroup allDayAreaGroup = AllDayAreaContainerGroups.First;
			if (allDayAreaGroup == null)
				return;
			allDayAreaGroup.View = View;
			allDayAreaGroup.CopyFrom(resourceContainers);
		}
		VisualResourceAllDayAreaContainerGroupCollection CreateVisualResourceAllDayAreaContainerGroups() {
			VisualResourceAllDayAreaContainerGroupCollection result = new VisualResourceAllDayAreaContainerGroupCollection();
			result.Add(new VisualResourceAllDayAreaContainerGroup());
			return result;
		}
		void UpdateTimeIndicatorVisibility(VisualResourcesCollection resourceContainers) {
			int count = resourceContainers.Count;
			for (int i = 0; i < count; i++) {
				VisualDayViewResource visualResource = resourceContainers[i] as VisualDayViewResource;
				visualResource.TimeIndicatorVisibility = TimeIndicatorVisibility;
			}
		}
		#region IVisualDayView Members
		ScrollViewer IVisualDayView.DayViewScrollViewer { get { return dayViewScrollViewer; } }
		#endregion
	}
	public class VisualDayViewGroupByNone : VisualDayViewResourcesBasedViewInfo {
		public VisualDayViewGroupByNone() {
			DefaultStyleKey = typeof(VisualDayViewGroupByNone);
		}
		protected override void CopyFromCore(ISchedulerViewInfoBase source) {
			base.CopyFromCore(source);
			DayViewGroupByNone groupByNone = source as DayViewGroupByNone;
			ShowTimeRulerHeader = groupByNone.ShowTimeRulerHeader;
		}
		protected override void CopyAppointmentsViewInfoCore(ISchedulerViewInfoBase source) {
			base.CopyAppointmentsViewInfoCore(source);
			if (ResourceContainers == null)
				return;
			DayViewGroupByNone groupByNone = source as DayViewGroupByNone;
			int count = groupByNone.ResourcesContainers.Count;
			for (int i = 0; i < count; i++)
				ResourceContainers[i].CopyAppointmentsFrom(groupByNone.ResourcesContainers[i]);
		}	  
	}	
	public class ObservableStringCollection : ObservableCollection<String> {
	}
}
