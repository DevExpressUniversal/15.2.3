#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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

using DevExpress.XtraScheduler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace DevExpress.ExpressApp.Scheduler.Win {
	public class SchedulerControlModelSynchronizer {
		public static void ApplyModel(IModelListViewScheduler model, SchedulerControl scheduler) {
			scheduler.ActiveViewType = model.SchedulerViewType;
			scheduler.DayView.ResourcesPerPage = model.VisibleResourcesCount;
			scheduler.WorkWeekView.ResourcesPerPage = model.VisibleResourcesCount;
			scheduler.WeekView.ResourcesPerPage = model.VisibleResourcesCount;
			scheduler.MonthView.ResourcesPerPage = model.VisibleResourcesCount;
			scheduler.TimelineView.ResourcesPerPage = model.VisibleResourcesCount;
			if(model.TimeScale != TimeSpan.Zero) {
				scheduler.DayView.TimeScale = model.TimeScale;
				scheduler.WorkWeekView.TimeScale = model.TimeScale;
			}
			TimeInterval visibleInterval = model.SelectedIntervalStart < model.SelectedIntervalEnd ? new TimeInterval(model.SelectedIntervalStart, model.SelectedIntervalEnd) : null;
			if(visibleInterval != null) {
				TimeIntervalCollection timeIntervalCollection;
				if(scheduler.ActiveViewType == SchedulerViewType.Day) {
					timeIntervalCollection = new DayIntervalCollection();
				}
				else {
					timeIntervalCollection = new TimeIntervalCollection();
				}
				timeIntervalCollection.Add(visibleInterval);
				scheduler.ActiveView.SetVisibleIntervals(timeIntervalCollection);
			}
			scheduler.OptionsBehavior.RecurrentAppointmentEditAction = ((IModelOptionsSchedulerModule)((Model.IModelNode)model).Application.Options).SchedulerModule.RecurrentAppointmentEditAction;
		}
		public static void SaveModel(IModelListViewScheduler model, SchedulerControl scheduler) {
			model.SchedulerViewType = scheduler.ActiveViewType;
			model.VisibleResourcesCount = scheduler.ActiveView.ResourcesPerPage;
			if(scheduler.ActiveViewType == SchedulerViewType.Day) {
				model.TimeScale = scheduler.DayView.TimeScale;
			}
			else if(scheduler.ActiveViewType == SchedulerViewType.WorkWeek) {
				model.TimeScale = scheduler.WorkWeekView.TimeScale;
			}
			else {
				model.TimeScale = TimeSpan.Zero;
			}
			TimeInterval visibleInterval = scheduler.ActiveView.GetVisibleIntervals().Interval;
			if(visibleInterval != null) {
				model.SelectedIntervalStart = visibleInterval.Start;
				model.SelectedIntervalEnd = visibleInterval.End;
			}
			else {
				model.SelectedIntervalStart = default(DateTime);
				model.SelectedIntervalEnd = default(DateTime);
			}
		}
	}
}
