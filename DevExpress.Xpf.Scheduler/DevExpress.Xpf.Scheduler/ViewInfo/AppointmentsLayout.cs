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
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Drawing;
using DevExpress.Utils;
using DevExpress.XtraScheduler.Native;
#if WPF||SL
using DevExpress.Xpf.Scheduler;
using DevExpress.XtraScheduler.Internal.Diagnostics;
using DevExpress.XtraScheduler.Drawing.Internal;
#endif
namespace DevExpress.Xpf.Scheduler.Drawing {
	public class AppointmentsLayoutCalculator {
		public AppointmentsLayoutCalculator(SchedulerViewBase view) {
			Guard.ArgumentNotNull(view, "view");
			View = view;
		}
		#region Properties
		protected internal SchedulerViewBase View { get; private set; }
		#endregion
		public virtual void LayoutDraggedAppointments() {
			View.ViewInfo.DraggedAppointmentControls.Clear();
			AppointmentBaseCollection draggedAppointment = GetDraggedAppointments();
			AppointmentBaseCollection nonpermanentAppointments = new AppointmentBaseCollection();
			LayoutAppointmentsCore(View.ViewInfo.DraggedAppointmentControls, draggedAppointment, nonpermanentAppointments, false);
			CellContainerCollection containers = View.ViewInfo.GetContainers();
			foreach(ICellContainer container in containers) {
				container.FillDraggedAppointmentControls(View.ViewInfo.DraggedAppointmentControls);
			}
		}
		public virtual AppointmentControl LayoutAppointment(Appointment appointment) {
			AppointmentBaseCollection appointments = new AppointmentBaseCollection();
			AppointmentControlCollection appointmentControls = new AppointmentControlCollection();
			appointments.Add(appointment);
			LayoutAppointmentsCore(appointmentControls, appointments, null, false);
			XtraSchedulerDebug.Assert(appointmentControls.Count == 1);
			return appointmentControls[0];
		}
		public virtual void LayoutAppointments() {
			View.ViewInfo.AppointmentControls.Clear();
			AppointmentBaseCollection permanentAppointments = GetPermanentAppointments();
			AppointmentBaseCollection nonpermanentAppointments = GetNonPermanentAppointments();
			LayoutAppointmentsCore(View.ViewInfo.AppointmentControls, permanentAppointments, nonpermanentAppointments, false);
			CellContainerCollection containers = View.ViewInfo.GetContainers();
			foreach (ICellContainer container in containers) {
				container.FillAppointmentControls(View.ViewInfo.AppointmentControls);
			}
		}
		protected internal virtual AppointmentBaseCollection GetAppointmentns() {
			AppointmentBaseCollection collection = new AppointmentBaseCollection();
			collection.AddRange(GetPermanentAppointments());
			collection.AddRange(GetNonPermanentAppointments());
			return collection;
		}
		protected internal virtual AppointmentBaseCollection GetPermanentAppointments() {
			return View.Control.AppointmentChangeHelper.GetActualVisibleAppointments(View.FilteredAppointments);
		}
		protected internal virtual AppointmentBaseCollection GetDraggedAppointments() {
			if(View.Control.DragDropOptions.MovementType == MovementType.SnapToCells)
				return new AppointmentBaseCollection();
			AppointmentChangeHelper appointmentChangeHelper = View.Control.AppointmentChangeHelper;
			if (!(appointmentChangeHelper.State is DragAppointmentChangeHelperState))
				return new AppointmentBaseCollection();
			return View.Control.AppointmentChangeHelper.GetChangedAppointments();
		}
		protected AppointmentBaseCollection GetNonPermanentAppointments() {
			AppointmentBaseCollection result = new AppointmentBaseCollection();
			return result;
		}
		protected internal virtual void LayoutAppointmentsCore(AppointmentControlCollection targetCollection, AppointmentBaseCollection appointments, AppointmentBaseCollection nonpermanentAppointments, bool alternate) {
			ISchedulerViewInfoBase viewInfo = View.ViewInfo;
			IContinuousCellsInfosCalculator cellsInfosCalculator = View.FactoryHelper.CreateVisuallyContinuousCellsInfosCalculator(View.InnerView, alternate);
			ResourceVisuallyContinuousCellsInfosCollection cellsInfos = cellsInfosCalculator.Calculate(viewInfo, View.VisibleResources);
			AppointmentBaseLayoutCalculator appointmentLayoutCalculator = View.FactoryHelper.CreateAppointmentLayoutCalculator(viewInfo, alternate);
			LayoutAppointmentsInnerCore(targetCollection, appointments, cellsInfos, appointmentLayoutCalculator, alternate, true);
			if (nonpermanentAppointments != null)
				LayoutAppointmentsInnerCore(targetCollection, nonpermanentAppointments, cellsInfos, appointmentLayoutCalculator, alternate, false);
		}
		protected internal virtual void LayoutAppointmentsInnerCore(AppointmentControlCollection targetCollection, AppointmentBaseCollection appointments, ResourceVisuallyContinuousCellsInfosCollection cellsInfo, AppointmentBaseLayoutCalculator layoutCalculator, bool alternate, bool isPermanentAppointments) {
			AppointmentsLayoutResult layoutResult = new AppointmentsLayoutResult();
			layoutCalculator.CalculateLayout(layoutResult, appointments, cellsInfo);
			int count = layoutResult.AppointmentControls.Count;
			for(int i = 0; i < count; i++) {
				AppointmentControl appointmentControl = layoutResult.AppointmentControls[i];
				appointmentControl.IsPermanentAppointment = isPermanentAppointments;
				AppointmentViewInfo appointmentViewInfo = appointmentControl.ViewInfo;
				targetCollection.Add(appointmentControl);
				CalculateContentViewInfo(View, appointmentViewInfo, alternate);
				AppointmentViewInfoCustomizingEventArgs args = new AppointmentViewInfoCustomizingEventArgs(appointmentViewInfo);
				View.Control.RaiseAppointmentViewInfoCustomizing(args);
			}
		}
		protected internal void CalculateContentViewInfo(SchedulerViewBase view, AppointmentViewInfo appointmentViewInfo, bool alternate) {
			appointmentViewInfo.Status = View.Control.GetStatus(appointmentViewInfo.Appointment.StatusKey);
			AppointmentContentCalculatorHelper helper = AppointmentContentCalculatorHelperFactory.Create(view, alternate);
			helper.CalculateViewInfoOptions(appointmentViewInfo);
			appointmentViewInfo.ContentCalculatorHelper = helper;
			appointmentViewInfo.IsStartVisible = CalculateIsStartVisible(view, appointmentViewInfo);
			appointmentViewInfo.IsEndVisible = CalculateIsEndVisible(view, appointmentViewInfo);
		}
		bool CalculateIsEndVisible(SchedulerViewBase view, AppointmentViewInfo viewInfo) {
			if(view == null)
				return true;
			DateTime visibleIntervalEnd = view.InnerVisibleIntervals.Interval.End;
			return viewInfo.Appointment.End <= visibleIntervalEnd || viewInfo.Interval.End != visibleIntervalEnd;
		}
		bool CalculateIsStartVisible(SchedulerViewBase view, AppointmentViewInfo viewInfo) {
			if(view == null)
				return true;
			DateTime visibleIntervalStart = view.InnerVisibleIntervals.Interval.Start;
			return viewInfo.Appointment.Start >= visibleIntervalStart && viewInfo.Interval.Start != visibleIntervalStart;
		}
	}
	public class AppointmentContentCalculatorHelperFactory {
		public static AppointmentContentCalculatorHelper Create(SchedulerViewBase view, bool alternate) {
			ISchedulerViewInfoBase viewInfo = view.ViewInfo;
			AppointmentDisplayOptions options = view.InnerView.AppointmentDisplayOptions;
			AppointmentStatusDisplayType status = options.StatusDisplayType;
			DevExpress.XtraScheduler.Services.IAppointmentFormatStringService formatStringProvider = view.ViewInfo.GetFormatStringProvider();
			switch(view.Type) {
				case SchedulerViewType.Day:
				case SchedulerViewType.WorkWeek:
				case SchedulerViewType.FullWeek:
					DayViewAppointmentDisplayOptions dayViewOptions = (DayViewAppointmentDisplayOptions)options;
					if(!alternate)
						return new VerticalAppointmentContentCalculatorHelper(viewInfo, status, formatStringProvider);
					else
						return new HorizontalAppointmentContentCalculatorHelper(viewInfo, dayViewOptions.AllDayAppointmentsStatusDisplayType, formatStringProvider);
				case SchedulerViewType.Timeline:
					return new TimelineAppointmentContentLayoutCalculatorHelper(viewInfo, status, formatStringProvider);
			}
			return new HorizontalAppointmentContentCalculatorHelper(viewInfo, status, formatStringProvider);
		}
	}
	public class DayViewAppointmentsLayoutCalculator : AppointmentsLayoutCalculator {
		public DayViewAppointmentsLayoutCalculator(SchedulerViewBase view)
			: base(view) {
		}
		public override void LayoutDraggedAppointments() {			
			View.ViewInfo.DraggedAppointmentControls.Clear();
			DayView view = (DayView)View;
			if (!view.ActualShowAllAppointmentsAtTimeCells)
				LayoutHorizontalAppointments(true);
			LayoutVerticalAppointments(true);
			CellContainerCollection containers = View.ViewInfo.GetContainers();
			foreach (ICellContainer container in containers) {
				container.FillDraggedAppointmentControls(View.ViewInfo.DraggedAppointmentControls);
			}
		}
		public override void LayoutAppointments() {
			View.ViewInfo.AppointmentControls.Clear();
			DayView view = (DayView)View;
			if (!view.ActualShowAllAppointmentsAtTimeCells)
				LayoutHorizontalAppointments(false);
			LayoutVerticalAppointments(false);
			CellContainerCollection containers = View.ViewInfo.GetContainers();
			foreach (ICellContainer container in containers) {
				container.FillAppointmentControls(View.ViewInfo.AppointmentControls);
			}
		}
		protected virtual void LayoutHorizontalAppointments(bool draggedAppointments) {
			AppointmentBaseCollection appointments = GetLongAppointments(draggedAppointments);
			AppointmentBaseCollection nonpermanentAppointments = GetLongNonPermanentAppointments(draggedAppointments);
			if(draggedAppointments)
				LayoutAppointmentsCore(View.ViewInfo.DraggedAppointmentControls, appointments, nonpermanentAppointments, true);
			else
				LayoutAppointmentsCore(View.ViewInfo.AppointmentControls, appointments, nonpermanentAppointments, true);
		}
		protected virtual void LayoutVerticalAppointments(bool draggedAppointments) {
			AppointmentBaseCollection appointments = GetTimeCellAppointments(draggedAppointments);
			AppointmentBaseCollection nonpermanentAppointments = GetTimeCellNonpermanentAppointments(draggedAppointments);
			if (draggedAppointments)
				LayoutAppointmentsCore(View.ViewInfo.DraggedAppointmentControls, appointments, nonpermanentAppointments, false);
			else
				LayoutAppointmentsCore(View.ViewInfo.AppointmentControls, appointments, nonpermanentAppointments, false);				
		}
		protected internal virtual AppointmentBaseCollection GetShortAppointments(bool draggedAppointments) {
			return GetAppointments(new DayViewTimeCellAppointmentsFilter(), draggedAppointments);
		}
		protected internal virtual AppointmentBaseCollection GetLongAppointments(bool draggedAppointments) {
			return GetAppointments(new DayViewAllDayAppointmentsFilter(), draggedAppointments);
		}
		protected internal virtual AppointmentBaseCollection GetLongNonPermanentAppointments(bool draggedAppointments) {
			return GetNonPermanentAppointments(new DayViewAllDayAppointmentsFilter());
		}
		protected internal virtual AppointmentBaseCollection GetShortNonpermanentAppointments(bool draggedAppointments) {
			return GetNonPermanentAppointments(new DayViewTimeCellAppointmentsFilter());
		}
		protected internal virtual AppointmentBaseCollection GetTimeCellAppointments(bool draggedAppointments) {
			DayView view = (DayView)View;
			if (view.ActualShowAllAppointmentsAtTimeCells)
				return draggedAppointments ? GetDraggedAppointments() : GetPermanentAppointments();
			else
				return GetShortAppointments(draggedAppointments);
		}
		protected internal virtual AppointmentBaseCollection GetTimeCellNonpermanentAppointments(bool draggedAppointments) {
			DayView view = (DayView)View;
			if (view.ActualShowAllAppointmentsAtTimeCells)
				return draggedAppointments ? GetDraggedAppointments() : GetNonPermanentAppointments();
			else
				return GetShortNonpermanentAppointments(draggedAppointments);
		}
		protected internal virtual AppointmentBaseCollection GetAppointments(ProcessorBase<Appointment> processor, bool draggedAppointments) {
			AppointmentBaseCollection appointments = draggedAppointments ? GetDraggedAppointments() : GetPermanentAppointments();
			processor.Process(appointments);
			return (AppointmentBaseCollection)processor.DestinationCollection;
		}
		protected internal virtual AppointmentBaseCollection GetNonPermanentAppointments(ProcessorBase<Appointment> processor) {
			AppointmentBaseCollection appointments = GetNonPermanentAppointments();
			processor.Process(appointments);
			AppointmentBaseCollection result = new AppointmentBaseCollection();
			result.AddRange(processor.DestinationCollection);
			return result;
		}
	}
}
