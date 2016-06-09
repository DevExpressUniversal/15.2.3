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

using DevExpress.Utils.Commands;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Commands;
namespace DevExpress.Xpf.Scheduler.Commands {
	[CommandLocalization(SchedulerControlStringId.Caption_IntervalCount, SchedulerControlStringId.Description_IntervalCount)]
	public class SetTimeIntervalCountCommand : XpfSchedulerCommand {
		public SetTimeIntervalCountCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.SetTimeIntervalCount; } }
		public override SchedulerMenuItemId MenuId { get { return SchedulerMenuItemId.Custom; } }
		public override ICommandUIState CreateDefaultCommandUIState() {
			return new DefaultValueBasedCommandUIState<int>();
		}
		public override void ForceExecute(ICommandUIState state) {
			TimelineView view = Control.ActiveView.Owner as TimelineView;
			if (view == null)
				return;
			DefaultValueBasedCommandUIState<int> valueState = state as DefaultValueBasedCommandUIState<int>;
			if (valueState == null)
				return;
			view.IntervalCount = valueState.Value;
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			state.Visible = true;
			TimelineView view = Control.ActiveView.Owner as TimelineView;
			if (view == null) {
				state.Enabled = false;
				view = Control.TimelineView.Owner as TimelineView;
				state.EditValue = view.IntervalCount;
				return;
			}
			state.Enabled = true;
			state.EditValue = view.IntervalCount;
		}
	}
}
