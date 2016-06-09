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
using System.ComponentModel.Design;
using DevExpress.XtraScheduler.Commands;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.Services;
using DevExpress.XtraScheduler.Services.Internal;
using DevExpress.XtraScheduler.Drawing;
using System.Collections.Generic;
using DevExpress.XtraScheduler.Internal.Implementations;
namespace DevExpress.XtraScheduler.Services {
	#region IDateTimeNavigationService
	public interface IDateTimeNavigationService {
		void GoToToday();
		void GoToDate(DateTime date);
		void GoToDate(DateTime date, SchedulerViewType viewType);
		void NavigateForward();
		void NavigateBackward();
	}
	#endregion
	#region IResourceNavigationService
	public interface IResourceNavigationService {
		int ResourcePerPage { get; set; }
		int FirstVisibleResourceIndex { get; set; }
		void GoToResource(Resource resource);
		void GoToResourceById(object resourceId);
		void NavigateFirst();
		void NavigateLast();
		void NavigateForward();
		void NavigateBackward();
		void NavigatePageForward();
		void NavigatePageBackward();
		bool CanGoToResource(Resource resource);
		bool CanGoToResourceById(object resourceId);
		bool CanNavigateFirst();
		bool CanNavigateLast();
		bool CanNavigateForward();
		bool CanNavigateBackward();
		bool CanNavigatePageForward();
		bool CanNavigatePageBackward();
	}
	#endregion
	#region ISelectionService
	public interface ISelectionService {
		TimeInterval SelectedInterval { get; set; }
		Resource SelectedResource { get; set; }
		void SetSelection(TimeInterval interval, Resource resource);
		event EventHandler SelectionChanged;
	}
	#endregion
	#region IAppointmentSelectionService
	public interface IAppointmentSelectionService {
		AppointmentBaseCollection GetSelectedAppointments();
		void SetSelectedAppointments(AppointmentBaseCollection appointments);
		bool IsAppointmentSelected(Appointment apt);
		void ClearSelection();
		void ToggleSelection(Appointment apt);
		void SelectAppointment(Appointment apt);
		void SelectAppointment(Appointment apt, AppointmentSelectionChangeAction changeAction);
		event EventHandler SelectionChanged;
	}
	#endregion
	#region SchedulerServicesBase (abstract class)
	public abstract class SchedulerServicesBase : IServiceContainer {
		InnerSchedulerControl control;
		protected SchedulerServicesBase(InnerSchedulerControl control) {
			if (control == null)
				Exceptions.ThrowArgumentNullException("control");
			this.control = control;
		}
		#region Properties
		internal InnerSchedulerControl Control { get { return control; } }
		public IDateTimeNavigationService DateTimeNavigation { get { return (IDateTimeNavigationService)control.GetService(typeof(IDateTimeNavigationService)); } }
		public IResourceNavigationService ResourceNavigation { get { return (IResourceNavigationService)control.GetService(typeof(IResourceNavigationService)); } }
		public DevExpress.XtraScheduler.Services.ISelectionService Selection { get { return (DevExpress.XtraScheduler.Services.ISelectionService)control.GetService(typeof(DevExpress.XtraScheduler.Services.ISelectionService)); } }
		public IAppointmentSelectionService AppointmentSelection { get { return (IAppointmentSelectionService)control.GetService(typeof(IAppointmentSelectionService)); } }
		#endregion
		#region IServiceContainer Members
		public void AddService(Type serviceType, ServiceCreatorCallback callback, bool promote) {
			control.AddService(serviceType, callback, promote);
		}
		public void AddService(Type serviceType, ServiceCreatorCallback callback) {
			control.AddService(serviceType, callback);
		}
		public void AddService(Type serviceType, object serviceInstance, bool promote) {
			control.AddService(serviceType, serviceInstance, promote);
		}
		public void AddService(Type serviceType, object serviceInstance) {
			control.AddService(serviceType, serviceInstance);
		}
		public void RemoveService(Type serviceType, bool promote) {
			control.RemoveService(serviceType, promote);
		}
		public void RemoveService(Type serviceType) {
			control.RemoveService(serviceType);
		}
		#endregion
		#region IServiceProvider Members
		public object GetService(Type serviceType) {
			return control.GetService(serviceType);
		}
		#endregion
	}
	#endregion
	#region ITimeRulerFormatStringService
	public interface ITimeRulerFormatStringService {
		string GetHourFormat(TimeRuler ruler);
		string GetHalfDayHourFormat(TimeRuler ruler);
		string GetHourOnlyFormat(TimeRuler ruler);
		string GetTimeDesignatorOnlyFormat(TimeRuler ruler);
		string GetMinutesOnlyFormat(TimeRuler ruler);
	}
	#endregion
	#region TimeRulerFormatStringServiceWrapper
	public class TimeRulerFormatStringServiceWrapper : ITimeRulerFormatStringService {
		ITimeRulerFormatStringService service;
		public TimeRulerFormatStringServiceWrapper(ITimeRulerFormatStringService service) {
			if (service == null)
				throw new ArgumentNullException("service", "service");
			this.service = service;
		}
		public ITimeRulerFormatStringService Service { get { return service; } }
		#region ITimeRulerFormatStringService Members
		public virtual string GetHourFormat(TimeRuler ruler) {
			return Service.GetHourFormat(ruler);
		}
		public virtual string GetHalfDayHourFormat(TimeRuler ruler) {
			return Service.GetHalfDayHourFormat(ruler);
		}
		public virtual string GetHourOnlyFormat(TimeRuler ruler) {
			return Service.GetHourOnlyFormat(ruler);
		}
		public virtual string GetTimeDesignatorOnlyFormat(TimeRuler ruler) {
			return Service.GetTimeDesignatorOnlyFormat(ruler);
		}
		public virtual string GetMinutesOnlyFormat(TimeRuler ruler) {
			return Service.GetMinutesOnlyFormat(ruler);
		}
		#endregion
	}
	#endregion
	#region IAppointmentFormatStringService
	public interface IAppointmentFormatStringService {
		string GetHorizontalAppointmentStartFormat(IAppointmentViewInfo aptViewInfo);
		string GetHorizontalAppointmentEndFormat(IAppointmentViewInfo aptViewInfo);
		string GetVerticalAppointmentStartFormat(IAppointmentViewInfo aptViewInfo);
		string GetVerticalAppointmentEndFormat(IAppointmentViewInfo aptViewInfo);
		string GetContinueItemStartFormat(IAppointmentViewInfo aptViewInfo);
		string GetContinueItemEndFormat(IAppointmentViewInfo aptViewInfo);
	}
	#endregion
	public class AppointmentFormatStringServiceWrapper : IAppointmentFormatStringService {
		IAppointmentFormatStringService service;
		public AppointmentFormatStringServiceWrapper(IAppointmentFormatStringService service) {
			if (service == null)
				throw new ArgumentNullException("service", "service");
			this.service = service;
		}
		public IAppointmentFormatStringService Service { get { return service; } }
		#region IAppointmentFormatStringService Members
		public virtual string GetHorizontalAppointmentStartFormat(IAppointmentViewInfo aptViewInfo) {
			return String.Empty;
		}
		public virtual string GetHorizontalAppointmentEndFormat(IAppointmentViewInfo aptViewInfo) {
			return String.Empty;
		}
		public virtual string GetVerticalAppointmentStartFormat(IAppointmentViewInfo aptViewInfo) {
			return String.Empty;
		}
		public virtual string GetVerticalAppointmentEndFormat(IAppointmentViewInfo aptViewInfo) {
			return String.Empty;
		}
		public virtual string GetContinueItemStartFormat(IAppointmentViewInfo aptViewInfo) {
			return String.Empty;
		}
		public virtual string GetContinueItemEndFormat(IAppointmentViewInfo aptViewInfo) {
			return String.Empty;
		}
		#endregion
	}
}
namespace DevExpress.XtraScheduler.Services.Internal {
	#region ValueFormatType
	public enum ValueFormatType {
		AppointmentDateTime
	}
	#endregion
	public interface IHeaderCaptionServiceObject {
		TimeInterval Interval { get;}
		Resource Resource { get;}
	}
	public interface IDayOfWeekHeaderCaptionServiceObject {
		Resource Resource { get;}
		DayOfWeek DayOfWeek { get;}
	}
	public interface ITimeScaleHeaderCaptionServiceObject {
		TimeInterval Interval { get;}
		TimeScale Scale { get;}
	}
	public abstract class HeaderCaptionFormatProviderBase {
		public abstract string GetDayColumnHeaderCaption(IHeaderCaptionServiceObject header);
		public abstract string GetDayOfWeekHeaderCaption(IDayOfWeekHeaderCaptionServiceObject header);
		public abstract string GetHorizontalWeekCellHeaderCaption(IHeaderCaptionServiceObject header);
		public abstract string GetVerticalWeekCellHeaderCaption(IHeaderCaptionServiceObject header);
		public abstract string GetTimeScaleHeaderCaption(ITimeScaleHeaderCaptionServiceObject header);
		public abstract string GetDayOfWeekAbbreviatedHeaderCaption(IDayOfWeekHeaderCaptionServiceObject header);
	}
	public interface IExternalAppointmentCompareService {
		IComparer<Appointment> Comparer { get; }
	}
}
namespace DevExpress.XtraScheduler.Services.Implementation {
	#region TimeRulerFormatStringService
	public class TimeRulerFormatStringService : ITimeRulerFormatStringService {
		#region ITimeRulerFormatStringService Members
		public string GetHourFormat(TimeRuler ruler) {
			return String.Empty;
		}
		public string GetHalfDayHourFormat(TimeRuler ruler) {
			return String.Empty;
		}
		public string GetHourOnlyFormat(TimeRuler ruler) {
			return String.Empty;
		}
		public string GetTimeDesignatorOnlyFormat(TimeRuler ruler) {
			return String.Empty;
		}
		public string GetMinutesOnlyFormat(TimeRuler ruler) {
			return String.Empty;
		}
		#endregion
	}
	#endregion
	#region AppointmentFormatStringService
	public class AppointmentFormatStringService : IAppointmentFormatStringService {
		#region IAppointmentFormatStringService Members
		public string GetHorizontalAppointmentStartFormat(IAppointmentViewInfo aptViewInfo) {
			return String.Empty;
		}
		public string GetHorizontalAppointmentEndFormat(IAppointmentViewInfo aptViewInfo) {
			return String.Empty;
		}
		public string GetVerticalAppointmentStartFormat(IAppointmentViewInfo aptViewInfo) {
			return String.Empty;
		}
		public string GetVerticalAppointmentEndFormat(IAppointmentViewInfo aptViewInfo) {
			return String.Empty;
		}
		public string GetContinueItemStartFormat(IAppointmentViewInfo aptViewInfo) {
			return String.Empty;
		}
		public string GetContinueItemEndFormat(IAppointmentViewInfo aptViewInfo) {
			return String.Empty;
		}
		#endregion
	}
	#endregion
	public class DateTimeNavigationService : IDateTimeNavigationService {
		InnerSchedulerControl control;
		public DateTimeNavigationService(InnerSchedulerControl control) {
			if (control == null)
				Exceptions.ThrowArgumentNullException("control");
			this.control = control;
		}
		public InnerSchedulerControl Control { get { return control; } }
		#region IDateTimeNavigationService Members
		public void GoToToday() {
			DateTime clientToday = control.TimeZoneHelper.ToClientTime(DateTime.Now).Date;
			GoToDate(clientToday);
		}
		public void GoToDate(DateTime date) {
			GoToDate(date, control.ActiveViewType);
		}
		public void GoToDate(DateTime date, SchedulerViewType viewType) {
			control.BeginUpdate();
			try {
				if (control.ActiveViewType != viewType) {
					if (viewType == SchedulerViewType.Day)
						control.DayView.DayCount = 1;
					control.ActiveViewType = viewType;
				}
				control.Start = date;
				ChangeActions actions = control.ActiveView.InitializeSelectionCore(control.Selection, date);
				actions |= control.ActiveView.ValidateSelectionInterval(control.Selection);
				control.ApplyChangesCore(SchedulerControlChangeType.SelectionChanged, actions);
			}
			finally {
				control.EndUpdate();
			}
		}
		public void NavigateForward() {
			NavigateViewForwardCommand cmd = new NavigateViewForwardCommand(control);
			cmd.Execute();
		}
		public void NavigateBackward() {
			NavigateViewBackwardCommand cmd = new NavigateViewBackwardCommand(control);
			cmd.Execute();
		}
		#endregion
	}
	#region ResourceNavigationService
	public class ResourceNavigationService : IResourceNavigationService {
		InnerSchedulerControl control;
		public ResourceNavigationService(InnerSchedulerControl control) {
			if (control == null)
				Exceptions.ThrowArgumentNullException("control");
			this.control = control;
		}
		public InnerSchedulerControl Control { get { return control; } }
		#region IResourceNavigationService Members
		public int ResourcePerPage { get { return control.ActiveView.ResourcesPerPage; } set { control.ActiveView.ResourcesPerPage = value; } }
		public int FirstVisibleResourceIndex { get { return control.ActiveView.FirstVisibleResourceIndex; } set { control.ActiveView.FirstVisibleResourceIndex = value; } }
		public virtual void GoToResource(Resource resource) {
			if (resource == null)
				return;
			GoToResourceById(resource.Id);
		}
		public virtual void GoToResourceById(object resourceId) {
			InnerSchedulerViewBase view = control.ActiveView;
			Resource resource = view.FilteredResources.GetResourceById(resourceId);
			if (resource != ResourceBase.Empty) {
				if (!view.VisibleResources.Contains(resource))
					view.FirstVisibleResourceIndex = view.FilteredResources.IndexOf(resource);
			}
		}
		public void NavigateFirst() {
			NavigateFirstResourceCommand command = new NavigateFirstResourceCommand(control);
			command.Execute();
		}
		public void NavigateLast() {
			NavigateLastResourceCommand command = new NavigateLastResourceCommand(control);
			command.Execute();
		}
		public void NavigateForward() {
			NavigateNextResourceCommand command = new NavigateNextResourceCommand(control);
			command.Execute();
		}
		public void NavigateBackward() {
			NavigatePrevResourceCommand command = new NavigatePrevResourceCommand(control);
			command.Execute();
		}
		public void NavigatePageForward() {
			NavigateResourcePageForwardCommand command = new NavigateResourcePageForwardCommand(control);
			command.Execute();
		}
		public void NavigatePageBackward() {
			NavigateResourcePageBackwardCommand command = new NavigateResourcePageBackwardCommand(control);
			command.Execute();
		}
		public virtual bool CanGoToResource(Resource resource) {
			if (resource == null)
				return false;
			return CanGoToResourceById(resource.Id);
		}
		public virtual bool CanGoToResourceById(object resourceId) {
			InnerSchedulerViewBase view = control.ActiveView;
			Resource resource = view.FilteredResources.GetResourceById(resourceId);
			return resource != ResourceBase.Empty;
		}
		public bool CanNavigateFirst() {
			NavigateFirstResourceCommand command = new NavigateFirstResourceCommand(control);
			return command.CanExecute();
		}
		public bool CanNavigateLast() {
			NavigateLastResourceCommand command = new NavigateLastResourceCommand(control);
			return command.CanExecute();
		}
		public bool CanNavigateForward() {
			NavigateNextResourceCommand command = new NavigateNextResourceCommand(control);
			return command.CanExecute();
		}
		public bool CanNavigateBackward() {
			NavigatePrevResourceCommand command = new NavigatePrevResourceCommand(control);
			return command.CanExecute();
		}
		public bool CanNavigatePageForward() {
			NavigateResourcePageForwardCommand command = new NavigateResourcePageForwardCommand(control);
			return command.CanExecute();
		}
		public bool CanNavigatePageBackward() {
			NavigateResourcePageBackwardCommand command = new NavigateResourcePageBackwardCommand(control);
			return command.CanExecute();
		}
		#endregion
	}
	#endregion
	#region SelectionService
	public class SelectionService : ISelectionService {
		InnerSchedulerControl control;
		public SelectionService(InnerSchedulerControl control) {
			if (control == null)
				Exceptions.ThrowArgumentNullException("control");
			this.control = control;
		}
		public InnerSchedulerControl Control { get { return control; } }
		protected internal virtual SetSelectionCommand CreateSetSelectionCommand(TimeInterval interval, Resource resource) {
			return new SetSelectionCommand(Control, interval, resource);
		}
		#region ISelectionService Members
		public TimeInterval SelectedInterval {
			get { return control.Selection.Interval.Clone(); }
			set {
				SetSelectionCommand command = CreateSetSelectionCommand(value, SelectedResource);
				command.Execute();
			}
		}
		public Resource SelectedResource {
			get { return control.Selection.Resource; }
			set {
				SetSelectionCommand command = CreateSetSelectionCommand(SelectedInterval, value);
				command.Execute();
			}
		}
		public void SetSelection(TimeInterval interval, Resource resource) {
			SetSelectionCommand command = CreateSetSelectionCommand(interval, resource);
			command.Execute();
		}
		public event EventHandler SelectionChanged { add { control.SelectionChanged += value; } remove { control.SelectionChanged -= value; } }
		#endregion
	}
	#endregion
	#region AppointmentSelectionService
	public class AppointmentSelectionService : IAppointmentSelectionService {
		InnerSchedulerControl control;
		public AppointmentSelectionService(InnerSchedulerControl control) {
			if (control == null)
				Exceptions.ThrowArgumentNullException("control");
			this.control = control;
		}
		public InnerSchedulerControl Control { get { return control; } }
		protected internal AppointmentSelectionController Controller { get { return Control.AppointmentSelectionController; } }
		#region IAppointmentSelectionService Members
		public AppointmentBaseCollection GetSelectedAppointments() {
			AppointmentBaseCollection result = new AppointmentBaseCollection();
			result.AddRange(Controller.SelectedAppointments);
			return result;
		}
		public virtual void SetSelectedAppointments(AppointmentBaseCollection appointments) {
			Controller.SelectAppointments(appointments);
		}
		public virtual bool IsAppointmentSelected(Appointment apt) {
			return Controller.IsAppointmentSelected(apt);
		}
		public virtual void ClearSelection() {
			Controller.ClearSelection();
		}
		public virtual void ToggleSelection(Appointment apt) {
			Controller.ChangeSelection(apt);
		}
		public virtual void SelectAppointment(Appointment apt) {
			Controller.SelectSingleAppointment(apt);
		}
		public virtual void SelectAppointment(Appointment apt, AppointmentSelectionChangeAction changeAction) {
			Controller.ApplyChanges(changeAction, apt);
		}
		public event EventHandler SelectionChanged { add { Controller.SelectionChanged += value; } remove { Controller.SelectionChanged -= value; } }
		#endregion
	}
	#endregion
}
