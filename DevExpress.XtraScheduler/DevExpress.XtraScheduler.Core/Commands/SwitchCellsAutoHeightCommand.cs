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
namespace DevExpress.XtraScheduler.Commands {
	public class SwitchCellsAutoHeightCommand : SchedulerMenuItemSimpleCommand {
		public SwitchCellsAutoHeightCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		public SwitchCellsAutoHeightCommand(InnerSchedulerControl scheduler)
			: base(scheduler) {
		}
		public override SchedulerMenuItemId MenuId { get { return SchedulerMenuItemId.SwitchCellsAutoHeight; } }
		public override Localization.SchedulerStringId DescriptionStringId { get { return Localization.SchedulerStringId.DescCmd_CellsAutoHeight; } }
		public override Localization.SchedulerStringId MenuCaptionStringId { get { return Localization.SchedulerStringId.MenuCmd_CellsAutoHeight; } }
		public override string ImageName { get { return SchedulerCommandImagesNames.CellsAutoHeight; } }
		protected internal override void ExecuteCore() {
			InnerTimelineView view = (InnerTimelineView)InnerControl.ActiveView;
			view.CellsAutoHeightOptions.Enabled = !view.CellsAutoHeightOptions.Enabled;
		}
		protected override void UpdateUIStateCore(Utils.Commands.ICommandUIState state) {
			InnerTimelineView timelineView = InnerControl.ActiveView as InnerTimelineView;
			state.Enabled = timelineView != null;
			state.Visible = InnerControl.TimelineView.Enabled;
			bool isChecked = false;
			if (state.Enabled)
				isChecked = timelineView.CellsAutoHeightOptions.Enabled;
			state.Checked = isChecked;
		}
	}
}
