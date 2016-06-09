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
using DevExpress.Utils;
using DevExpress.XtraScheduler.Internal.Diagnostics;
namespace DevExpress.XtraScheduler {
	#region AppointmentSelectionChangeAction
	public enum AppointmentSelectionChangeAction {
		None,
		Clear,
		Select,
		Unselect,
		Add,
		Toggle
	};
	#endregion
}
namespace DevExpress.XtraScheduler.Native {
	#region AppointmentSelectionController
	public class AppointmentSelectionController : IDisposable, IBatchUpdateable, IBatchUpdateHandler {
		AppointmentBaseCollection selectedAppointments;
		bool isDisposed;
		BatchUpdateHelper batchUpdateHelper;
		bool allowAppointmentMultiSelect;
		public AppointmentSelectionController(bool allowAppointmentMultiSelect) {
			this.batchUpdateHelper = new BatchUpdateHelper(this);
			selectedAppointments = new AppointmentBaseCollection();
			SubscribeSelectedAppointmentsEvents();
			this.allowAppointmentMultiSelect = allowAppointmentMultiSelect;
		}
		#region Proprties
		internal bool IsDisposed { get { return isDisposed; } }
		public AppointmentBaseCollection SelectedAppointments { get { return selectedAppointments; } }
		#region AllowAppointmentMultiSelect
		public bool AllowAppointmentMultiSelect {
			get { return allowAppointmentMultiSelect; }
			set {
				if (allowAppointmentMultiSelect == value)
					return;
				allowAppointmentMultiSelect = value;
				OnAllowAppointmentMultiselectChanged();
			}
		}
		#endregion
		#endregion
		#region Events
		EventHandler onSelectionChanged;
		protected internal event EventHandler SelectionChanged { add { onSelectionChanged += value; } remove { onSelectionChanged -= value; } }
		protected internal virtual void RaiseSelectionChanged() {
			if (onSelectionChanged != null)
				onSelectionChanged(this, EventArgs.Empty);
		}
		#endregion
		#region IBatchUpdateable implementation
		public void BeginUpdate() {
			batchUpdateHelper.BeginUpdate();
		}
		public void EndUpdate() {
			batchUpdateHelper.EndUpdate();
		}
		public void CancelUpdate() {
			batchUpdateHelper.CancelUpdate();
		}
		BatchUpdateHelper IBatchUpdateable.BatchUpdateHelper { get { return batchUpdateHelper; } }
		public bool IsUpdateLocked { get { return batchUpdateHelper.IsUpdateLocked; } }
		#endregion
		#region IBatchUpdateHandler implementation
		void IBatchUpdateHandler.OnFirstBeginUpdate() {
			OnFirstBeginUpdate();
		}
		void IBatchUpdateHandler.OnBeginUpdate() {
			OnBeginUpdate();
		}
		void IBatchUpdateHandler.OnEndUpdate() {
			OnEndUpdate();
		}
		void IBatchUpdateHandler.OnLastEndUpdate() {
			OnLastEndUpdate();
		}
		void IBatchUpdateHandler.OnCancelUpdate() {
			OnCancelUpdate();
		}
		void IBatchUpdateHandler.OnLastCancelUpdate() {
			OnLastCancelUpdate();
		}
		protected virtual void OnFirstBeginUpdate() {
			selectedAppointments.BeginUpdate();
		}
		protected virtual void OnBeginUpdate() {
		}
		protected virtual void OnEndUpdate() {
		}
		protected virtual void OnLastEndUpdate() {
			RemoveUnselectableAppointments();
			ValidateMultiSelect();
			selectedAppointments.EndUpdate();
		}
		protected virtual void OnCancelUpdate() {
		}
		protected virtual void OnLastCancelUpdate() {
			selectedAppointments.CancelUpdate();
		}
		#endregion
		#region IDisposable implementation
		public void Dispose() {
			Dispose(true);
		}
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				if (!isDisposed) {
					UnsubscribeSelectedAppointmentsEvents();
					selectedAppointments.Clear();
					selectedAppointments = null;
				}
			}
			isDisposed = true;
		}
		~AppointmentSelectionController() {
			Dispose(false);
		}
		#endregion
		#region SubscribeUnsubscribeEvents
		protected internal virtual void SubscribeSelectedAppointmentsEvents() {
			selectedAppointments.CollectionChanged += new CollectionChangedEventHandler<Appointment>(OnSelectedAppointmentsChanged);
		}
		protected internal virtual void UnsubscribeSelectedAppointmentsEvents() {
			selectedAppointments.CollectionChanged -= new CollectionChangedEventHandler<Appointment>(OnSelectedAppointmentsChanged);
		}
		#endregion
		#region OnAllowAppointmentMultiselectChanged
		void OnAllowAppointmentMultiselectChanged() {
			BeginUpdate();
			try {
				ValidateMultiSelect();
			}
			finally {
				EndUpdate();
			}
		}
		#endregion
		#region OnSelectedAppointmentsChanged
		protected internal virtual void OnSelectedAppointmentsChanged(object sender, CollectionChangedEventArgs<Appointment> e) {
			OnSelectionChanged();
		}
		#endregion
		#region OnSelectionChanged
		protected internal virtual void OnSelectionChanged() {
			if (!IsUpdateLocked)
				RaiseSelectionChanged();
		}
		#endregion
		#region OnAppointmentsCleared
		protected internal virtual void OnAppointmentsCleared() {
			ClearSelection();
		}
		#endregion
		#region OnAppointmentsLoaded
		protected internal virtual void OnAppointmentsLoaded() {
			ClearSelection();
		}
		#endregion
		#region OnStorageChanged
		protected internal virtual void OnStorageChanged() {
			ClearSelection();
		}
		#endregion
		#region OnAppointmentsInserted
		protected internal virtual void OnAppointmentsInserted(AppointmentBaseCollection insertedAppointments) {
			if (selectedAppointments.Count == 0)
				return;
			BeginUpdate();
			try {
				int count = insertedAppointments.Count;
				for (int i = 0; i < count; i++)
					OnAppointmentInsertedCore(insertedAppointments[i]);
			}
			finally {
				EndUpdate();
			}
		}
		#endregion
		#region OnAppointmentsDeleted
		protected internal virtual void OnAppointmentsDeleted(AppointmentBaseCollection deletedAppointments) {
			if (selectedAppointments.Count == 0)
				return;
			BeginUpdate();
			try {
				int count = deletedAppointments.Count;
				for (int i = 0; i < count; i++)
					OnAppointmentDeletedCore(deletedAppointments[i]);
			}
			finally {
				EndUpdate();
			}
		}
		#endregion
		#region OnAppointmentsChanged
		protected internal virtual void OnAppointmentsChanged(AppointmentBaseCollection changedAppointments) {
			if (selectedAppointments.Count == 0)
				return;
			BeginUpdate();
			try {
				int count = changedAppointments.Count;
				for (int i = 0; i < count; i++)
					OnAppointmentChangedCore(changedAppointments[i]);
			}
			finally {
				EndUpdate();
			}
		}
		#endregion
		#region OnStorageDeferredNotifications
		protected internal virtual void OnStorageDeferredNotifications(SchedulerStorageDeferredChanges deferredChanges) {
			if (deferredChanges.ClearAppointments || deferredChanges.LoadAppointments) {
				BeginUpdate();
				try
				{
					ClearSelection();
				}
				finally {
					CancelUpdate();
				}
				return;
			}
			BeginUpdate();
			try {
				if (deferredChanges.DeletedAppointments.Count > 0)
					OnAppointmentsDeleted(deferredChanges.DeletedAppointments);
				if (deferredChanges.InsertedAppointments.Count > 0)
					OnAppointmentsInserted(deferredChanges.InsertedAppointments);
				if (deferredChanges.ChangedAppointments.Count > 0)
					OnAppointmentsChanged(deferredChanges.ChangedAppointments);
			}
			finally {
				EndUpdate();
			}
		}
		#endregion
		#region OnAppointmentInsertedCore
		protected internal virtual void OnAppointmentInsertedCore(Appointment appointment) {
			if (!appointment.IsException)
				return;
			XtraSchedulerDebug.Assert(appointment.Type != AppointmentType.Occurrence);
			int index = IndexOfSelectedOccurrence(appointment);
			if (index < 0)
				return;
			ReplaceOccurrence(appointment, index);
		}
		#endregion
		#region OnAppointmentDeletedCore
		protected internal virtual void OnAppointmentDeletedCore(Appointment appointment) {
			switch (appointment.Type) {
				case AppointmentType.Normal:
					selectedAppointments.Remove(appointment);
					break;
				case AppointmentType.Pattern:
					RemovePatternOccurrences(appointment);
					break;
				default:
					OnExceptionDeletedCore(appointment);
					break;
			}
		}
		#endregion
		#region OnExceptionDeletedCore
		protected internal virtual void OnExceptionDeletedCore(Appointment occurrence) {
			XtraSchedulerDebug.Assert(occurrence.IsException);
			int index = selectedAppointments.IndexOf(occurrence);
			if (index < 0)
				return;
			Appointment recurrencePattern = occurrence.RecurrencePattern;
			if (recurrencePattern == null || recurrencePattern.Type == AppointmentType.Normal)
				return;
			int recurrenceIndex = selectedAppointments[index].RecurrenceIndex;
			Appointment newOccurrence = recurrencePattern.GetOccurrence(recurrenceIndex);
			if (newOccurrence.Type == AppointmentType.Occurrence && occurrence.Type == AppointmentType.DeletedOccurrence) {
				RemovePatternOccurrences(recurrencePattern);
				return;
			}
			ReplaceOccurrence(newOccurrence, index);
		}
		#endregion
		#region OnAppointmentChangedCore
		protected internal virtual void OnAppointmentChangedCore(Appointment appointment) {
			RemovePatternOccurrences(appointment);
			if (appointment.Type == AppointmentType.Pattern) {
				int index = selectedAppointments.IndexOf(appointment);
				if (index >= 0)
					selectedAppointments.RemoveAt(index);
			}
		}
		#endregion
		#region RemoveUnselectableAppointments
		protected internal virtual void RemoveUnselectableAppointments() {
			int count = selectedAppointments.Count;
			for (int i = count - 1; i >= 0; i--) {
				if (IsUnselectableAppointment(selectedAppointments[i]))
					selectedAppointments.RemoveAt(i);
			}
		}
		#endregion
		void ValidateMultiSelect() {
			int count = selectedAppointments.Count;
			if (allowAppointmentMultiSelect || count <= 1)
				return;
			for (int i = count - 2; i >= 0; i--)
				selectedAppointments.RemoveAt(i);
		}
		#region IsUnselectableAppointment
		protected internal virtual bool IsUnselectableAppointment(Appointment apt) {
			return apt.Type == AppointmentType.DeletedOccurrence || apt.Type == AppointmentType.Pattern ||
				(apt.Type == AppointmentType.ChangedOccurrence && apt.RecurrencePattern == null);
		}
		#endregion
		#region IndexOfSelectedOccurrence
		protected internal virtual int IndexOfSelectedOccurrence(Appointment appointment) {
			Appointment recurrencePattern = appointment.RecurrencePattern;
			if (recurrencePattern == null)
				return -1;
			int recurrenceIndex = appointment.RecurrenceIndex;
			int count = selectedAppointments.Count;
			for (int i = 0; i < count; i++) {
				Appointment selectedAppointment = selectedAppointments[i];
				if (IsTheSameOccurrence(recurrencePattern, recurrenceIndex, selectedAppointment))
					return i;
			}
			return -1;
		}
		#endregion
		#region IsTheSameOccurrence
		protected internal virtual bool IsTheSameOccurrence(Appointment recurrencePattern, int recurrenceIndex, Appointment occurrence) {
			return occurrence.RecurrencePattern == recurrencePattern && occurrence.RecurrenceIndex == recurrenceIndex;
		}
		#endregion
		#region RemovePatternOccurrences
		protected internal virtual void RemovePatternOccurrences(Appointment pattern) {
			int count = selectedAppointments.Count;
			for (int i = count - 1; i >= 0; i--)
				if (selectedAppointments[i].RecurrencePattern == pattern)
					selectedAppointments.RemoveAt(i);
		}
		#endregion
		#region ReplaceOccurrence
		protected internal virtual void ReplaceOccurrence(Appointment newOccurrence, int selectedOccurrenceIndex) {
			XtraSchedulerDebug.Assert(newOccurrence.IsOccurrence);
			XtraSchedulerDebug.Assert(selectedOccurrenceIndex >= 0);
			selectedAppointments.RemoveAt(selectedOccurrenceIndex);
			selectedAppointments.Add(newOccurrence);
		}
		#endregion
		#region ValidateAppointment
		protected internal virtual void ValidateAppointment(Appointment appointment) {
			if (appointment == null)
				Exceptions.ThrowArgumentException("appointment", appointment);
		}
		#endregion
		#region SelectSingleAppointment
		public virtual bool SelectSingleAppointment(Appointment appointment) {
			ValidateAppointment(appointment);
			if (selectedAppointments.Count == 1 && IsAppointmentSelected(appointment))
				return false;
			BeginUpdate();
			try {
				selectedAppointments.Clear();
				selectedAppointments.Add(appointment);
			}
			finally {
				EndUpdate();
			}
			return true;
		}
		#endregion
		#region SelectAppointments
		public virtual void SelectAppointments(AppointmentBaseCollection appointments) {
			BeginUpdate();
			try {
				selectedAppointments.Clear();
				selectedAppointments.AddRange(appointments);
			}
			finally {
				EndUpdate();
			}
		}
		#endregion
		#region ClearSelection
		public virtual bool ClearSelection() {
			if (selectedAppointments.Count == 0)
				return false;
			selectedAppointments.Clear();
			return true;
		}
		#endregion
		#region ChangeSelection
		public virtual bool ChangeSelection(Appointment appointment) {
			ValidateAppointment(appointment);
			int index = IndexOfSelectedAppointment(appointment);
			if (index < 0) {
				BeginUpdate();
				try {
					selectedAppointments.Add(appointment);
				}
				finally {
					EndUpdate();
				}
			}
			else
				selectedAppointments.RemoveAt(index);
			return true;
		}
		#endregion
		#region AddToSelection
		public virtual bool AddToSelection(Appointment appointment) {
			ValidateAppointment(appointment);
			if (IsAppointmentSelected(appointment))
				return false;
			BeginUpdate();
			try {
				selectedAppointments.Add(appointment);
			}
			finally {
				EndUpdate();
			}
			return true;
		}
		#endregion
		#region ApplyChanges
		public virtual bool ApplyChanges(AppointmentSelectionChangeAction changeAction, Appointment appointment) {
			switch (changeAction) {
				case AppointmentSelectionChangeAction.Clear:
					return ClearSelection();
				case AppointmentSelectionChangeAction.Toggle:
					return ChangeSelection(appointment);
				case AppointmentSelectionChangeAction.Add:
					return AddToSelection(appointment);
				case AppointmentSelectionChangeAction.Select:
					return SelectSingleAppointment(appointment);
				case AppointmentSelectionChangeAction.Unselect:
					return Unselect(appointment);
			}
			return false;
		}
		#endregion
		public virtual bool IsAppointmentSelected(Appointment appointment) {
			return IndexOfSelectedAppointment(appointment) >= 0;
		}
		public virtual int IndexOfSelectedAppointment(Appointment appointment) {
			ValidateAppointment(appointment);
			if (appointment.Type == AppointmentType.Normal || appointment.Type == AppointmentType.ChangedOccurrence)
				return selectedAppointments.IndexOf(appointment);
			if (appointment.Type == AppointmentType.Occurrence)
				return IndexOfSelectedOccurrence(appointment);
			return -1;
		}
		public virtual bool Unselect(Appointment appointment) {
			ValidateAppointment(appointment);
			int index = IndexOfSelectedAppointment(appointment);
			if (index < 0)
				return false;
			selectedAppointments.RemoveAt(index);
			return true;
		}
		public virtual void OnVisibleIntervalChanged() {
			ClearSelection();
		}
	}
	#endregion
}
