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
	public class SwitchShowWorkTimeOnlyCommand : SchedulerMenuItemSimpleCommand {
		public SwitchShowWorkTimeOnlyCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		public SwitchShowWorkTimeOnlyCommand(InnerSchedulerControl control)
			: base(control) {
		}
		public override SchedulerMenuItemId MenuId { get { return SchedulerMenuItemId.SwitchShowWorkTimeOnly; } }
		public override Localization.SchedulerStringId DescriptionStringId { get { return Localization.SchedulerStringId.DescCmd_ShowWorkTimeOnly; } }
		public override Localization.SchedulerStringId MenuCaptionStringId { get { return Localization.SchedulerStringId.MenuCmd_ShowWorkTimeOnly; } }
		public override string ImageName { get { return SchedulerCommandImagesNames.ShowWorkTimeOnly; } }
		protected internal override void ExecuteCore() {
			InnerDayView view = Control.ActiveView as InnerDayView;
			if (view == null)
				return;
			view.ShowWorkTimeOnly = !view.ShowWorkTimeOnly;
		}
		protected override void UpdateUIStateCore(Utils.Commands.ICommandUIState state) {
			InnerDayView view = Control.ActiveView as InnerDayView;
			state.Visible = Control.DayView.Enabled || Control.WorkWeekView.Enabled || Control.FullWeekView.Enabled;
			state.Enabled = view != null;
			bool isChecked = false;
			if (view != null)
				isChecked = view.ShowWorkTimeOnly;
			state.Checked = isChecked;
		}
	}
}
