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
using System.Windows.Controls;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Native;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Scheduler.Native;
using DevExpress.Xpf.Core.Native;
using DevExpress.Utils;
using System.Diagnostics;
using System.Collections.ObjectModel;
namespace DevExpress.Xpf.Scheduler {
	#region SchedulerViewRepository
	public class SchedulerViewRepository : SchedulerViewTypedRepositoryBase<SchedulerViewBase> {
		ObservableCollection<SchedulerViewBase> xpfViews = new ObservableCollection<SchedulerViewBase>();
		#region Properties
		#region DayView
		[
#if !SL
	DevExpressXpfSchedulerLocalizedDescription("SchedulerViewRepositoryDayView"),
#endif
Category(SRCategoryNames.View), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		public DayView DayView { get { return (DayView)this[SchedulerViewType.Day]; } }
		#endregion
		#region WorkWeekView
		[
#if !SL
	DevExpressXpfSchedulerLocalizedDescription("SchedulerViewRepositoryWorkWeekView"),
#endif
		Category(SRCategoryNames.View), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		public WorkWeekView WorkWeekView { get { return (WorkWeekView)this[SchedulerViewType.WorkWeek]; } }
		#endregion
		#region WeekView
		[
#if !SL
	DevExpressXpfSchedulerLocalizedDescription("SchedulerViewRepositoryWeekView"),
#endif
Category(SRCategoryNames.View), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		public WeekView WeekView { get { return (WeekView)this[SchedulerViewType.Week]; } }
		#endregion
		#region FullWeekView
		[
#if !SL
	DevExpressXpfSchedulerLocalizedDescription("SchedulerViewRepositoryFullWeekView"),
#endif
		Category(SRCategoryNames.View), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		public FullWeekView FullWeekView { get { return (FullWeekView)this[SchedulerViewType.FullWeek]; } }
		#endregion
		#region MonthView
		[
#if !SL
	DevExpressXpfSchedulerLocalizedDescription("SchedulerViewRepositoryMonthView"),
#endif
Category(SRCategoryNames.View), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		public MonthView MonthView { get { return (MonthView)this[SchedulerViewType.Month]; } }
		#endregion
		#region TimelineView
		[
#if !SL
	DevExpressXpfSchedulerLocalizedDescription("SchedulerViewRepositoryTimelineView"),
#endif
Category(SRCategoryNames.View), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		public TimelineView TimelineView { get { return (TimelineView)this[SchedulerViewType.Timeline]; } }
		#endregion
		#region GanttView
		[Category(SRCategoryNames.View)]
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("SchedulerViewRepositoryGanttView")]
#endif
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		public GanttView GanttView { get { return (GanttView)this[SchedulerViewType.Gantt]; } }
		#endregion
#if SL
public ObservableCollection<SchedulerViewBase> XpfViews { get { return xpfViews; } }
#endif
		#endregion
		protected internal override void CreateViews(InnerSchedulerControl control) {
			RegisterView(new DayView());
			RegisterView(new WorkWeekView());
			RegisterView(new FullWeekView());
			RegisterView(new WeekView());
			RegisterView(new MonthView());
			RegisterView(new TimelineView());
		}
		protected internal virtual SchedulerViewBase ReplaceView(SchedulerControl control, SchedulerViewBase newView) {
			Guard.ArgumentNotNull(control, "control");
			Guard.ArgumentNotNull(newView, "newView");
			SchedulerViewBase oldView = Views[newView.Type];
			if (Object.ReferenceEquals(oldView, newView))
				return oldView;
			this.Views.Remove(oldView);
			this.Views.Add(newView);
#if SL
			XpfViews.Remove(oldView);
			XpfViews.Add(newView);
#endif
			InnerSchedulerViewBase oldInnerView = oldView.DetachExistingInnerView();
			control.BeginUpdate(); 
			try {
				newView.SynchronizeToInnerView(oldInnerView);
				newView.AttachExistingInnerView(control, oldInnerView); 
				control.OnAfterActiveViewChangeCore();
			}
			finally {
				control.EndUpdate();
			}
			return oldView;
		}
		protected internal virtual SchedulerViewCollection<SchedulerViewBase> GetViews() {
			return Views;
		}
#if SL
		protected internal override void RegisterView(SchedulerViewBase view) {
			base.RegisterView(view);
			XpfViews.Add(view);
		}
		protected internal override void UnregisterView(SchedulerViewBase view) {
			base.UnregisterView(view);
			XpfViews.Remove(view);
		}
#endif
	}
	#endregion
}
