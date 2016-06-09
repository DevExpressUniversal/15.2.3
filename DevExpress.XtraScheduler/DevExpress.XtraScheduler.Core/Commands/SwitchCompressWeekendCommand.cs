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
	public class SwitchCompressWeekendCommand : SchedulerMenuItemSimpleCommand {
		public SwitchCompressWeekendCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		public SwitchCompressWeekendCommand(InnerSchedulerControl scheduler)
			: base(scheduler) {
		}
		public override SchedulerMenuItemId MenuId { get { return SchedulerMenuItemId.SwitchCompressWeekend; } }
		public override Localization.SchedulerStringId DescriptionStringId { get { return Localization.SchedulerStringId.DescCmd_CompressWeekend; } }
		public override Localization.SchedulerStringId MenuCaptionStringId { get { return Localization.SchedulerStringId.MenuCmd_CompressWeekend; } }
		public override string ImageName { get { return SchedulerCommandImagesNames.CompressWeekend; } }
		protected internal override void ExecuteCore() {
			InnerMonthView monthView = (InnerMonthView)InnerControl.ActiveView;
			monthView.CompressWeekend = !monthView.CompressWeekend;
		}
		protected override void UpdateUIStateCore(Utils.Commands.ICommandUIState state) {
			InnerMonthView monthView = InnerControl.ActiveView as InnerMonthView;
			state.Enabled = monthView != null;
			state.Visible = InnerControl.MonthView.Enabled;
			bool isChecked = false;
			if (state.Enabled)
				isChecked = monthView.CompressWeekend;
			state.Checked = isChecked;
		}
	}
}
