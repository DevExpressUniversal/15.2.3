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
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Native;
using DevExpress.Xpf.Scheduler.Native;
using DevExpress.Xpf.Scheduler;
using DevExpress.Scheduler.Native;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Scheduler.Drawing;
using DevExpress.Utils.Serializing;
#if SL
using DevExpress.Xpf.Core;
#endif
namespace DevExpress.Xpf.Scheduler {
	public class GanttView : TimelineView, IGanttViewProperties {
		public GanttView() {
			DefaultStyleKey = typeof(GanttView);
		}
		protected internal new InnerGanttView InnerView { get { return (InnerGanttView)base.InnerView; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override SchedulerViewType Type { get { return SchedulerViewType.Gantt; } }
		protected internal override InnerSchedulerViewBase CreateInnerView() {
			return new WpfInnerGanttView(this, this);
		}
		protected internal override ViewFactoryHelper CreateFactoryHelper() {
			return new GanttViewFactoryHelper();
		}
	}
	public class WpfInnerGanttView : InnerGanttView {
		public WpfInnerGanttView(IInnerSchedulerViewOwner owner, IGanttViewProperties properties)
			: base(owner, properties) {
		}
		protected internal override void PopulateVisibleIntervalsCore(DateTime date) {
			GanttView view = (GanttView)Owner;
			TimeScaleIntervalCollection collection = (TimeScaleIntervalCollection)InnerVisibleIntervals;
			TimeScale scale = collection.Scale;
			DateTime currentDate = scale.Floor(date);
			int count = view.IntervalCount;
			for (int i = 0; i < count; i++) {
				InnerVisibleIntervals.Add(new TimeInterval(currentDate, TimeSpan.Zero));
				currentDate = scale.GetNextDate(currentDate);
			}
		}
	}
}
namespace DevExpress.Xpf.Scheduler.Drawing {
	#region GanttViewContentStyleSelector
	public class GanttViewContentStyleSelector : ViewContentStyleSelector {
		protected internal override Type GroupByNoneType { get { return typeof(VisualTimelineViewGroupByNone); } }
		protected internal override Type GroupByDateType { get { return typeof(VisualGanttViewGroupByDate); } }
		protected internal override Type GroupByResourceType { get { return typeof(VisualGanttViewGroupByDate); } }
	}
	#endregion
}
