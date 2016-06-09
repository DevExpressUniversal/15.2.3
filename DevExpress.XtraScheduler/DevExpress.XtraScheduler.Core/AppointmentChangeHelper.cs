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
using DevExpress.XtraScheduler.Drawing;
using DevExpress.XtraScheduler.Native;
using DevExpress.Utils;
using DevExpress.Utils.KeyboardHandler;
using System.Security;
using DevExpress.XtraScheduler.Internal.Implementations;
#if !SILVERLIGHT
using PlatformIndependentDragDropEffects = System.Windows.Forms.DragDropEffects;
using PlatformIndependentPoint = System.Drawing.Point;
using System.Windows.Forms;
using System.Collections.Generic;
using DevExpress.XtraScheduler.Internal.Diagnostics;
#else
using PlatformIndependentDragDropEffects = DevExpress.Utils.DragDropEffects;
using PlatformIndependentPoint = System.Drawing.Point;
using System.Windows;
#endif
namespace DevExpress.XtraScheduler {
	#region SchedulerDragData
	public class SchedulerDragData {
		public static SchedulerDragData GetData(IDataObject dataObject) {
			if (!GetPresent(dataObject))
				return null;
			try {
				return (SchedulerDragData)dataObject.GetData(typeof(SchedulerDragData));
			} catch {
				return null;
			}
		}
		public static bool GetPresent(IDataObject dataObject) {
			if (dataObject == null)
				return false;
			try {
				return dataObject.GetDataPresent(typeof(SchedulerDragData));
			} catch (SecurityException) {
				return false;
			}
		}
		readonly AppointmentBaseCollection appointments;
		int primaryAppointmentIndex;
		public SchedulerDragData(Appointment appointment) {
			if (appointment == null)
				Exceptions.ThrowArgumentException("appointment", appointment);
			this.appointments = new AppointmentBaseCollection();
			this.appointments.Add(appointment);
			this.primaryAppointmentIndex = 0;
		}
		public SchedulerDragData(AppointmentBaseCollection appointments)
			: this(appointments, 0) {
		}
		public SchedulerDragData(AppointmentBaseCollection appointments, int primaryAppointmentIndex) {
			if (appointments == null)
				Exceptions.ThrowArgumentException("appointments", appointments);
			if (primaryAppointmentIndex < 0)
				Exceptions.ThrowArgumentException("primaryAppointmentIndex", primaryAppointmentIndex);
			this.appointments = new AppointmentBaseCollection();
			this.appointments.AddRange(appointments);
			this.primaryAppointmentIndex = primaryAppointmentIndex;
		}
		public AppointmentBaseCollection Appointments { get { return appointments; } }
		public int PrimaryAppointmentIndex { get { return primaryAppointmentIndex; } }
		public Appointment PrimaryAppointment { get { return appointments[primaryAppointmentIndex]; } }		
	}
	#endregion
}
namespace DevExpress.XtraScheduler.Drawing {
}
namespace DevExpress.XtraScheduler.Native {
	#region AppointmentResizingEdge
	public enum AppointmentResizingEdge {
		AppointmentStart,
		AppointmentEnd
	}
	#endregion
	#region AppointmentProcessValues
	public struct AppointmentProcessValues {
		public const int Min = 0;
		public const int Max = 100;
		public const int Step = 0;
	}
	#endregion
	public interface ISchedulerHitInfo {
		PlatformIndependentPoint HitPoint { get; }
		ISelectableIntervalViewInfo ViewInfo { get; }
		bool Contains(SchedulerHitTest types);
		SchedulerHitTest HitTest { get; }
		ISchedulerHitInfo FindFirstLayoutHitInfo();
		ISchedulerHitInfo FindHitInfo(SchedulerHitTest types, SchedulerHitTest stopTypes);
		ISchedulerHitInfo FindHitInfo(SchedulerHitTest types);
		ISchedulerHitInfo NextHitInfo { get; }
	}
	public interface ISelectableIntervalViewInfo {
		TimeInterval Interval { get; }
		Resource Resource { get; }
		bool Selected { get; }
		SchedulerHitTest HitTestType { get; }
	}
	#region AppointmentChangeHelperState
	public class AppointmentChangeHelperState {
		readonly AppointmentChangeHelper changeHelper;
		public AppointmentChangeHelperState(AppointmentChangeHelper changeHelper) {
			Guard.ArgumentNotNull(changeHelper, "changeHelper");
			this.changeHelper = changeHelper;
		}
		#region Properties
		protected internal AppointmentChangeHelper ChangeHelper { get { return changeHelper; } }
		protected internal InnerSchedulerControl Control { get { return changeHelper.Control; } }
		public virtual bool ShowSourceAppointments { get { return true; } }
		public virtual bool ShowEditedAppointment { get { return false; } }
		public virtual bool ActiveState { get { return false; } }
		public virtual bool SelectSourceAppointments { get { return true; } }
		public virtual bool SelectEditedAppointments { get { return false; } }
		public virtual bool HideSelection { get { return false; } }
		#endregion
	}
	#endregion
	#region EditNewAppointmentViaInplaceEditorChangeHelperState
	public class EditNewAppointmentViaInplaceEditorChangeHelperState : AppointmentChangeHelperState {
		public EditNewAppointmentViaInplaceEditorChangeHelperState(AppointmentChangeHelper changeHelper, SafeAppointment newAppointment)
			: base(changeHelper) {
			changeHelper.BeginAppointmentChanges(newAppointment);
		}
		#region Properties
		public override bool ShowSourceAppointments { get { return false; } }
		public override bool ShowEditedAppointment { get { return true; } }
		public override bool ActiveState { get { return true; } }
		public override bool SelectSourceAppointments { get { return false; } }
		public override bool SelectEditedAppointments { get { return true; } }
		public override bool HideSelection { get { return true; } }
		#endregion
		void CommitChanges(object sender, EventArgs e) {
			Control.BeginUpdate();
			try {
				Control.Storage.BeginUpdate();
				try {
					try {
						if (ChangeHelper.EditedAppointments.Count == 0) {
							XtraSchedulerDebug.Assert(false);
						}
						else {
							UnsubscribeEditControllerEvents();
							Appointment newAppointment = ChangeHelper.EditedAppointments[0].Copy();
							Control.Storage.Appointments.Add(newAppointment);							
#if !SILVERLIGHT && !WPF
							Control.AppointmentSelectionController.SelectSingleAppointment(newAppointment);
#endif
						}
					}
					finally {
						ChangeHelper.EndChanges();
					}
				}
				finally {
					Control.Storage.EndUpdate();
				}
			}
			finally {
				Control.EndUpdate();
			}
		}
		void UnsubscribeEditControllerEvents() {
			ISchedulerInplaceEditController controller = Control.InplaceEditController;
			controller.CommitChanges -= new EventHandler(CommitChanges);
			controller.RollbackChanges -= new EventHandler(RollbackChanges);
		}
		void RollbackChanges(object sender, EventArgs e) {
			UnsubscribeEditControllerEvents();
			ChangeHelper.CancelChanges();
		}
		public void Edit() {
			Control.ApplyChangesCore(SchedulerControlChangeType.None, ChangeActions.RecalcViewLayout | ChangeActions.ClearPreliminaryAppointmentsAndCellContainers | ChangeActions.RecalcPreliminaryLayout);
			ISchedulerInplaceEditController controller = Control.InplaceEditController;
			Appointment editedAppointment = ChangeHelper.EditedAppointments[0];
			controller.SetEditedAppointment(editedAppointment);
			controller.CommitChanges += new EventHandler(CommitChanges);
			controller.RollbackChanges += new EventHandler(RollbackChanges);
			controller.Activate();
		}
	}
	#endregion
	#region ResizeAppointmentChangeHelperState
	public class ResizeAppointmentChangeHelperState : AppointmentChangeHelperState {
		readonly AppointmentResizingEdge resizingEdge;
		readonly AppointmentOperationHelper operationHelper;
		public ResizeAppointmentChangeHelperState(AppointmentChangeHelper changeHelper, SafeAppointment sourceAppointment, AppointmentResizingEdge resizingEdge)
			: base(changeHelper) {
			changeHelper.BeginAppointmentChanges(sourceAppointment);
			this.resizingEdge = resizingEdge;
			this.operationHelper = new AppointmentOperationHelper(changeHelper.Control);
		}
		#region Properties
		protected internal Appointment EditedAppointment { get { return ChangeHelper.EditedAppointments[0]; } }
		public override bool ShowSourceAppointments { get { return false; } }
		public override bool ActiveState { get { return true; } }
		#endregion
		public virtual void Resize(TimeInterval layoutInterval, Resource layoutResource, bool isResized) {
			XtraSchedulerDebug.Assert(ChangeHelper.EditedAppointments.Count == 1);
			Appointment editedAppointment = EditedAppointment;
			TimeInterval newAppointmentInterval = CalculateNewEditedAppointmentInterval(layoutInterval);
			editedAppointment.BeginUpdate();
			try {
				editedAppointment.AllDay = newAppointmentInterval.AllDay;
				editedAppointment.Start = newAppointmentInterval.Start;
				editedAppointment.Duration = newAppointmentInterval.Duration;
				ResizedSide resizedSide = (resizingEdge == AppointmentResizingEdge.AppointmentStart) ? ResizedSide.AtStartTime : ResizedSide.AtEndTime;
				AppointmentResizeEventArgs e = new AppointmentResizeEventArgs(ChangeHelper.SourceAppointments[0].Appointment, editedAppointment, layoutInterval, layoutResource, resizedSide);
				RaiseAppointmentResizeEvent(e, isResized);
			}
			finally {
				editedAppointment.EndUpdate();
			}
		}
		protected internal virtual void RaiseAppointmentResizeEvent(AppointmentResizeEventArgs e, bool isResized) {
			bool actualHandled = e.Handled;
			bool actualAllow = e.Allow;
			if (!isResized) {
				operationHelper.RaiseAppointmentResizingEvent(e);
				actualHandled = e.Handled;
				actualAllow = e.Allow;
			}
			else {
				operationHelper.RaiseAppointmentResizingEvent(e);
				actualHandled = e.Handled;
				actualAllow = e.Allow;
				e.ResetHandledState();
				e.Allow = actualAllow;
				operationHelper.RaiseAppointmentResizedEvent(e);
				if (e.Handled) {
					actualHandled = e.Handled;
					actualAllow = e.Allow;
				}
			}
			if (!actualHandled)
				return;
			if (!actualAllow)
				ChangeHelper.DisableEditedAppointment(EditedAppointment);
		}
		protected internal virtual TimeInterval CalculateNewEditedAppointmentInterval(TimeInterval layoutInterval) {
			Appointment editedAppointment = EditedAppointment;
			bool isResultingIntervalAllDay = DateTimeHelper.IsIntervalWholeDays(layoutInterval) && editedAppointment.AllDay;
			TimeInterval actualLayoutInterval = (isResultingIntervalAllDay) ? layoutInterval : ChangeHelper.Control.TimeZoneHelper.FromClientTime(layoutInterval, false);
			TimeInterval editedAppointmentInterval = new TimeInterval(editedAppointment.Start, editedAppointment.End);
			if (editedAppointment.AllDay && !isResultingIntervalAllDay) {
				editedAppointmentInterval = ChangeHelper.Control.TimeZoneHelper.FromClientTime(editedAppointmentInterval, false);
			}
			TimeInterval result = TimeInterval.Empty;
			if (resizingEdge == AppointmentResizingEdge.AppointmentStart)
				result = new TimeInterval(DateTimeHelper.Min(actualLayoutInterval.Start, editedAppointmentInterval.End), editedAppointmentInterval.End);
			else
				result = new TimeInterval(editedAppointmentInterval.Start, DateTimeHelper.Max(actualLayoutInterval.End, editedAppointmentInterval.Start));
			result.AllDay = isResultingIntervalAllDay;
			return result;
		}
		public virtual void Commit() {
			if (ChangeHelper.IsAppointmentDisabled(EditedAppointment))
				return;
			Appointment sourceAppointment = ChangeHelper.SourceAppointments[0].Appointment;
			Control.BeginUpdate();
			try {
				ISchedulerStorageBase storage = Control.Storage;
				storage.BeginUpdate();
				try {
					sourceAppointment.BeginUpdate();
					try {
						sourceAppointment.Assign(EditedAppointment);
					}
					finally {
						sourceAppointment.EndUpdate();
					}
				}
				finally {
					storage.EndUpdate();
				}
			}
			finally {
				Control.EndUpdate();
			}
			Control.MouseHandler.SynchronizeSelectionWithAppointment(sourceAppointment, null);
		}
	}
	#endregion
	#region DragAppointmentChangeHelperState
	public class DragAppointmentChangeHelperState : AppointmentChangeHelperState {
		#region Fields
		readonly SafeAppointment primaryAppointment;
		TimeSpan appointmentDragOffset;
		bool appointmentDragOffsetCorrect;
		readonly AppointmentOperationHelper operationHelper;
		readonly bool internalDragSource;
		bool copy;
		Resource lastHitResource;
		SchedulerGroupType groupType;
		#endregion
		protected internal delegate void CommitAppointmentChangesHandler(Appointment sourceAppointment, Appointment editedAppointment);
		protected internal delegate void DragAppointmentHandler(Appointment sourceAppointment, Appointment editedAppointment, TimeInterval layoutInterval, TimeSpan appointmentsOffset);
		public DragAppointmentChangeHelperState(AppointmentChangeHelper changeHelper, SafeAppointmentCollection sourceAppointments, SafeAppointment primaryAppointment, TimeSpan appointmentDragOffset, bool internalDragSource, SchedulerGroupType groupType)
			: base(changeHelper) {
			changeHelper.BeginChanges(sourceAppointments);
			this.appointmentDragOffset = appointmentDragOffset;
			this.primaryAppointment = primaryAppointment;
			this.internalDragSource = internalDragSource;
			this.appointmentDragOffsetCorrect = internalDragSource;
			this.lastHitResource = ResourceBase.Empty;
			this.operationHelper = new AppointmentOperationHelper(changeHelper.Control);
			this.groupType = groupType;
		}
		#region Properties
		public override bool ShowSourceAppointments { get { return internalDragSource && copy; } }
		public override bool ShowEditedAppointment { get { return !internalDragSource; } }
		public override bool ActiveState { get { return true; } }
		public override bool SelectSourceAppointments { get { return !copy; } }
		protected internal SafeAppointment PrimaryAppointment { get { return primaryAppointment; } }
		protected internal SchedulerGroupType GroupType { get { return groupType; } set { groupType = value; } }
		#endregion
		public virtual void DragTo(TimeInterval layoutInterval, Resource layoutResource, bool copy, bool isDrop) {
			TimeInterval actualLayoutInterval = Control.TimeZoneHelper.FromClientTime(layoutInterval, true);
			if (!appointmentDragOffsetCorrect) {
				appointmentDragOffset = CalculateAppointmentDragOffset(actualLayoutInterval);
				appointmentDragOffsetCorrect = true;
			}
			TimeSpan appointmentsOffset = CalculateAppointmentsOffset(actualLayoutInterval);
			lastHitResource = layoutResource;
			DragAppointments(actualLayoutInterval, layoutResource, appointmentsOffset, isDrop);
			this.copy = copy;
		}
		public virtual void DragOnExternalControl(SchedulerDragData dragData, AppointmentBaseCollection changedAppointments, bool copy, bool isDrop) {
			this.copy = copy;
			XtraSchedulerDebug.Assert(dragData.Appointments.Count == changedAppointments.Count);
			XtraSchedulerDebug.Assert(dragData.Appointments.Count == ChangeHelper.SourceAppointments.Count);
			XtraSchedulerDebug.Assert(dragData.Appointments.Count == ChangeHelper.EditedAppointments.Count);
#if (DEBUG)
			for (int i = 0; i < dragData.Appointments.Count; i++)
				XtraSchedulerDebug.Assert(Object.ReferenceEquals(dragData.Appointments[i], ChangeHelper.SourceAppointments[i].Appointment));
#endif
			AppointmentBaseCollection editedAppointments = ChangeHelper.EditedAppointments;
			SafeAppointmentCollection sourceAppointments = ChangeHelper.SourceAppointments;
			int count = editedAppointments.Count;
			for (int i = 0; i < count; i++) {
				Appointment editedAppointment = editedAppointments[i];
				editedAppointment.BeginUpdate();
				try {
					Appointment changedAppointmnet = changedAppointments[i];
					Appointment sourceAppointment = sourceAppointments[i].Appointment;
					IInternalAppointment editedAppInternal = ((IInternalAppointment)editedAppointment);
					if (editedAppointment.Type != changedAppointmnet.Type)
						editedAppInternal.SetTypeCore(AppointmentType.Normal);
					editedAppointment.AllDay = changedAppointmnet.AllDay;
					editedAppointment.Start = changedAppointmnet.Start;
					editedAppointment.Duration = changedAppointmnet.Duration;
					editedAppointment.ResourceIds.Clear();
					editedAppointment.ResourceIds.AddRange(changedAppointmnet.ResourceIds);
					if (internalDragSource && ResourceBase.InternalAreResourceIdsCollectionsSame(changedAppointmnet.ResourceIds, sourceAppointment.ResourceIds))
						editedAppInternal.SetTypeCore(changedAppointmnet.Type);
					AppointmentDragEventArgs dragEventArgs = new AppointmentDragEventArgs(sourceAppointment, editedAppointment, TimeInterval.Empty, ResourceBase.Empty);
					dragEventArgs.CopyEffect = this.copy;
					RaiseAppointmentDragEvent(dragEventArgs, isDrop);
				}
				finally {
					editedAppointment.EndUpdate();
				}
			}
		}
		protected internal virtual TimeSpan CalculateAppointmentDragOffset(TimeInterval layoutInterval) {
			return layoutInterval.Start.TimeOfDay - primaryAppointment.Appointment.Start.TimeOfDay;
		}
		protected internal virtual void DragAppointments(TimeInterval layoutInterval, Resource layoutResource, TimeSpan appointmentsOffset, bool isDrop) {
			DragAppointmentsCore(layoutInterval, layoutResource, appointmentsOffset, DragAppointment, isDrop);
		}
		protected internal virtual TimeSpan CalculateAppointmentsOffset(TimeInterval layoutInterval) {
			DateTime newPrimaryAppointmentStart = Control.TimeZoneHelper.FromClientTime(layoutInterval.Start) - appointmentDragOffset;
			return newPrimaryAppointmentStart - primaryAppointment.Appointment.Start;
		}
		protected internal virtual void DragAppointmentsCore(TimeInterval layoutInterval, Resource layoutResource, TimeSpan appointmentsOffset, DragAppointmentHandler dragAppointmentHandler, bool isDrop) {
			SafeAppointmentCollection sourceAppointments = ChangeHelper.SourceAppointments;
			AppointmentBaseCollection editedAppointments = ChangeHelper.EditedAppointments;
			int count = sourceAppointments.Count;
			XtraSchedulerDebug.Assert(editedAppointments.Count == count);
			for (int i = 0; i < count; i++) {
				Appointment editedAppointment = editedAppointments[i];
				SafeAppointment sourceAppointment = sourceAppointments[i];
				editedAppointment.BeginUpdate();
				try {
					ChangeResource(sourceAppointment, editedAppointment, layoutResource);
					dragAppointmentHandler(sourceAppointment.Appointment, editedAppointment, layoutInterval, appointmentsOffset);
					AppointmentDragEventArgs ea = new AppointmentDragEventArgs(sourceAppointment.Appointment, editedAppointment, layoutInterval, layoutResource);
					ea.CopyEffect = this.copy;
					AppointmentOperationHelper operationHelper = new AppointmentOperationHelper(Control);
					ea.Allow = operationHelper.IsFitToLimitInterval(editedAppointment);
					RaiseAppointmentDragEvent(ea, isDrop);
					if (!ea.Allow)
						ChangeHelper.DisableEditedAppointment(editedAppointment);
				}
				finally {
					editedAppointments.EndUpdate();
				}
			}
		}
		protected internal virtual void RaiseAppointmentDragEvent(AppointmentDragEventArgs e, bool isDrop) {
			if (!isDrop)
				operationHelper.RaiseAppointmentDragEvent(e);
			else
				operationHelper.RaiseAppointmentDropEvent(e);
			Control.ForceQueryAppointments = e.ForceUpdateFromStorage;
			Appointment editedAppointment = e.EditedAppointment;
			if (e.NewAppointmentResourceIds != null) {
				Appointment sourceAppointment = e.SourceAppointment;
				IInternalAppointment editedAppInternal = (IInternalAppointment)editedAppointment;
				editedAppInternal.SetTypeCore(AppointmentType.Normal);
				editedAppointment.ResourceIds.Clear();
				editedAppointment.ResourceIds.AddRange(e.NewAppointmentResourceIds);
				bool areResourceIdsCollectionsSame = ResourceBase.InternalAreResourceIdsCollectionsSame(sourceAppointment.ResourceIds, e.NewAppointmentResourceIds);
				if (areResourceIdsCollectionsSame)
					editedAppInternal.SetTypeCore(sourceAppointment.Type);
			}
		}
		protected internal virtual void DragAppointment(Appointment sourceAppointment, Appointment editedAppointment, TimeInterval layoutInterval, TimeSpan appointmentsOffset) {
			editedAppointment.Start = sourceAppointment.Start + appointmentsOffset;
			editedAppointment.Duration = sourceAppointment.Duration;
		}
		protected internal virtual void ChangeResource(SafeAppointment sourceAppointment, Appointment editedAppointment, Resource layoutResource) {
			bool isAddResource = KeyboardHandler.IsAltPressed && Control.ResourceSharing;
			IInternalAppointment editedAppInternal = ((IInternalAppointment)editedAppointment);
			if (!isAddResource && ShouldChangeResource(sourceAppointment, layoutResource)) {
				editedAppInternal.SetTypeCore(AppointmentType.Normal);
				editedAppointment.ResourceId = layoutResource.Id;
			}
			else {
				editedAppointment.ResourceIds.Clear();
				editedAppointment.ResourceIds.AddRange(sourceAppointment.Appointment.ResourceIds);
				if (isAddResource) {
					editedAppInternal.SetTypeCore(AppointmentType.Normal);
					editedAppointment.ResourceIds.Add(layoutResource.Id);
				}
				else {
					editedAppInternal.SetTypeCore(sourceAppointment.Appointment.Type);
					AppointmentCopyHelper helper = new AppointmentCopyHelper();
					helper.AssignRecurrenceProperties(sourceAppointment.Appointment, editedAppointment);
				}
			}
		}
		protected internal virtual bool ShouldChangeResource(SafeAppointment sourceAppointment, Resource layoutResource) {
			if (!internalDragSource && GroupType != SchedulerGroupType.None)
				return true;
			if (layoutResource == ResourceBase.Empty || sourceAppointment.Appointment.ResourceId == EmptyResourceId.Id || sourceAppointment.Appointment.ResourceIds.Contains(layoutResource.Id))
				return false;
			return true;
		}
		public virtual void Commit(PlatformIndependentDragDropEffects dropEffect, bool undoChanges) {
			Control.ForceQueryAppointments = false;
			if (dropEffect != PlatformIndependentDragDropEffects.Copy && dropEffect != PlatformIndependentDragDropEffects.Move)
				return;
			bool copyToExternalTarget = internalDragSource && undoChanges && dropEffect == PlatformIndependentDragDropEffects.Copy;
			if (copyToExternalTarget)
				return;
			bool moveFromExternalTarget = dropEffect == PlatformIndependentDragDropEffects.Move && !internalDragSource;
			if (dropEffect == PlatformIndependentDragDropEffects.Copy || moveFromExternalTarget)
				CommitAppointmentsChanges(CopyAppointment);
			else {
				if (dropEffect == PlatformIndependentDragDropEffects.Move) {
					if (!undoChanges) {
						CommitAppointmentsChanges(MoveAppointment);
						Control.MouseHandler.SynchronizeSelectionWithAppointment(primaryAppointment.Appointment, lastHitResource);
					}
					else {
						CommitAppointmentsChanges(DeleteAppointment);
					}
				}
			}
		}
		protected internal virtual void CommitAppointmentsChanges(CommitAppointmentChangesHandler handler) {
			SafeAppointmentCollection sourceAppointments = ChangeHelper.SourceAppointments;
			AppointmentBaseCollection editedAppointments = ChangeHelper.EditedAppointments;
			int count = sourceAppointments.Count;
			XtraSchedulerDebug.Assert(editedAppointments.Count == count);
			ISchedulerStorageBase storage = Control.Storage;
			if (storage == null)
				return;
			Control.BeginUpdate();
			try {
				storage.BeginUpdate();
				try {
					for (int i = 0; i < count; i++)
						handler(sourceAppointments[i].Appointment, editedAppointments[i]);
				}
				finally {
					storage.EndUpdate();
				}
			}
			finally {
				Control.EndUpdate();
			}
		}
		protected internal virtual void CopyAppointment(Appointment sourceAppointment, Appointment editedAppointment) {
			if (ChangeHelper.IsAppointmentDisabled(editedAppointment))
				return;
			AppointmentSelectionController selectionController = Control.AppointmentSelectionController;
			selectionController.Unselect(sourceAppointment);
			Appointment newAppointment = sourceAppointment.Copy();
			if (!Control.MouseHandler.State.IsInternalState)
				newAppointment.SetId(sourceAppointment.Id);
			newAppointment.BeginUpdate();
			try {
				((IInternalAppointment)editedAppointment).SetTypeCore(AppointmentType.Normal);
				newAppointment.Assign(editedAppointment);
			}
			finally {
				newAppointment.EndUpdate();
			}
			if (Control.Storage.Appointments.Add(newAppointment) >= 0) 
				selectionController.AddToSelection(newAppointment);
		}
		protected internal virtual void DeleteAppointment(Appointment sourceAppointment, Appointment editedAppointment) {
			sourceAppointment.Delete();
		}
		protected internal virtual void MoveAppointment(Appointment sourceAppointment, Appointment editedAppointment) {
			if (ChangeHelper.IsAppointmentDisabled(editedAppointment))
				return;
			sourceAppointment.BeginUpdate();
			try {
				if (!ResourceBase.InternalAreResourceIdsCollectionsSame(sourceAppointment.ResourceIds, editedAppointment.ResourceIds)) {
					if (sourceAppointment.Type == AppointmentType.ChangedOccurrence || sourceAppointment.Type == AppointmentType.Occurrence) {
						CopyAppointment(sourceAppointment, editedAppointment);
						sourceAppointment.Delete();
						return;
					}
				}
				IInternalAppointment sourceAppInternal = (IInternalAppointment)sourceAppointment;
				TimeInterval sourceInterval = sourceAppInternal.GetInterval();
				TimeInterval editedInterval = ((IInternalAppointment)editedAppointment).GetInterval();
				if (sourceInterval.Equals(editedInterval) && ResourceBase.InternalAreResourceIdsCollectionsSame(sourceAppointment.ResourceIds, editedAppointment.ResourceIds))
					return;
				sourceAppointment.Assign(editedAppointment);
				if (sourceAppInternal.OnContentChanging(String.Empty, sourceAppointment, sourceAppointment))
					sourceAppInternal.OnContentChanged();
			}
			finally {
				sourceAppointment.EndUpdate();
			}
		}
	}
	#endregion
	#region DragDayViewAppointmentChangeHelperState
	public class DragDayViewAppointmentChangeHelperState : DragAppointmentChangeHelperState {
		public InnerDayView View { get { return Control.ActiveView as InnerDayView; } }
		public DragDayViewAppointmentChangeHelperState(AppointmentChangeHelper changeHelper, SafeAppointmentCollection sourceAppointments, SafeAppointment primaryAppointment, TimeSpan appointmentDragOffset, bool internalDragSource, SchedulerGroupType groupType)
			: base(changeHelper, sourceAppointments, primaryAppointment, appointmentDragOffset, internalDragSource, groupType) {
		}
		protected internal override TimeSpan CalculateAppointmentDragOffset(TimeInterval layoutInterval) {
			if (PrimaryAppointment.Appointment.Duration > DateTimeHelper.DaySpan)
				return -PrimaryAppointment.Appointment.Start.TimeOfDay;
			else
				return TimeSpan.Zero;
		}
		protected internal override void DragAppointments(TimeInterval layoutInterval, Resource layoutResource, TimeSpan appointmentsOffset, bool isDrop) {
			if (layoutInterval.Duration >= DateTimeHelper.DaySpan)
				DragAppointmentsCore(layoutInterval, layoutResource, appointmentsOffset, DragToAllDayArea, isDrop);
			else
				DragAppointmentsCore(layoutInterval, layoutResource, appointmentsOffset, DragToCell, isDrop);
		}
		protected internal override TimeSpan CalculateAppointmentsOffset(TimeInterval layoutInterval) {
			if (layoutInterval.Duration >= DateTimeHelper.DaySpan && PrimaryAppointment.Appointment.Duration < DateTimeHelper.DaySpan)
				return layoutInterval.Start - PrimaryAppointment.Appointment.Start.Date;
			else
				return base.CalculateAppointmentsOffset(layoutInterval);
		}
		protected internal virtual void DragToAllDayArea(Appointment sourceAppointment, Appointment editedAppointment, TimeInterval layoutInterval, TimeSpan appointmentsOffset) {
			if (sourceAppointment.Duration < DateTimeHelper.DaySpan)
				DragShortAppointmentToAllDayArea(sourceAppointment, editedAppointment, appointmentsOffset);
			else
				DragLongAppointmentToAllDayArea(sourceAppointment, editedAppointment, layoutInterval, appointmentsOffset);
		}
		protected internal virtual void DragLongAppointmentToAllDayArea(Appointment sourceAppointment, Appointment editedAppointment, TimeInterval layoutInterval, TimeSpan appointmentsOffset) {
			if (View.ActualShowAllAppointmentsAtTimeCells)
				editedAppointment.AllDay = true;
			else
				editedAppointment.AllDay = sourceAppointment.AllDay;
			editedAppointment.Start = sourceAppointment.Start + CalculateLongAppointmentOffset(editedAppointment.AllDay, layoutInterval, appointmentsOffset);
			editedAppointment.Duration = sourceAppointment.Duration;
		}
		protected internal virtual void DragShortAppointmentToAllDayArea(Appointment sourceAppointment, Appointment editedAppointment, TimeSpan appointmentsOffset) {
			editedAppointment.AllDay = true;
			editedAppointment.Start = sourceAppointment.Start.Date + appointmentsOffset;
			editedAppointment.Duration = DateTimeHelper.DaySpan;
		}
		protected internal virtual void DragToCell(Appointment sourceAppointment, Appointment editedAppointment, TimeInterval layoutInterval, TimeSpan appointmentsOffset) {
			if (sourceAppointment.Duration < DateTimeHelper.DaySpan)
				DragShortAppointmentToCell(sourceAppointment, editedAppointment, appointmentsOffset);
			else
				DragLongAppointmentToCell(sourceAppointment, editedAppointment, layoutInterval, appointmentsOffset);
		}
		protected internal virtual void DragLongAppointmentToCell(Appointment sourceAppointment, Appointment editedAppointment, TimeInterval layoutInterval, TimeSpan appointmentsOffset) {
			if (View.ActualShowAllAppointmentsAtTimeCells) {
				editedAppointment.AllDay = sourceAppointment.AllDay;
				editedAppointment.Start = sourceAppointment.Start + CalculateLongAppointmentOffset(editedAppointment.AllDay, layoutInterval, appointmentsOffset);
				editedAppointment.Duration = sourceAppointment.Duration;
			}
			else {
				editedAppointment.AllDay = false;
				editedAppointment.Start = layoutInterval.Start;
				editedAppointment.Duration = layoutInterval.Duration;
			}
		}
		protected internal virtual TimeSpan CalculateLongAppointmentOffset(bool allDay, TimeInterval layoutInterval, TimeSpan appointmentsOffset) {
			if (!allDay)
				return appointmentsOffset;
			else {
				DateTime dragStart = layoutInterval.Start.Add(-appointmentsOffset);
				long daysDifference = CalculateDaysDifference(dragStart, layoutInterval.Start);
				return TimeSpan.FromDays(daysDifference);
			}
		}
		protected internal virtual long CalculateDaysDifference(DateTime date1, DateTime date2) {
			return (date2.Date.Ticks - date1.Date.Ticks) / TimeSpan.FromDays(1).Ticks;
		}
		protected internal virtual void DragShortAppointmentToCell(Appointment sourceAppointment, Appointment editedAppointment, TimeSpan appointmentsOffset) {
			editedAppointment.AllDay = false;
			editedAppointment.Start = sourceAppointment.Start + appointmentsOffset;
			editedAppointment.Duration = sourceAppointment.Duration;
		}
	}
	#endregion
	#region DragTimelineViewAppointmentChangeHelperState
	public class DragTimelineViewAppointmentChangeHelperState : DragAppointmentChangeHelperState {
		public DragTimelineViewAppointmentChangeHelperState(AppointmentChangeHelper changeHelper, SafeAppointmentCollection sourceAppointments, SafeAppointment primaryAppointment, TimeSpan appointmentDragOffset, bool internalDragSource, SchedulerGroupType groupType)
			: base(changeHelper, sourceAppointments, primaryAppointment, appointmentDragOffset, internalDragSource, groupType) {
		}
		protected internal override TimeSpan CalculateAppointmentDragOffset(TimeInterval layoutInterval) {
			return TimeSpan.Zero;
		}
	}
	#endregion
	public interface IAppointmentVisualStateCalculator {
		bool IsSelected(Appointment apt);
		bool IsDisabled(Appointment apt);
	}
	public class DefaultAppointmentVisualStateCalculator : IAppointmentVisualStateCalculator {
		InnerSchedulerControl innerControl;
		public DefaultAppointmentVisualStateCalculator(InnerSchedulerControl innerControl) {
			Guard.ArgumentNotNull(innerControl, "innerControl");
			this.innerControl = innerControl;
		}
		public bool IsSelected(Appointment apt) {
			AppointmentSelectionController selectionController = innerControl.AppointmentSelectionController;
			AppointmentBaseCollection selectedAppointments = selectionController.SelectedAppointments;
			return this.innerControl.AppointmentChangeHelper.IsSelected(apt, selectedAppointments);
		}
		public bool IsDisabled(Appointment apt) {
			return this.innerControl.AppointmentChangeHelper.IsAppointmentDisabled(apt);
		}
	}
	#region AppointmentChangeHelper
	public class AppointmentChangeHelper {
		#region Fields
		readonly SafeAppointmentCollection sourceAppointments;
		readonly AppointmentBaseCollection editedAppointments;
		readonly InnerSchedulerControl control;
		AppointmentChangeHelperState state;
		bool[] disabledAppointments;
		bool undoChanges;
		ISelectableIntervalViewInfo lastLayoutViewInfo;
		#endregion
		public AppointmentChangeHelper(InnerSchedulerControl control) {
			Guard.ArgumentNotNull(control, "control");
			this.sourceAppointments = new SafeAppointmentCollection();
			this.editedAppointments = new AppointmentBaseCollection();
			this.control = control;
			state = new AppointmentChangeHelperState(this);
		}
		#region Properties
		public bool Active { get { return state.ActiveState; } }
		public SafeAppointmentCollection SourceAppointments { get { return sourceAppointments; } }
		public AppointmentBaseCollection EditedAppointments { get { return editedAppointments; } }
		public bool HideSelection { get { return state.HideSelection; } }
		public InnerSchedulerControl Control { get { return control; } }
		protected bool ShouldUndoChanges { get { return undoChanges; } }
		protected internal AppointmentChangeHelperState State { get { return state; } set { state = value; } }
		internal ISelectableIntervalViewInfo LastLayoutViewInfo { get { return lastLayoutViewInfo; } set { lastLayoutViewInfo = value; } }
		#endregion
		public virtual void BeginAppointmentsResize(SafeAppointmentCollection sourceAppointments, AppointmentResizingEdge resizingEdge) {
			if (sourceAppointments.Count != 1)
				Exceptions.ThrowArgumentException("sourceAppointments.Count", sourceAppointments.Count);
			state = new ResizeAppointmentChangeHelperState(this, sourceAppointments[0], resizingEdge);
		}
		public virtual void BeginInternalDragDrop(SchedulerDragData dragData, ISchedulerHitInfo aptHitInfo) {
			ISchedulerHitInfo cellHitInfo = aptHitInfo.FindFirstLayoutHitInfo();
			XtraSchedulerDebug.Assert(!cellHitInfo.ViewInfo.Interval.Equals(TimeInterval.Empty));
			SafeAppointment primaryAppointment = new SafeAppointment(dragData.PrimaryAppointment, Control.Storage);
			TimeSpan appointmentDragOffset = Control.TimeZoneHelper.FromClientTime(cellHitInfo.ViewInfo.Interval.Start) - primaryAppointment.Appointment.Start;
			BeginInternalDragCommand(cellHitInfo, aptHitInfo, new SafeAppointmentCollection(dragData.Appointments, Control.Storage), primaryAppointment, appointmentDragOffset);
		}
		protected internal virtual DragAppointmentChangeHelperState CreateDragAppointmentChangeHelperState(SafeAppointmentCollection sourceAppointments, SafeAppointment primaryAppointment, TimeSpan appointmentDragOffset, bool internalDragSource) {
			if (Control.ActiveViewType == SchedulerViewType.Day || Control.ActiveViewType == SchedulerViewType.WorkWeek || Control.ActiveViewType == SchedulerViewType.FullWeek)
				return new DragDayViewAppointmentChangeHelperState(this, sourceAppointments, primaryAppointment, appointmentDragOffset, internalDragSource, control.GroupType);
			if (Control.ActiveViewType == SchedulerViewType.Timeline || Control.ActiveViewType == SchedulerViewType.Gantt)
				return new DragTimelineViewAppointmentChangeHelperState(this, sourceAppointments, primaryAppointment, appointmentDragOffset, internalDragSource, control.GroupType);
			else
				return new DragAppointmentChangeHelperState(this, sourceAppointments, primaryAppointment, appointmentDragOffset, internalDragSource, control.GroupType);
		}
		public virtual void BeginInternalDragCommand(ISchedulerHitInfo hitInfo, ISchedulerHitInfo aptHitInfo, SafeAppointmentCollection sourceAppointments, SafeAppointment primaryAppointment, TimeSpan appointmentDragOffset) {
			state = CreateDragAppointmentChangeHelperState(sourceAppointments, primaryAppointment, appointmentDragOffset, true);
		}
		public virtual void BeginExternalDragDrop(SchedulerDragData dragData) {
			state = CreateDragAppointmentChangeHelperState(new SafeAppointmentCollection(dragData.Appointments, Control.Storage), new SafeAppointment(dragData.PrimaryAppointment, Control.Storage), TimeSpan.Zero, false);
		}
		public virtual void CommitDrag(PlatformIndependentDragDropEffects dropEffect) {
			CommitDrag(dropEffect, false);
		}
		public virtual void CommitDrag(PlatformIndependentDragDropEffects dropEffect, bool skipRaiseDropEvent) {
			if (Active) {
				if (dropEffect != PlatformIndependentDragDropEffects.None) {
					if (!skipRaiseDropEvent && lastLayoutViewInfo != null && !undoChanges && !PerformDragDrop(lastLayoutViewInfo, dropEffect, true)) {
						CancelChanges();
						return;
					}
				}
				Control.BeginUpdate();
				try {
					DragAppointmentChangeHelperState dragState = (DragAppointmentChangeHelperState)state;
					try {
						dragState.Commit(dropEffect, undoChanges);
					}
					finally {
						EndChanges();
					}
				}
				finally {
					Control.EndUpdate();
				}
			}
		}
		public virtual void CommitResize() {
			if (Active) {
				if (lastLayoutViewInfo != null && !undoChanges && !PerformResize(lastLayoutViewInfo, true)) {
					CancelChanges();
					return;
				}
			}
			try {
				if (!undoChanges && Active) {
					ResizeAppointmentChangeHelperState resizeState = (ResizeAppointmentChangeHelperState)state;
					resizeState.Commit();
				}
			}
			finally {
				EndChanges();
			}
		}
		public virtual void CancelChanges() {
			if (!Active)
				return;
			control.BeginUpdate();
			try {
				EndChanges();
			}
			finally {
				control.EndUpdate();
			}
		}
		protected internal virtual void EndChanges() {
			state = new AppointmentChangeHelperState(this);
			int count = editedAppointments.Count;
			for (int i = 0; i < count; i++)
				editedAppointments[i].Dispose();
			editedAppointments.Clear();
			sourceAppointments.Clear();
			control.ApplyChangesCore(SchedulerControlChangeType.None, ChangeActions.RecalcViewLayout | ChangeActions.ClearPreliminaryAppointmentsAndCellContainers | ChangeActions.RecalcPreliminaryLayout);
		}
		public AppointmentBaseCollection GetChangedAppointments() {
			AppointmentBaseCollection result = new AppointmentBaseCollection();
			int count = editedAppointments.Count;
			for (int i = 0; i < count; i++)
				result.Add(((IInternalAppointment)editedAppointments[i]).CopyCore());
			return result;
		}
		protected internal virtual bool ValidateOperation(bool copy) {
			AppointmentOperationHelper operationHelper = new AppointmentOperationHelper(control);
			int count = sourceAppointments.Count;
			bool disableAllOperation = true;
			for (int i = 0; i < count; i++) {
				if (!disabledAppointments[i])
					disabledAppointments[i] = !CanChangeAppointment(operationHelper, sourceAppointments[i].Appointment, editedAppointments[i], copy);
				disableAllOperation &= disabledAppointments[i];
			}
			return !disableAllOperation;
		}
		protected internal virtual bool CanChangeAppointment(AppointmentOperationHelper operationHelper, Appointment sourceAppointment, Appointment editedAppointment, bool copy) {
			if (copy && !operationHelper.CanCopyAppointment(sourceAppointment))
				return false;
			bool sameResources = ResourceBase.InternalAreResourceIdsCollectionsSame(sourceAppointment.ResourceIds, editedAppointment.ResourceIds);
			if (!sameResources && !operationHelper.CanDragAppointmentBetweenResources(sourceAppointment, false))
				return false;
			return operationHelper.IsConflictResolved(editedAppointment, sourceAppointment, copy);
		}
		public virtual void Resize(ISelectableIntervalViewInfo layoutViewInfo) {
			this.lastLayoutViewInfo = layoutViewInfo;
			PerformResize(layoutViewInfo, false);
		}
		protected virtual bool PerformResize(ISelectableIntervalViewInfo layoutViewInfo, bool isResized) {
			undoChanges = false;
			EnableAllEditedAppointments();
			ResizeAppointmentChangeHelperState resizeState = (ResizeAppointmentChangeHelperState)state;
			resizeState.Resize(layoutViewInfo.Interval, layoutViewInfo.Resource, isResized);
			return ValidateOperation(false);
		}
		public virtual bool Drag(ISelectableIntervalViewInfo layoutViewInfo, PlatformIndependentDragDropEffects effect, PlatformIndependentPoint hitPoint) {
			this.lastLayoutViewInfo = layoutViewInfo;
			return PerformDragDrop(LastLayoutViewInfo, effect, false);
		}
		protected virtual bool PerformDragDrop(ISelectableIntervalViewInfo layoutViewInfo, PlatformIndependentDragDropEffects effect, bool isDrop) {
			undoChanges = false;
			EnableAllEditedAppointments();
			DragAppointmentChangeHelperState dragState = (DragAppointmentChangeHelperState)state;
			bool copy = (effect == PlatformIndependentDragDropEffects.Copy);
			dragState.DragTo(layoutViewInfo.Interval, layoutViewInfo.Resource, copy, isDrop);
			return ValidateOperation(copy);
		}
		public bool DragOnExternalControl(SchedulerDragData dragData, AppointmentBaseCollection changedAppointments, bool copy, bool isDrop) {
			undoChanges = false;
			EnableAllEditedAppointments();
			DragAppointmentChangeHelperState dragState = (DragAppointmentChangeHelperState)state;
			dragState.DragOnExternalControl(dragData, changedAppointments, copy, isDrop);
			return ValidateOperation(copy);
		}
		protected internal virtual void EnableAllEditedAppointments() {
			int count = editedAppointments.Count;
			for (int i = 0; i < count; i++)
				disabledAppointments[i] = false;
		}
		protected internal virtual Appointment GetEditedAppointment(Appointment sourceAppointment) {
			int index = SourceAppointments.FindIndex(safeApt => safeApt.Appointment == sourceAppointment);
			if (index < 0)
				return null;
			if (sourceAppointment.Id == null)
				return editedAppointments[index];
			return editedAppointments.Find(apt => Object.Equals(apt.Id, sourceAppointment.Id));
		}
		protected internal virtual AppointmentBaseCollection GetActualVisibleAppointments(AppointmentBaseCollection sourceAppointments) {
			AppointmentBaseCollection appointments = (!Active || undoChanges) ? new AppointmentBaseCollection(DXCollectionUniquenessProviderType.None) : new AppointmentBaseCollection();
			appointments.AddRange(sourceAppointments);
			if (!Active || undoChanges)
				return appointments;
			appointments.AddRange(GetVisibleEditedAppointments());
			if (state.ShowEditedAppointment || state.ShowSourceAppointments)
				return appointments;
			int count = sourceAppointments.Count;
			for (int i = count - 1; i >= 0; i--) {
				Appointment sourceAppointment = appointments[i];
				if (sourceAppointment.IsDisposed)
					continue;
				Appointment editedAppointment = GetEditedAppointment(sourceAppointment);
				if (editedAppointment != null)
					appointments.Remove(sourceAppointment);
			}
			return appointments;	
		}
		protected internal virtual AppointmentBaseCollection GetActualAppointments(AppointmentBaseCollection sourceAppointments) {
			AppointmentBaseCollection appointments = (!Active || undoChanges) ? new AppointmentBaseCollection(DXCollectionUniquenessProviderType.None) : new AppointmentBaseCollection();
			appointments.AddRange(sourceAppointments);
			if (!Active || undoChanges)
				return appointments;
			if (state.ShowEditedAppointment || state.ShowSourceAppointments)
				return appointments;
			int count = appointments.Count;
			for (int i = count - 1; i >= 0; i--) {
				Appointment sourceAppointment = appointments[i];
				if (sourceAppointment.IsDisposed)
					continue;
				Appointment editedAppointment = GetEditedAppointment(sourceAppointment);
				if (editedAppointment != null) {
					appointments.Remove(sourceAppointment);
					appointments.Add(editedAppointment);
				}
			}
			return appointments;
		}
		protected virtual AppointmentBaseCollection GetVisibleEditedAppointments() {
			AppointmentBaseCollection result = new AppointmentBaseCollection();
			ResourceIdCollection visibleResourceIds = new ResourceIdCollection();
			ResourceBaseCollection visibleResources = Control.ActiveView.VisibleResources;
			int count = visibleResources.Count;
			for (int i = 0; i < count; i++)
				visibleResourceIds.Add(visibleResources[i].Id);
			count = editedAppointments.Count;
			bool showOnlyResourceAppointments = control.OptionsView.ShowOnlyResourceAppointments;
			for (int i = 0; i < count; i++) {
				if ((editedAppointments[i].ResourceId == EmptyResourceId.Id && !showOnlyResourceAppointments) ||
					ResourceBase.InternalAreResourceIdsCollectionsIntersect(visibleResourceIds, editedAppointments[i].ResourceIds))
					result.Add(editedAppointments[i]);
			}
			return result;
		}
		public virtual void UndoChanges() {
			undoChanges = true;
		}
		protected internal virtual bool IsSelected(Appointment appointment, AppointmentBaseCollection selectedAppointments) {
			if (!Active) {
				return control.AppointmentSelectionController.IsAppointmentSelected(appointment);
			}
			if (undoChanges)
				return false;
			int index = editedAppointments.IndexOf(appointment);
			if (index >= 0)
				return state.SelectEditedAppointments || selectedAppointments.Contains(sourceAppointments[index].Appointment);
			else {
				if (state.SelectSourceAppointments) {
					return control.AppointmentSelectionController.IsAppointmentSelected(appointment);
				}
				else
					return false;
			}
		}
		protected internal virtual void BeginChangesCore(SafeAppointment sourceAppointment) {
			sourceAppointments.Add(sourceAppointment);
			Appointment editedAppointment = ((IInternalAppointment)sourceAppointment.Appointment).CopyCore();
			editedAppointment.RowHandle = sourceAppointment.Appointment.RowHandle;
			editedAppointment.SetId(sourceAppointment.Appointment.Id);
			editedAppointments.Add(editedAppointment);
		}
		protected internal virtual void BeginAppointmentChanges(SafeAppointment sourceAppointment) {
			disabledAppointments = new bool[1];
			BeginChangesCore(sourceAppointment);
		}
		protected internal virtual void BeginChanges(SafeAppointmentCollection sourceAppointments) {
			disabledAppointments = new bool[sourceAppointments.Count];
			int count = sourceAppointments.Count;
			for (int i = 0; i < count; i++)
				BeginChangesCore(sourceAppointments[i]);
		}
		protected internal virtual void DisableEditedAppointment(Appointment editedAppointment) {
			int index = editedAppointments.IndexOf(editedAppointment);
			if (index >= 0)
				disabledAppointments[index] = true;
		}
		protected internal virtual bool IsAppointmentDisabled(Appointment editedAppointment) {
			if (!Active)
				return false;
			int index = editedAppointments.IndexOf(editedAppointment);
			if (index >= 0)
				return disabledAppointments[index];
			else
				return false;
		}
		public void BeginEditNewAppointment(Appointment apt) {
			if (this.state.ActiveState)
				return;
			undoChanges = false;
			SafeAppointment safeAPpointment = new SafeAppointment(apt, null);
			EditNewAppointmentViaInplaceEditorChangeHelperState editState = new EditNewAppointmentViaInplaceEditorChangeHelperState(this, safeAPpointment);
			this.state = editState;
			editState.Edit();
		}		
	}
	#endregion
}
