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
using DevExpress.Utils.Commands;
using DevExpress.XtraScheduler.Localization;
using DevExpress.XtraScheduler.Native;
#if !SL
#else
using System.Windows.Controls;
#endif
namespace DevExpress.XtraScheduler.Commands {
	public class ChangeTimelineScaleWidthUICommand : SchedulerCommand {
		public const int MinWidth = 10;
		public const int MaxWidth = 200;
		public ChangeTimelineScaleWidthUICommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		public ChangeTimelineScaleWidthUICommand(InnerSchedulerControl control)
			: base(control) {
		}
		public override SchedulerMenuItemId MenuId { get { return SchedulerMenuItemId.ChangeTimelineScaleWidthUI; } }
		public override SchedulerStringId DescriptionStringId { get { return SchedulerStringId.DescCmd_ChangeTimelineScaleWidth; } }
		public override SchedulerStringId MenuCaptionStringId { get { return SchedulerStringId.MenuCmd_ChangeTimelineScaleWidth; } }
		protected override void UpdateUIStateCore(ICommandUIState state) {
			InnerTimelineView timelineView = Control.TimelineView;
			state.Visible = timelineView.Enabled;
			state.Enabled = state.Visible && Control.ActiveViewType == SchedulerViewType.Timeline;
			state.EditValue = GetActualWidth(timelineView.GetBaseTimeScale().Width);
		}
		public override void ForceExecute(ICommandUIState state) {
			if (!CanExecute())
				return;
			int actualWidth = GetActualWidth(state.EditValue);
			Control.TimelineView.GetBaseTimeScale().Width = actualWidth;
		}
		protected virtual int GetActualWidth(object width) {
			int actualWidth = Convert.ToInt32(width);
			if (actualWidth < MinWidth)
				actualWidth = MinWidth;
			else if (actualWidth > MaxWidth)
				actualWidth = MaxWidth;
			return actualWidth;
		}
		public override bool CanExecute() {
			return Control.ActiveViewType == SchedulerViewType.Timeline;
		}
		public override ICommandUIState CreateDefaultCommandUIState() {
			return new DefaultValueBasedCommandUIState<int>();
		}
	}
}
