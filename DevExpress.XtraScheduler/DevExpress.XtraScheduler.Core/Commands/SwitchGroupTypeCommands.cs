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
using DevExpress.Utils.Commands;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.Localization;
namespace DevExpress.XtraScheduler.Commands {
	public abstract class SwitchGroupTypeCommand : SchedulerMenuItemSimpleCommand {
		protected SwitchGroupTypeCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		#region Properties
		public abstract SchedulerGroupType GroupType { get; }
		#endregion
		protected internal override void ExecuteCore() {
			InnerControl.GroupType = GroupType;
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			state.Checked = InnerControl.GroupType == GroupType;
			state.Enabled = true;
			state.Visible = true;
		}
	}
	public class SwitchToGroupByNoneCommand : SwitchGroupTypeCommand {
		public SwitchToGroupByNoneCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		#region Properties
		public override SchedulerGroupType GroupType { get { return SchedulerGroupType.None; } }
		public override SchedulerMenuItemId MenuId { get { return SchedulerMenuItemId.SwitchToGroupByNone; } }
		public override SchedulerStringId MenuCaptionStringId { get { return SchedulerStringId.MenuCmd_SwitchToGroupByNone; } }
		public override SchedulerStringId DescriptionStringId { get { return SchedulerStringId.Caption_GroupByNoneDescription; } }
		public override string ImageName { get { return SchedulerCommandImagesNames.GroupByNone; } }
		#endregion
	}
	public class SwitchToGroupByDateCommand : SwitchGroupTypeCommand {
		public SwitchToGroupByDateCommand(ISchedulerCommandTarget target) : base(target) {
		}
		#region Properties
		public override SchedulerGroupType GroupType { get { return SchedulerGroupType.Date; } }
		public override SchedulerMenuItemId MenuId { get { return SchedulerMenuItemId.SwitchToGroupByDate; } }
		public override SchedulerStringId MenuCaptionStringId { get { return SchedulerStringId.MenuCmd_SwitchToGroupByDate; } }
		public override SchedulerStringId DescriptionStringId { get { return SchedulerStringId.Caption_GroupByDateDescription; ; } }
		public override string ImageName { get { return SchedulerCommandImagesNames.GroupByDate; } }
		#endregion
	}
	public class SwitchTOGroupByResourceCommand : SwitchGroupTypeCommand {
		public SwitchTOGroupByResourceCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		#region Properties
		public override SchedulerGroupType GroupType { get { return SchedulerGroupType.Resource; } }
		public override SchedulerMenuItemId MenuId { get { return SchedulerMenuItemId.SwitchToGroupByResource; } }
		public override SchedulerStringId MenuCaptionStringId { get { return SchedulerStringId.MenuCmd_SwitchToGroupByResource; } }
		public override SchedulerStringId DescriptionStringId { get { return SchedulerStringId.Caption_GroupByResourceDescription; } }
		public override string ImageName { get { return SchedulerCommandImagesNames.GroupByResource; } }
		#endregion
	}
}
