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
namespace DevExpress.XtraScheduler.Native {
	#region AppointmentOperationHelper
	public class AppointmentOperationHelper {
		InnerSchedulerControl control;
		public AppointmentOperationHelper(InnerSchedulerControl control) {
			if (control == null)
				Exceptions.ThrowArgumentException("control", control);
			this.control = control;
		}
		protected internal InnerSchedulerControl Control { get { return control; } }
		protected internal virtual bool CanPerformOperation(AppointmentBaseCollection appointments, UsedAppointmentType usedType, AppointmentOperationEventHandler eventObject) {
			if (usedType == UsedAppointmentType.None)
				return false;
			int count = appointments.Count;
			if (count <= 0)
				return false;
			if (usedType == UsedAppointmentType.All)
				return true;
			bool first = CanPerformOperation(appointments[0], usedType, eventObject);
			for (int i = 1; i < count; i++)
				if (CanPerformOperation(appointments[i], usedType, eventObject) != first)
					return true;
			return first;
		}
		protected internal virtual bool CanPerformOperation(Appointment apt, UsedAppointmentType usedType, AppointmentOperationEventHandler eventObject) {
			return CanPerformOperation(apt, usedType, eventObject, true);
		}
		protected internal virtual bool CanPerformOperation(Appointment apt, UsedAppointmentType usedType, AppointmentOperationEventHandler eventObject, bool checkLimitInterval) {
			if (checkLimitInterval && !IsFitToLimitInterval(apt))
				return false;
			switch (usedType) {
				case UsedAppointmentType.All:
					return true;
				case UsedAppointmentType.Recurring:
					return apt.IsRecurring;
				case UsedAppointmentType.NonRecurring:
					return !apt.IsRecurring;
				case UsedAppointmentType.Custom:
					return RaiseCanPerformExistingAppointmentOperationEvent(apt, eventObject);
				case UsedAppointmentType.None:
				default:
					return false;
			}
		}
		protected internal virtual bool CanPerformRecurrenceAppointmentOperation(Appointment apt, UsedAppointmentType usedType, AppointmentOperationEventHandler eventObject) {
			switch (usedType) {
				case UsedAppointmentType.All:
					return true;
				case UsedAppointmentType.Recurring:
					return false;
				case UsedAppointmentType.NonRecurring:
					return false;
				case UsedAppointmentType.Custom:
					return RaiseCanPerformExistingAppointmentOperationEvent(apt, eventObject);
				case UsedAppointmentType.None:
				default:
					return false;
			}
		}
		protected internal virtual bool RaiseCanPerformExistingAppointmentOperationEvent(Appointment apt, AppointmentOperationEventHandler eventObject) {
			AppointmentOperationEventArgs args = new AppointmentOperationEventArgs(apt);
			control.RaiseAppointmentOperationEvent(eventObject, args);
			return args.Allow;
		}
		protected internal virtual bool CanPerformNewAppointmentOperation(bool recurring, UsedAppointmentType usedType, AppointmentOperationEventHandler eventObject, bool supportsRecurrence) {
			if (recurring)
				if (!supportsRecurrence)
					return false;
			switch (usedType) {
				case UsedAppointmentType.All:
					return true;
				case UsedAppointmentType.Recurring:
					return recurring && supportsRecurrence;
				case UsedAppointmentType.NonRecurring:
					return !recurring;
				case UsedAppointmentType.Custom:
					return RaiseCanPerformNewAppointmentOperationEvent(recurring, eventObject);
				case UsedAppointmentType.None:
				default:
					return false;
			}
		}
		protected internal virtual bool RaiseCanPerformNewAppointmentOperationEvent(bool recurring, AppointmentOperationEventHandler eventObject) {
			NewAppointmentOperationEventArgs args = new NewAppointmentOperationEventArgs();
			args.SetRecurring(recurring);
			control.RaiseAppointmentOperationEvent(eventObject, args);
			return args.Allow;
		}
		public virtual bool CanEditAppointmentAndChangeRecurrence(Appointment apt) {
			return CanPerformRecurrenceAppointmentOperation(apt, control.OptionsCustomization.AllowAppointmentEdit, control.onAllowAppointmentEdit);
		}
		public virtual bool CanEditAppointment(Appointment apt) {
			return CanEditAppointment(apt, true);
		}
		public virtual bool CanEditAppointment(Appointment apt, bool checkLimitInterval) {
			return CanPerformOperation(apt, control.OptionsCustomization.AllowAppointmentEdit, control.onAllowAppointmentEdit, checkLimitInterval);
		}
		public virtual bool CanEditAppointments(AppointmentBaseCollection appointments) {
			return CanPerformOperation(appointments, control.OptionsCustomization.AllowAppointmentEdit, control.onAllowAppointmentEdit);
		}
		public virtual bool CanDragAppointment(Appointment apt) {
			if (!CanEditAppointment(apt))
				return false;
			return CanPerformOperation(apt, control.OptionsCustomization.AllowAppointmentDrag, control.onAllowAppointmentDrag);
		}
		public virtual bool IsFitToLimitInterval(Appointment apt) {
			return Control.LimitInterval.IntersectsWith(((IInternalAppointment)apt).GetInterval());
		}
		public virtual bool CanDragAppointmentBetweenResources(Appointment apt) {
			return CanDragAppointmentBetweenResources(apt, true);
		}
		public virtual bool CanDragAppointmentBetweenResources(Appointment apt, bool checkLimitInterval) {
			if (!CanEditAppointment(apt, checkLimitInterval))
				return false;
			return CanPerformOperation(apt, control.OptionsCustomization.AllowAppointmentDragBetweenResources, control.onAllowAppointmentDragBetweenResources, checkLimitInterval);
		}
		public virtual bool CanResizeAppointment(Appointment apt) {
			if (!CanEditAppointment(apt))
				return false;
			return CanPerformOperation(apt, control.OptionsCustomization.AllowAppointmentResize, control.onAllowAppointmentResize);
		}
		public virtual bool CanDeleteAppointment(Appointment apt) {
			return CanPerformOperation(apt, control.OptionsCustomization.AllowAppointmentDelete, control.onAllowAppointmentDelete);
		}
		public virtual bool CanDeleteAppointments(AppointmentBaseCollection appointments) {
			return CanPerformOperation(appointments, control.OptionsCustomization.AllowAppointmentDelete, control.onAllowAppointmentDelete);
		}
		public virtual bool CanCopyAppointment(Appointment apt) {
			return CanPerformOperation(apt, control.OptionsCustomization.AllowAppointmentCopy, control.onAllowAppointmentCopy);
		}
		public virtual bool CanCopyAppointments(AppointmentBaseCollection appointments) {
			return CanPerformOperation(appointments, control.OptionsCustomization.AllowAppointmentCopy, control.onAllowAppointmentCopy);
		}
		public virtual bool CanEditAppointmentViaInplaceEditor(Appointment apt) {
			if (!CanEditAppointment(apt))
				return false;
			return CanPerformOperation(apt, control.OptionsCustomization.AllowInplaceEditor, control.onAllowInplaceEditor);
		}
		public virtual bool CanCreateAppointment(Appointment appointment) {
			if (!CanPerformNewAppointmentOperation(appointment.IsRecurring, control.OptionsCustomization.AllowAppointmentCreate, control.onAllowAppointmentCreate, control.Storage.SupportsRecurrence))
				return false;
			if (appointment.Start < control.LimitInterval.Start)
				return false;
			if (control.OptionsCustomization.AllowAppointmentConflicts == AppointmentConflictsMode.Allowed)
				return true;
			AppointmentConflictsCalculator conflictsCalculator = CreateConflictsCalculator(control.GetNonFilteredAppointments());
			if (control.OptionsCustomization.AllowAppointmentConflicts == AppointmentConflictsMode.Forbidden) {
				Resource resource = Control.Storage.GetResourceById(appointment.ResourceId);
				return !conflictsCalculator.IsIntersecting(((IInternalAppointment)appointment).GetInterval(), resource);
			}
			return true;
		}
		public virtual bool CanCreateAppointment(TimeInterval interval, Resource resource, bool recurring, bool checkAppointmentConflicts) {
			if (!CanPerformNewAppointmentOperation(recurring, control.OptionsCustomization.AllowAppointmentCreate, control.onAllowAppointmentCreate, control.Storage.SupportsRecurrence))
				return false;
			if (!interval.IntersectsWithExcludingBounds(Control.LimitInterval))
				return false;
			if (control.OptionsCustomization.AllowAppointmentConflicts == AppointmentConflictsMode.Allowed)
				return true;
			if (!checkAppointmentConflicts)
				return true;
			AppointmentConflictsCalculator conflictsCalculator = CreateConflictsCalculator(control.GetNonFilteredAppointments());
			if (control.OptionsCustomization.AllowAppointmentConflicts == AppointmentConflictsMode.Forbidden) {
				return !conflictsCalculator.IsIntersecting(interval, resource);
			} else {
				AppointmentType type = recurring ? AppointmentType.Pattern : AppointmentType.Normal;
				Appointment fakeAppointment = SchedulerUtils.CreateAppointmentInstance(control.Storage, type);
				fakeAppointment.Start = interval.Start;
				fakeAppointment.Duration = interval.Duration;
				fakeAppointment.ResourceId = resource.Id;
				Appointment clone = fakeAppointment.Copy();
				return IsConflictResolved(clone, fakeAppointment, false);
			}
		}
		public virtual bool CanCreateAppointment(TimeInterval interval, Resource resource, bool recurring) {
			return CanCreateAppointment(interval, resource, recurring, true);
		}
		public virtual bool CanCreateAppointmentViaInplaceEditor(TimeInterval interval, Resource resource) {
			if (!CanCreateAppointment(interval, resource, false))
				return false;
			return CanPerformNewAppointmentOperation(false, control.OptionsCustomization.AllowInplaceEditor, control.onAllowInplaceEditor, control.Storage.SupportsRecurrence);
		}
		public virtual bool IsConflictResolved(Appointment clone, Appointment source, bool copy) {
			if (control.OptionsCustomization.AllowAppointmentConflicts == AppointmentConflictsMode.Allowed)
				return true;
			AppointmentBaseCollection conflicts = ObtainConflictedAppointments(clone, source, copy);
			return IsConflictResolvedCore(conflicts, source, copy);
		}
		public virtual int CalculateConflictCount(Appointment clone, Appointment source, bool copy) {
			if (control.OptionsCustomization.AllowAppointmentConflicts == AppointmentConflictsMode.Allowed)
				return 0;
			AppointmentBaseCollection conflicts = ObtainConflictedAppointments(clone, source, copy);
			return CalcConflictCountCore(conflicts, source, copy);
		}
		protected internal virtual AppointmentBaseCollection ObtainConflictedAppointments(Appointment clone, Appointment source, bool copy) {
			TimeInterval interval = CalculateIntersectionInterval(clone);
			AppointmentConflictsCalculator conflictCalculator = CreateConflictsCalculator(control.GetNonFilteredAppointments());
			AppointmentBaseCollection conflicts = conflictCalculator.CalculateConflicts(clone, interval);
			if (control.OptionsCustomization.AllowAppointmentConflicts == AppointmentConflictsMode.Custom) {
				AppointmentConflictEventArgs args = new AppointmentConflictEventArgs(source, clone, conflicts);
				args.SetInterval(interval);
				Control.RaiseAllowAppointmentConflicts(args);
			}
			return conflicts;
		}
		protected internal virtual AppointmentConflictsCalculator CreateConflictsCalculator(AppointmentBaseCollection appointments) {
			return new AppointmentConflictsCalculator(appointments);
		}
		protected internal virtual bool IsConflictResolvedCore(AppointmentBaseCollection conflicts, Appointment source, bool copy) {
			if (conflicts.Count <= 0)
				return true;
			if (!copy && conflicts.Count == 1 && AreTheSameAppointments(conflicts[0], source))
				return true;
			int count = conflicts.Count;
			for (int i = 0; i < count; i++)
				if (IsActualConflict(conflicts[i], source, copy))
					return false;
			return true;
		}
		protected internal virtual int CalcConflictCountCore(AppointmentBaseCollection conflicts, Appointment source, bool copy) {
			if (conflicts.Count <= 0)
				return 0;
			if (!copy && conflicts.Count == 1 && AreTheSameAppointments(conflicts[0], source))
				return 0;
			int result = 0;
			int count = conflicts.Count;
			for (int i = 0; i < count; i++)
				if (IsActualConflict(conflicts[i], source, copy))
					result++;
			return result;
		}
		protected internal virtual bool IsActualConflict(Appointment conflictedApt, Appointment source, bool copy) {
			if (conflictedApt.RecurrencePattern != source) {
				if (copy || !control.IsAppointmentCurrentlyEdited(conflictedApt))
					return true;
			}
			return false;
		}
		TimeInterval CalculateIntersectionInterval(Appointment appointment) {
			if (appointment.Type == AppointmentType.Pattern)
				return AppointmentCollection.CalcPatternInterval(appointment);
			else
				return ((IInternalAppointment)appointment).CreateInterval();
		}
		protected internal virtual bool AreTheSameAppointments(Appointment firstAppointment, Appointment secondAppointment) {
			if (firstAppointment.Type == AppointmentType.Occurrence && secondAppointment.Type == AppointmentType.Occurrence)
				return firstAppointment.RecurrencePattern == secondAppointment.RecurrencePattern && firstAppointment.RecurrenceIndex == secondAppointment.RecurrenceIndex;
			else
				return firstAppointment == secondAppointment;
		}
		internal void RaiseAppointmentDragEvent(AppointmentDragEventArgs args) {
			control.RaiseAppointmentDrag(args);
		}
		internal void RaiseAppointmentDropEvent(AppointmentDragEventArgs args) {
			control.RaiseAppointmentDrop(args);
		}
		internal void RaiseAppointmentResizingEvent(AppointmentResizeEventArgs args) {
			control.RaiseAppointmentResizing(args);
		}
		internal void RaiseAppointmentResizedEvent(AppointmentResizeEventArgs args) {
			control.RaiseAppointmentResized(args);
		}
	}
	#endregion
}
