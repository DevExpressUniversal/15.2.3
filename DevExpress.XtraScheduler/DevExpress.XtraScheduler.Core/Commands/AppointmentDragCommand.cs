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
using DevExpress.Utils.Commands;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.Localization;
#if !SILVERLIGHT
using PlatformIndependentDragDropEffects = System.Windows.Forms.DragDropEffects;
using System.Drawing;
using DevExpress.XtraScheduler.Internal.Diagnostics;
#else
using PlatformIndependentDragDropEffects = DevExpress.Utils.DragDropEffects;
#endif
namespace DevExpress.XtraScheduler.Commands {
	public abstract class AppointmentDragCommand : SchedulerMenuItemSimpleCommand {
		readonly SchedulerDragData dragData;
		readonly AppointmentBaseCollection changedAppointments;
		protected AppointmentDragCommand(ISchedulerCommandTarget target, SchedulerDragData dragData, AppointmentBaseCollection changedAppointments)
			: base(target) {
			Guard.ArgumentNotNull(dragData, "dragData");
			Guard.ArgumentNotNull(changedAppointments, "changedAppointments");
			if (dragData.Appointments.Count != changedAppointments.Count)
				Exceptions.ThrowArgumentException("changedAppointments.Count", changedAppointments.Count);
			this.dragData = dragData;
			this.changedAppointments = new AppointmentBaseCollection();
			this.changedAppointments.AddRange(changedAppointments);
		}
		protected SchedulerDragData DragData { get { return dragData; } }
		protected AppointmentBaseCollection ChangedAppointments { get { return changedAppointments; } }
		protected internal abstract bool Copy { get; }
		protected override void UpdateUIStateCore(ICommandUIState state) {
		}
		public override void ForceExecute(ICommandUIState state) {
			AppointmentChangeHelper changeHelper = InnerControl.AppointmentChangeHelper;
			changeHelper.BeginInternalDragCommand(null, null, new SafeAppointmentCollection(dragData.Appointments, Control.Storage), new SafeAppointment(dragData.PrimaryAppointment, Control.Storage), TimeSpan.Zero);
			PlatformIndependentDragDropEffects effect = Copy ? PlatformIndependentDragDropEffects.Copy : PlatformIndependentDragDropEffects.Move;
			changeHelper.DragOnExternalControl(dragData, changedAppointments, Copy, true);
			changeHelper.CommitDrag(effect, true);
		}
		protected internal override void ExecuteCore() {
		}
	}
	#region AppointmentDragMoveCommand
	public class AppointmentDragMoveCommand : AppointmentDragCommand {
		public AppointmentDragMoveCommand(ISchedulerCommandTarget target, SchedulerDragData dragData, AppointmentBaseCollection changedAppointments)
			: base(target, dragData, changedAppointments) {
		}
		#region Properties
		public override SchedulerMenuItemId MenuId { get { return SchedulerMenuItemId.AppointmentDragMove; } }
		public override SchedulerStringId MenuCaptionStringId { get { return SchedulerStringId.MenuCmd_AppointmentMove; } }
		public override SchedulerStringId DescriptionStringId { get { return SchedulerStringId.Caption_EmptyResource; } }
		protected internal override bool Copy { get { return false; } }
		#endregion
	}
	#endregion
	#region AppointmentDragCopyCommand
	public class AppointmentDragCopyCommand : AppointmentDragCommand {
		public AppointmentDragCopyCommand(ISchedulerCommandTarget target, SchedulerDragData dragData, AppointmentBaseCollection changedAppointments)
			: base(target, dragData, changedAppointments) {
		}
		#region Properties
		public override SchedulerMenuItemId MenuId { get { return SchedulerMenuItemId.AppointmentDragCopy; } }
		public override SchedulerStringId MenuCaptionStringId { get { return SchedulerStringId.MenuCmd_AppointmentCopy; } }
		public override SchedulerStringId DescriptionStringId { get { return SchedulerStringId.Caption_EmptyResource; } }
		protected internal override bool Copy { get { return true; } }
		#endregion
	}
	#endregion
	#region AppointmentDragCancelCommand
	public class AppointmentDragCancelCommand : AppointmentDragCommand {
		public AppointmentDragCancelCommand(ISchedulerCommandTarget target, SchedulerDragData dragData, AppointmentBaseCollection changedAppointments)
			: base(target, dragData, changedAppointments) {
		}
		#region Properties
		public override SchedulerMenuItemId MenuId { get { return SchedulerMenuItemId.AppointmentDragCancel; } }
		public override SchedulerStringId MenuCaptionStringId { get { return SchedulerStringId.MenuCmd_AppointmentCancel; } }
		public override SchedulerStringId DescriptionStringId { get { return SchedulerStringId.Caption_EmptyResource; } }
		protected internal override bool Copy { get { XtraSchedulerDebug.Assert(false); return false; } }
		#endregion
		protected override void UpdateUIStateCore(ICommandUIState state) {
		}
		public override void ForceExecute(ICommandUIState state) {
		}
	}
	#endregion
}
