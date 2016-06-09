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
using DevExpress.XtraScheduler.Commands;
using DevExpress.XtraScheduler.Drawing;
using DevExpress.XtraScheduler.Native;
using DevExpress.Utils;
using DevExpress.Utils.Menu;
namespace DevExpress.XtraScheduler.Native {
	#region WinFormsSchedulerMenuBuilderUIFactory
	public class WinFormsSchedulerMenuBuilderUIFactory : IMenuBuilderUIFactory<SchedulerCommand, SchedulerMenuItemId> {
		public IDXMenuItemCommandAdapter<SchedulerMenuItemId> CreateMenuItemAdapter(SchedulerCommand command) {
			return new SchedulerMenuItemCommandWinAdapter(command);
		}
		public IDXMenuCheckItemCommandAdapter<SchedulerMenuItemId> CreateMenuCheckItemAdapter(SchedulerCommand command) {
			return new SchedulerMenuCheckItemCommandWinAdapter(command);
		}
		public IDXPopupMenu<SchedulerMenuItemId> CreatePopupMenu() {
			return new SchedulerPopupMenu();
		}
		public IDXPopupMenu<SchedulerMenuItemId> CreateSubMenu() {
			return new SchedulerPopupMenu();
		}
	}
	#endregion
	public abstract class SchedulerPopupMenuWinBuilder : SchedulerPopupMenuBuilder {
		readonly SchedulerHitInfo hitInfo;
		protected SchedulerPopupMenuWinBuilder(IMenuBuilderUIFactory<SchedulerCommand, SchedulerMenuItemId> uiFactory, InnerSchedulerControl control, SchedulerHitInfo hitInfo)
			: base(control, uiFactory) {
			Guard.ArgumentNotNull(hitInfo, "hitInfo");
			this.hitInfo = hitInfo;
		}
		protected internal SchedulerHitInfo HitInfo { get { return hitInfo; } }
	}
	public class SchedulerEmptyPopupMenuBuilder : SchedulerPopupMenuBuilder {
		public SchedulerEmptyPopupMenuBuilder(InnerSchedulerControl control, IMenuBuilderUIFactory<SchedulerCommand, SchedulerMenuItemId> uiFactory)
			: base(control, uiFactory) {
		}
		public override void PopulatePopupMenu(IDXPopupMenu<SchedulerMenuItemId> menu) {
		}
	}
	public class SchedulerDefaultPopupMenuWinBuilder : SchedulerDefaultPopupMenuBuilder {
		readonly SchedulerControl control;
		public SchedulerDefaultPopupMenuWinBuilder(IMenuBuilderUIFactory<SchedulerCommand, SchedulerMenuItemId> uiFactory, SchedulerControl control, ISchedulerHitInfo hitInfo)
			: base(control.InnerControl, uiFactory, hitInfo) {
			Guard.ArgumentNotNull(control, "control");
			this.control = control;
		}
		protected internal SchedulerControl Control {
			get { return control; }
		}
		protected internal override SchedulerCommand CreateCustomizeTimeRulerCommand() {
			TimeRulerViewInfo rulerViewInfo = (TimeRulerViewInfo)HitInfo.ViewInfo;
			return new CustomizeTimeRulerCommand(Control, rulerViewInfo.Ruler);
		}
		protected internal override SchedulerCommand CreateAppointmentDependencyCreatingOperationCommand() {
			return new AppointmentCreateDependencyOperationCommand(Control);
		}
		protected internal override SchedulerCommand CreateChangeAppointmentLabelCommand(IAppointmentLabel label, int labelIndex) {
			return new ChangeAppointmentLabelCommand(control, (AppointmentLabel)label, labelIndex);
		}
		protected internal override SchedulerCommand CreateChangeAppointmentStatusCommand(IAppointmentStatus status, int statusIndex) {
			return new ChangeAppointmentStatusCommand(control, (AppointmentStatus)status, statusIndex);
		}
	}
}
