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
using System.Collections.Generic;
using System.Text;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.Localization;
namespace DevExpress.XtraScheduler.Commands {
	#region SwitchTimeScalesUICommand
	public class SwitchTimeScalesUICommand : SchedulerMenuItemSimpleCommand {
		public SwitchTimeScalesUICommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		public SwitchTimeScalesUICommand(InnerSchedulerControl target)
			: base(target) {
		}
		public override SchedulerMenuItemId MenuId { get { return SchedulerMenuItemId.SwitchTimeScalesTo; } }
		public override Localization.SchedulerStringId DescriptionStringId { get { return Localization.SchedulerStringId.DescCmd_TimeScalesMenu; } }
		public override Localization.SchedulerStringId MenuCaptionStringId { get { return Localization.SchedulerStringId.MenuCmd_TimeScalesMenu; } }
		public override string ImageName { get { return SchedulerCommandImagesNames.SwitchTimeScalesTo; } }
		protected override void UpdateUIStateCore(Utils.Commands.ICommandUIState state) {
			state.Visible = true;
			SchedulerViewType activeViewType = Control.ActiveViewType;
			state.Enabled = activeViewType == SchedulerViewType.Day || activeViewType == SchedulerViewType.WorkWeek || activeViewType == SchedulerViewType.Timeline || activeViewType == SchedulerViewType.Gantt || activeViewType == SchedulerViewType.FullWeek;
		}
		protected internal override void ExecuteCore() {
		}
	}
	#endregion
	#region SwitchTimeScalesCaptionUICommand
	public class SwitchTimeScalesCaptionUICommand : SchedulerMenuItemSimpleCommand {
		public SwitchTimeScalesCaptionUICommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		public SwitchTimeScalesCaptionUICommand(InnerSchedulerControl target)
			: base(target) {
		}
		public override SchedulerMenuItemId MenuId { get { return SchedulerMenuItemId.SwitchTimeScalesTo; } }
		public override Localization.SchedulerStringId DescriptionStringId { get { return SchedulerStringId.MenuCmd_TimeScaleCaptionsMenu; } }
		public override Localization.SchedulerStringId MenuCaptionStringId { get { return SchedulerStringId.MenuCmd_TimeScaleCaptionsMenu; } }
		protected override void UpdateUIStateCore(Utils.Commands.ICommandUIState state) {
			state.Visible = true;
			SchedulerViewType activeViewType = Control.ActiveViewType;
			state.Enabled = activeViewType == SchedulerViewType.Timeline || activeViewType == SchedulerViewType.Gantt;
		}
		protected internal override void ExecuteCore() {
		}
	}
	#endregion
}
