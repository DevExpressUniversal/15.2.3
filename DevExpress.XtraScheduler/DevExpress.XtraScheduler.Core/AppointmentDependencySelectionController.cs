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
namespace DevExpress.XtraScheduler {
	#region AppointmentDependencySelectionChangeAction
	public enum AppointmentDependencySelectionChangeAction {
		None,
		Clear,
		Select,
		Unselect,
		Add,
		Toggle
	}
	#endregion
}
namespace DevExpress.XtraScheduler.Native {
	#region AppointmentDependencySelectionController
	public class AppointmentDependencySelectionController : IDisposable, IBatchUpdateable, IBatchUpdateHandler {
		#region Fields
		AppointmentDependencyBaseCollection selectedDependencies;
		bool isDisposed;
		BatchUpdateHelper batchUpdateHelper;
		bool allowAppointmentDependencyMultiSelect;
		#endregion
		public AppointmentDependencySelectionController(bool allowAppointmentDependencyMultiSelect) {
			this.batchUpdateHelper = new BatchUpdateHelper(this);
			selectedDependencies = new AppointmentDependencyBaseCollection();
			SubscribeSelectedAppointmentDependenciesEvents();
			this.allowAppointmentDependencyMultiSelect = allowAppointmentDependencyMultiSelect;
		}
		#region Proprties
		internal bool IsDisposed { get { return isDisposed; } }
		public AppointmentDependencyBaseCollection SelectedDependencies { get { return selectedDependencies; } }
		#region AllowAppointmentDependencyMultiSelect
		public bool AllowAppointmentDependenciesMultiSelect {
			get { return allowAppointmentDependencyMultiSelect; }
			set {
				if (allowAppointmentDependencyMultiSelect == value)
					return;
				allowAppointmentDependencyMultiSelect = value;
				OnAllowAppointmentMultiselectChanged();
			}
		}
		#endregion
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
			selectedDependencies.BeginUpdate();
		}
		protected virtual void OnBeginUpdate() {
		}
		protected virtual void OnEndUpdate() {
		}
		protected virtual void OnLastEndUpdate() {
			RemoveUnselectableDependencies();
			ValidateMultiSelect();
			selectedDependencies.EndUpdate();
		}
		protected virtual void OnCancelUpdate() {
		}
		protected virtual void OnLastCancelUpdate() {
			selectedDependencies.CancelUpdate();
		}
		#endregion
		#region IDisposable implementation
		public void Dispose() {
			Dispose(true);
		}
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				if (!isDisposed) {
					UnsubscribeSelectedAppointmentDependenciesEvents();
					selectedDependencies.Clear();
					selectedDependencies = null;
				}
			}
			isDisposed = true;
		}
		~AppointmentDependencySelectionController() {
			Dispose(false);
		}
		#endregion
		#region SubscribeUnsubscribeEvents
		protected internal virtual void SubscribeSelectedAppointmentDependenciesEvents() {
			selectedDependencies.CollectionChanged += new CollectionChangedEventHandler<AppointmentDependency>(OnSelectedDependenciesChanged);
		}
		protected internal virtual void UnsubscribeSelectedAppointmentDependenciesEvents() {
			selectedDependencies.CollectionChanged -= new CollectionChangedEventHandler<AppointmentDependency>(OnSelectedDependenciesChanged);
		}
		#endregion
		#region OnSelectedDependenciesChanged
		protected internal virtual void OnSelectedDependenciesChanged(object sender, CollectionChangedEventArgs<AppointmentDependency> e) {
			OnSelectionChanged();
		}
		#endregion
		#region OnSelectionChanged
		protected internal virtual void OnSelectionChanged() {
			if (!IsUpdateLocked)
				RaiseSelectionChanged();
		}
		#endregion
		#region Events
		EventHandler onSelectionChanged;
		protected internal event EventHandler SelectionChanged { add { onSelectionChanged += value; } remove { onSelectionChanged -= value; } }
		protected internal virtual void RaiseSelectionChanged() {
			if (onSelectionChanged != null)
				onSelectionChanged(this, EventArgs.Empty);
		}
		#endregion
		#region ApplyChanges
		public virtual bool ApplyChanges(AppointmentDependencySelectionChangeAction changeAction, AppointmentDependency appointmentDependency) {
			switch (changeAction) {
				case AppointmentDependencySelectionChangeAction.Clear:
					return ClearSelectionCore();
				case AppointmentDependencySelectionChangeAction.Toggle:
					return ChangeSelection(appointmentDependency);
				case AppointmentDependencySelectionChangeAction.Add:
					return AddToSelection(appointmentDependency);
				case AppointmentDependencySelectionChangeAction.Select:
					return SelectSingleAppointmentDependency(appointmentDependency);
				case AppointmentDependencySelectionChangeAction.Unselect:
					return Unselect(appointmentDependency);
			}
			return false;
		}
		#endregion
		#region ValidateMultiSelect
		void ValidateMultiSelect() {
			int count = selectedDependencies.Count;
			if (allowAppointmentDependencyMultiSelect || count <= 1)
				return;
			for (int i = count - 2; i >= 0; i--)
				selectedDependencies.RemoveAt(i);
		}
		#endregion
		#region ValidateAppointmentDependency
		protected internal virtual void ValidateAppointmentDependency(AppointmentDependency appointmentDependency) {
			if (appointmentDependency == null)
				Exceptions.ThrowArgumentException("appointmentDependency", appointmentDependency);
		}
		#endregion
		#region SelectSingleAppointment
		public virtual bool SelectSingleAppointmentDependency(AppointmentDependency appointmentDependency) {
			ValidateAppointmentDependency(appointmentDependency);
			if (selectedDependencies.Count == 1 && IsAppointmentDependencySelected(appointmentDependency))
				return false;
			BeginUpdate();
			try {
				selectedDependencies.Add(appointmentDependency);
			}
			finally {
				EndUpdate();
			}
			return true;
		}
		#endregion
		#region SelectAppointments
		public virtual void SelectDependencies(AppointmentDependencyBaseCollection appointmentDependencies) {
			BeginUpdate();
			try {
				selectedDependencies.Clear();
				selectedDependencies.AddRange(appointmentDependencies);
			}
			finally {
				EndUpdate();
			}
		}
		#endregion
		#region ClearSelection
		public virtual bool ClearSelection() {
			return ApplyChanges(AppointmentDependencySelectionChangeAction.Clear, null); 
		}
		private bool ClearSelectionCore() {
			if (selectedDependencies.Count == 0)
				return false;
			selectedDependencies.Clear();
			return true;
		}
		#endregion
		#region ChangeSelection
		public virtual bool ChangeSelection(AppointmentDependency appointmentDependency) {
			ValidateAppointmentDependency(appointmentDependency);
			int index = IndexOfSelectedAppointmentDependency(appointmentDependency);
			if (index < 0) {
				BeginUpdate();
				try {
					selectedDependencies.Add(appointmentDependency);
				}
				finally {
					EndUpdate();
				}
			}
			else
				selectedDependencies.RemoveAt(index);
			return true;
		}
		#endregion
		#region AddToSelection
		public virtual bool AddToSelection(AppointmentDependency appointmentDependency) {
			ValidateAppointmentDependency(appointmentDependency);
			if (IsAppointmentDependencySelected(appointmentDependency))
				return false;
			BeginUpdate();
			try {
				selectedDependencies.Add(appointmentDependency);
			}
			finally {
				EndUpdate();
			}
			return true;
		}
		#endregion
		#region IsAppointmentDependencySelected
		public virtual bool IsAppointmentDependencySelected(AppointmentDependency appointmentDependency) {
			if (appointmentDependency == null)
				return false;
			return IndexOfSelectedAppointmentDependency(appointmentDependency) >= 0;
		}
		#endregion
		#region IndexOfSelectedAppointmentDependency
		public virtual int IndexOfSelectedAppointmentDependency(AppointmentDependency appointmentDependency) {
			ValidateAppointmentDependency(appointmentDependency);
			return selectedDependencies.IndexOf(appointmentDependency);
		}
		#endregion
		#region Unselect
		public virtual bool Unselect(AppointmentDependency appointmentDependency) {
			ValidateAppointmentDependency(appointmentDependency);
			int index = IndexOfSelectedAppointmentDependency(appointmentDependency);
			if (index < 0)
				return false;
			selectedDependencies.RemoveAt(index);
			return true;
		}
		#endregion
		#region OnAppointmentDependenciesCleared
		protected internal virtual void OnAppointmentDependenciesCleared() {
			ClearSelection();
		}
		#endregion
		#region OnAppointmentDependenciesLoaded
		protected internal virtual void OnAppointmentDependenciesLoaded() {
			ClearSelection();
		}
		#endregion
		#region OnStorageChanged
		protected internal virtual void OnStorageChanged() {
			ClearSelection();
		}
		#endregion
		#region OnAppointmentDependenciesInserted
		protected internal virtual void OnAppointmentDependenciesInserted(AppointmentDependencyBaseCollection insertedDependencies) {
			if (selectedDependencies.Count == 0)
				return;
			BeginUpdate();
			try {
				int count = insertedDependencies.Count;
				for (int i = 0; i < count; i++)
					OnAppointmentDependencyInsertedCore(insertedDependencies[i]);
			}
			finally {
				EndUpdate();
			}
		}
		#endregion
		#region OnAppointmentDependenciesDeleted
		protected internal virtual void OnAppointmentDependenciesDeleted(AppointmentDependencyBaseCollection deletedDependencies) {
			if (selectedDependencies.Count == 0)
				return;
			BeginUpdate();
			try {
				int count = deletedDependencies.Count;
				for (int i = 0; i < count; i++)
					OnAppointmentDependencyDeletedCore(deletedDependencies[i]);
			}
			finally {
				EndUpdate();
			}
		}
		#endregion
		#region OnAppointmentDependenciesChanged
		protected internal virtual void OnAppointmentDependenciesChanged(AppointmentDependencyBaseCollection changedDependencies) {
			if (selectedDependencies.Count == 0)
				return;
			BeginUpdate();
			try {
				int count = changedDependencies.Count;
				for (int i = 0; i < count; i++)
					OnAppointmentDependencyChangedCore(changedDependencies[i]);
			}
			finally {
				EndUpdate();
			}
		}
		#endregion
		#region OnVisibleIntervalChanged
		public virtual void OnVisibleIntervalChanged() {
		}
		#endregion
		#region  Difference between AppointmentSelectionController
		#region OnStorageDeferredNotifications
		public void OnStorageDeferredNotifications(SchedulerStorageDeferredChanges deferredChanges) {
			if (deferredChanges.ClearDependencies || deferredChanges.LoadDependencies) {
				ClearSelection();
				return;
			}
			BeginUpdate();
			try {
				if (deferredChanges.DeletedDependencies.Count > 0)
					OnAppointmentDependenciesDeleted(deferredChanges.DeletedDependencies);
				if (deferredChanges.InsertedDependencies.Count > 0)
					OnAppointmentDependenciesInserted(deferredChanges.InsertedDependencies);
				if (deferredChanges.ChangedDependencies.Count > 0)
					OnAppointmentDependenciesChanged(deferredChanges.ChangedDependencies);
			}
			finally {
				EndUpdate();
			}
		}
		#endregion
		#region OnAppointmentDependencyInsertedCore
		protected internal virtual void OnAppointmentDependencyInsertedCore(AppointmentDependency appointmentDependency) {
		}
		#endregion
		#region OnAppointmentDependencyChangedCore
		protected internal virtual void OnAppointmentDependencyChangedCore(AppointmentDependency appointmentDependency) {
		}
		#endregion
		#region OnAppointmentDependencyDeletedCore
		protected internal virtual void OnAppointmentDependencyDeletedCore(AppointmentDependency appointmentDependency) {
			selectedDependencies.Remove(appointmentDependency);
		}
		#endregion
		#region RemoveUnselectableDependencies
		protected internal virtual void RemoveUnselectableDependencies() {
		}
		#endregion
		#endregion
	}
	#endregion
}
