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

using DevExpress.XtraScheduler.Internal.Diagnostics;
using System;
using DevExpress.XtraScheduler.Commands;
namespace DevExpress.XtraScheduler.Native {
	#region ISchedulerControlChangeTarget
	public interface ISchedulerControlChangeTarget {
		void UpdatePaintStyle();
		bool IsDateTimeScrollbarVisibilityDependsOnClientSize();
		void RecalcClientBounds();
		bool ChangeResourceScrollBarOrientationIfNeeded();
		bool ChangeDateTimeScrollBarOrientationIfNeeded();
		bool ChangeResourceScrollBarVisibilityIfNeeded();
		bool ChangeDateTimeScrollBarVisibilityIfNeeded();
		void RecalcViewBounds();
		void RecalcScrollBarVisibility();
		void RecreateViewInfo();
		ChangeActions RecalcFinalLayout();
		void RecalcPreliminaryLayout();
		void RecalcAppointmentsLayout();
		void EnsureCalculationsAreFinished();
		void ClearPreliminaryAppointmentsAndCellContainers();
		ChangeActions PrepareChangeActions();
		void RecalcDraggingAppointmentPosition();
		bool ObtainDateTimeScrollBarVisibility();
		void UpdateScrollBarsPosition();
		void UpdateDateTimeScrollBarValue();
		void UpdateResourceScrollBarValue();
		ChangeActions QueryResources();
		ChangeActions QueryAppointments();
		void AssignLimitInterval();
		ChangeActions UpdateVisibleIntervals();
		ChangeActions SynchronizeSelectionInterval(bool activeViewChanged);
		ChangeActions SynchronizeOrResetSelectionInterval(bool activeViewChanged);
		ChangeActions SynchronizeSelectionResource();
		ChangeActions ValidateSelectionResource();
		void RaiseSelectionChanged();
		ChangeActions RaiseVisibleIntervalChanged();
		void RepaintView();
		void UpdateScrollMoreButtonsVisibility();
		void UpdateStateIdentity();
	}
	#endregion
	#region SchedulerControlChangeManager
	public class SchedulerControlChangeManager {
		#region Fields
		ChangeActions actions;
		ISchedulerControlChangeTarget target;
		#endregion
		public SchedulerControlChangeManager(ChangeActions actions) {
			this.actions = actions;
		}
		#region Properties
		internal ChangeActions Actions { get { return actions; } set { actions = value; } }
		#endregion
		internal void SetTarget(ISchedulerControlChangeTarget target) {
			this.target = target;
		}
		public bool IsActionQueued(ChangeActions action) {
			return (actions & action) != 0;
		}
		public virtual void ApplyChanges(ISchedulerControlChangeTarget target) {
			if (target == null)
				Exceptions.ThrowArgumentException("target", target);
			if (actions == ChangeActions.None)
				return;
			this.target = target;
			try {
				ApplyChangesCore();
			} finally {
				this.target = null;
			}
		}
		protected internal virtual void ApplyChangesCore() {
			InnerSchedulerControl innerControl = null;
			IInnerSchedulerCommandTarget schedulerCommandTarget = target as IInnerSchedulerCommandTarget;
			if (schedulerCommandTarget != null)
				innerControl = schedulerCommandTarget.InnerSchedulerControl;
			if (IsActionQueued(ChangeActions.CellContentScroll)) {
				XtraSchedulerDebug.Assert(actions == ChangeActions.CellContentScroll);
				UpdateScrollMoreButtonsVisibility();
				RepaintView();
				return;
			}
			EnsureCalculationsAreFinished();
			if (IsActionQueued(ChangeActions.UpdatePaintStyle))
				UpdatePaintStyle();
			bool updateScrollBarsPosition;
			if (IsActionQueued(ChangeActions.RecalcClientBounds)) {
				RecalcClientBounds();
				updateScrollBarsPosition = true;
			} else
				updateScrollBarsPosition = false;
			if (IsActionQueued(ChangeActions.AssignLimitInterval))
				AssignLimitInterval();
			if (IsActionQueued(ChangeActions.UpdateVisibleIntervals))
				UpdateVisibleIntervals();
			if (IsActionQueued(ChangeActions.SynchronizeSelectionInterval))
				SynchronizeSelectionInterval();
			if (IsActionQueued(ChangeActions.SynchronizeOrResetSelectionInterval))
				SynchronizeOrResetSelectionInterval();
			if (innerControl == null || !innerControl.NestedQuery)
				QueryResources();
			if (IsActionQueued(ChangeActions.SynchronizeSelectionResource))
				SynchronizeSelectionResource();
			if (IsActionQueued(ChangeActions.ValidateSelectionResource))
				ValidateSelectionResource();
			actions |= QueryAppointments();
			if (IsActionQueued(ChangeActions.MayChangeResourceScrollBarOrientation))
				if (ChangeResourceScrollBarOrientationIfNeeded())
					updateScrollBarsPosition = true;
			if (IsActionQueued(ChangeActions.MayChangeDateTimeScrollBarOrientation))
				if (ChangeDateTimeScrollBarOrientationIfNeeded())
					updateScrollBarsPosition = true;
			if (IsActionQueued(ChangeActions.MayChangeResourceScrollBarVisibility))
				if (ChangeResourceScrollBarVisibilityIfNeeded())
					updateScrollBarsPosition = true;
			RecalcViewBounds();
			actions |= PrepareChangeActions();
			RecreateViewInfo();
			if (IsActionQueued(ChangeActions.ClearPreliminaryAppointmentsAndCellContainers)) {
				ClearPreliminaryAppointmentsAndCellContainers();
			}
			if (IsActionQueued(ChangeActions.RecalcPreliminaryLayout)) {
				if (IsDateTimeScrollbarVisibilityDependsOnClientSize()) {
					RecalcPreliminaryLayout();
					actions |= ChangeActions.RecalcViewLayout;
				} else {
					if (IsActionQueued(ChangeActions.MayChangeDateTimeScrollBarVisibility))
						if (ChangeDateTimeScrollBarVisibilityIfNeeded())
							updateScrollBarsPosition = true;
					if (IsActionQueued(ChangeActions.RecalcViewBounds))
						RecalcViewBounds();
					if (IsActionQueued(ChangeActions.RecalcViewLayout) || updateScrollBarsPosition) {
						RecalcClientBounds();
						RecalcPreliminaryLayout();
						actions |= ChangeActions.RecalcViewLayout;
					} else if (IsActionQueued(ChangeActions.RecalcAppointmentsLayout))
						RecalcAppointmentsLayout();
				}
			}
			if (IsActionQueued(ChangeActions.RecalcDraggingAppointmentPosition))
				RecalcDraggingAppointmentPosition();
			if (IsActionQueued(ChangeActions.RecalcViewBounds | ChangeActions.RecalcPreliminaryLayout | ChangeActions.AdjustDateTimeScrollBarPosition)) {
				RecalcViewBounds();
				RecalcScrollBarVisibility();
				bool dateTimeScrollBarVisibilityChanged = ChangeDateTimeScrollBarVisibilityIfNeeded();
				if (dateTimeScrollBarVisibilityChanged) {
					RecalcViewBounds();
					updateScrollBarsPosition = true;
				}
			}
			if (IsActionQueued(ChangeActions.RecalcViewLayout))
				actions |= RecalcFinalLayout();
			if (updateScrollBarsPosition || IsActionQueued(ChangeActions.AdjustDateTimeScrollBarPosition))
				UpdateScrollBarsPosition();
			if (IsActionQueued(ChangeActions.UpdateDateTimeScrollBarValue))
				UpdateDateTimeScrollBarValue();
			if (IsActionQueued(ChangeActions.UpdateResourceScrollBarValue))
				UpdateResourceScrollBarValue();
			if (IsActionQueued(ChangeActions.RaiseVisibleIntervalChanged)) {
				ChangeActions intervalChangedResult = RaiseVisibleIntervalChanged();
				if (intervalChangedResult != ChangeActions.None && innerControl != null) {
					innerControl.NestedQuery = true;
					try {
						ApplyChangesCore();
					} finally {
						innerControl.NestedQuery = false;
					}
				}
			}
			if (IsActionQueued(ChangeActions.RaiseSelectionChanged))
				RaiseSelectionChanged();
			if (actions != ChangeActions.None)
				target.UpdateStateIdentity();
		}
		protected internal virtual void RepaintView() {
			target.RepaintView();
		}
		protected internal virtual void UpdateScrollMoreButtonsVisibility() {
			target.UpdateScrollMoreButtonsVisibility();
		}
		protected internal virtual void UpdatePaintStyle() {
			target.UpdatePaintStyle();
		}
		protected internal virtual bool IsDateTimeScrollbarVisibilityDependsOnClientSize() {
			return target.IsDateTimeScrollbarVisibilityDependsOnClientSize();
		}
		protected internal virtual void RecalcClientBounds() {
			target.RecalcClientBounds();
		}
		protected internal virtual bool ChangeResourceScrollBarOrientationIfNeeded() {
			return target.ChangeResourceScrollBarOrientationIfNeeded();
		}
		protected internal virtual bool ChangeDateTimeScrollBarOrientationIfNeeded() {
			return target.ChangeDateTimeScrollBarOrientationIfNeeded();
		}
		protected internal virtual bool ChangeResourceScrollBarVisibilityIfNeeded() {
			return target.ChangeResourceScrollBarVisibilityIfNeeded();
		}
		protected internal virtual bool ChangeDateTimeScrollBarVisibilityIfNeeded() {
			return target.ChangeDateTimeScrollBarVisibilityIfNeeded();
		}
		protected internal virtual void RecalcViewBounds() {
			target.RecalcViewBounds();
		}
		protected virtual void RecalcScrollBarVisibility() {
			target.RecalcScrollBarVisibility();
		}
		void RecreateViewInfo() {
			target.RecreateViewInfo();
		}
		ChangeActions PrepareChangeActions() {
			return target.PrepareChangeActions();
		}
		protected internal virtual ChangeActions RecalcFinalLayout() {
			return target.RecalcFinalLayout();
		}
		protected internal virtual void RecalcDraggingAppointmentPosition() {
			target.RecalcDraggingAppointmentPosition();
		}
		protected internal virtual void RecalcPreliminaryLayout() {
			target.RecalcPreliminaryLayout();
		}
		private void ClearPreliminaryAppointmentsAndCellContainers() {
			target.ClearPreliminaryAppointmentsAndCellContainers();
		}
		protected internal virtual void RecalcAppointmentsLayout() {
			target.RecalcAppointmentsLayout();
		}
		protected internal virtual bool ObtainDateTimeScrollBarVisibility() {
			return target.ObtainDateTimeScrollBarVisibility();
		}
		protected internal virtual void UpdateScrollBarsPosition() {
			target.UpdateScrollBarsPosition();
		}
		protected internal virtual void UpdateDateTimeScrollBarValue() {
			target.UpdateDateTimeScrollBarValue();
		}
		protected internal virtual void UpdateResourceScrollBarValue() {
			target.UpdateResourceScrollBarValue();
		}
		protected internal virtual void EnsureCalculationsAreFinished() {
			target.EnsureCalculationsAreFinished();
		}
		protected internal virtual void QueryResources() {
			actions |= target.QueryResources();
		}
		protected internal virtual ChangeActions QueryAppointments() {
			return target.QueryAppointments();
		}
		protected internal virtual void AssignLimitInterval() {
			target.AssignLimitInterval();
		}
		protected internal virtual void UpdateVisibleIntervals() {
			actions |= target.UpdateVisibleIntervals();
		}
		protected internal virtual void SynchronizeSelectionInterval() {
			actions |= target.SynchronizeSelectionInterval(IsActionQueued(ChangeActions.NotifyActiveViewChanged));
		}
		protected internal virtual void SynchronizeOrResetSelectionInterval() {
			actions |= target.SynchronizeOrResetSelectionInterval(IsActionQueued(ChangeActions.NotifyActiveViewChanged));
		}
		protected internal virtual void SynchronizeSelectionResource() {
			actions |= target.SynchronizeSelectionResource();
		}
		protected internal virtual void ValidateSelectionResource() {
			actions |= target.ValidateSelectionResource();
		}
		protected internal virtual void RaiseSelectionChanged() {
			target.RaiseSelectionChanged();
		}
		protected internal virtual ChangeActions RaiseVisibleIntervalChanged() {
			return target.RaiseVisibleIntervalChanged();
		}
	}
	#endregion
}
