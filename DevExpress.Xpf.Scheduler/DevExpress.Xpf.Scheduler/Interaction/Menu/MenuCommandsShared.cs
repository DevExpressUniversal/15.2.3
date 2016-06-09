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
using System.Reflection;
using DevExpress.Utils.Commands;
using DevExpress.Utils.Controls;
using DevExpress.XtraScheduler.Localization;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.Operations;
using DevExpress.XtraScheduler.Services;
using DevExpress.Utils;
using DevExpress.Utils.Menu;
using DevExpress.XtraScheduler.Drawing;
#if WPF || SL
using DevExpress.Xpf.Scheduler.Native;
using DevExpress.Xpf.Scheduler;
#endif
namespace DevExpress.XtraScheduler.Commands {
	#region SchedulerMenuItemWinSimpleCommand (abstract class)
	public abstract class SchedulerMenuItemWinSimpleCommand : SchedulerMenuItemSimpleCommand {
		readonly SchedulerControl control;
		protected SchedulerMenuItemWinSimpleCommand(ISchedulerCommandTarget target)
			: base(target) {
			this.control = InnerControl.Owner as SchedulerControl;
			if (this.control == null)
				Exceptions.ThrowArgumentException("target", target);
		}
		protected internal SchedulerControl SchedulerControl { get { return control; } }
		protected override Assembly ImageResourceAssembly { get { return Assembly.GetExecutingAssembly(); } }
	}
	#endregion
	#region CustomizeTimeRulerCommand
	public class CustomizeTimeRulerCommand : SchedulerMenuItemWinSimpleCommand {
		TimeRuler ruler;
		public CustomizeTimeRulerCommand(ISchedulerCommandTarget control, TimeRuler ruler)
			: base(control) {
			if (ruler == null)
				Exceptions.ThrowArgumentException("ruler", ruler);
			this.ruler = ruler;
		}
		#region Properties
		public override SchedulerMenuItemId MenuId { get { return SchedulerMenuItemId.CustomizeTimeRuler; } }
		public override SchedulerStringId MenuCaptionStringId { get { return SchedulerStringId.MenuCmd_CustomizeTimeRuler; } }
		public override SchedulerStringId DescriptionStringId { get { return SchedulerStringId.Caption_EmptyResource; } }
		protected internal TimeRuler Ruler { get { return ruler; } }
		#endregion
		protected internal override void ExecuteCore() {
			SchedulerControl.ShowCustomizeTimeRulerForm(ruler);
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
		}
	}
	#endregion
}
