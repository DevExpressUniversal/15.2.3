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
using DevExpress.XtraScheduler.Services.Internal;
using DevExpress.Utils;
namespace DevExpress.XtraScheduler {
	#region ISchedulerInplaceEditorEx
	public interface ISchedulerInplaceEditorEx : IDisposable {
		void Activate();
		void Deactivate();
		void ApplyChanges();
		event EventHandler CommitChanges;
		event EventHandler RollbackChanges;
	}
	#endregion
}
namespace DevExpress.XtraScheduler.Native {
	public interface ISchedulerInplaceEditController : IDisposable {
		Appointment EditedAppointment { get; }
		event EventHandler CommitChanges;
		event EventHandler RollbackChanges;
		void Activate();
		void SetEditedAppointment(Appointment appointment);
		void ResetEditedAppointment();
	}
	#region SchedulerInplaceEditController (abstract class)
	public abstract class SchedulerInplaceEditController : ISchedulerInplaceEditController {
		#region Fields
		readonly InnerSchedulerControl control;
		ISchedulerInplaceEditorEx editor;
		Appointment editedAppointment;
		bool isDisposed;
		#endregion
		protected SchedulerInplaceEditController(InnerSchedulerControl control) {
			Guard.ArgumentNotNull(control, "control");
			this.control = control;
		}
		#region Properties
		internal ISchedulerInplaceEditorEx Editor { get { return editor; } }
		public Appointment EditedAppointment { get { return editedAppointment; } }
		internal bool IsDisposed { get { return isDisposed; } }
		protected internal InnerSchedulerControl InnerControl { get { return control; } }
		#endregion
		#region Events
		#region CommitChanges
		public event EventHandler CommitChanges;
		protected internal virtual void RaiseCommitChanges() {
			if (CommitChanges != null)
				CommitChanges(this, EventArgs.Empty);
		}
		#endregion
		#region RollbackChanges
		public event EventHandler RollbackChanges;
		protected internal virtual void RaiseRollbackChanges() {
			if (RollbackChanges != null)
				RollbackChanges(this, EventArgs.Empty);
		}
		#endregion
		#endregion
		#region ISchedulerInplaceEditController Members
		public virtual void SetEditedAppointment(Appointment appointment) {
			Guard.ArgumentNotNull(appointment, "appointment");
			this.editedAppointment = appointment;
		}
		public void ResetEditedAppointment() {
			this.editedAppointment = null;
		}
		public virtual void Activate() {
			if (Editor != null)
				return;
			if (EditedAppointment == null) {
				RaiseRollbackChanges();
				return;
			}
			this.editor = CreateAppointmentInplaceEditor(EditedAppointment);
			if (editor == null) {
				RaiseRollbackChanges();
				return;
			}
			SubscribeEditorEvents();
			BeforeEditorActivate();
			Editor.Activate();
			ISetSchedulerStateService setStateService = InnerControl.GetService<ISetSchedulerStateService>();
			if (setStateService != null)
				setStateService.IsInplaceEditorOpened = true;
		}
		#endregion
		#region IDisposable Members
		protected internal virtual void Dispose(bool disposing) {
			if (disposing) {
				this.isDisposed = true;
				Deactivate();
			}
		}
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		#endregion
		protected internal virtual void Deactivate() {
			if (Editor == null)
				return;
			UnsubscribeEditorEvents();
			Editor.Deactivate();
			AfterEditorDeactivate();
			Editor.Dispose();
			this.editor = null;
			ISetSchedulerStateService setStateService = InnerControl.GetService<ISetSchedulerStateService>();
			if (setStateService != null)
				setStateService.IsInplaceEditorOpened = false;
		}
		protected internal virtual void OnCommitChanges() {
			Editor.ApplyChanges();
			Deactivate();
			RaiseCommitChanges();
		}
		protected virtual void OnRollbackChanges() {
			Deactivate();
			RaiseRollbackChanges();
		}
		void SubscribeEditorEvents() {
			Editor.RollbackChanges += Editor_RollbackChanges;
			Editor.CommitChanges += Editor_CommitChanges;
		}
		void UnsubscribeEditorEvents() {
			Editor.RollbackChanges -= Editor_RollbackChanges;
			Editor.CommitChanges -= Editor_CommitChanges;
		}
		void Editor_CommitChanges(object sender, EventArgs e) {
			OnCommitChanges();
		}
		void Editor_RollbackChanges(object sender, EventArgs e) {
			OnRollbackChanges();
		}
		protected internal abstract void BeforeEditorActivate();
		protected internal abstract void AfterEditorDeactivate();
		protected internal abstract ISchedulerInplaceEditorEx CreateAppointmentInplaceEditor(Appointment apt);
	}
	#endregion
}
