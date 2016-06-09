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

using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Bars.Native;
using DevExpress.Xpf.Scheduler.Commands;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Commands;
using DevExpress.XtraScheduler.Native;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
namespace DevExpress.Xpf.Scheduler.UI {
	public class TimeScaleBarSubItem : SubMenuBarSubItem {
		protected override void UpdateItemsCore() {
			SchedulerViewType viewType = SchedulerControl.ActiveViewType;
			if (viewType == SchedulerViewType.Timeline || viewType == SchedulerViewType.Gantt)
				PopulatePopupMenuFromTimelineScales(ItemLinks);
			else if (viewType == SchedulerViewType.Day || viewType == SchedulerViewType.WorkWeek || viewType == SchedulerViewType.FullWeek)
				PopulatePopupMenuFromDayViewScales(ItemLinks);	
		}
		void PopulatePopupMenuFromDayViewScales(BarItemLinkCollection itemLinks) {
			DayView view = SchedulerControl.ActiveView as DayView;
			if (view == null)
				return;
			TimeSlotCollection slots = view.InnerView.TimeSlots;
			if (slots == null)
				return;
			foreach (TimeSlot timeSlot in slots) {
				BarCheckItem item = CreateBarCheckItem(new SetDayViewTimeScaleUICommand(timeSlot));
				itemLinks.Add(item);
			}
		}
		void PopulatePopupMenuFromTimelineScales(BarItemLinkCollection itemLinks) {
			TimelineView view = SchedulerControl.ActiveView as TimelineView;
			if (view == null)
				return;
			InnerTimelineView innerView = (InnerTimelineView)view.InnerView;
			IList<TimeScale> scales = innerView.Scales;
			if (scales == null)
				return;
			foreach (TimeScale timeScale in scales) {
				BarCheckItem item = CreateBarCheckItem(new SetTimeScaleEnableUICommand(timeScale));
				itemLinks.Add(item);
			}
		}
		BarCheckItem CreateBarCheckItem(SchedulerUICommand command) {
			BarCheckItem item = new BarCheckItem();
			item.Command = command;
			item.CommandParameter = SchedulerControl;
			item.IsPrivate = true;
			SchedulerControl.CommandManager.UpdateBarItemDefaultValues(item);
			SchedulerControl.CommandManager.UpdateBarItemCommandUIState(item, command.CreateCommand(SchedulerControl));
			return item;
		}
	}
}
