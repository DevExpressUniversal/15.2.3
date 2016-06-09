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

using DevExpress.Xpf.Core.Native;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Commands;
using DevExpress.XtraScheduler.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
namespace DevExpress.Xpf.Scheduler.Commands {
	public abstract class ScrollByPhysicalOffsetCommandBase : XpfSchedulerCommand {
		const int DefaultScrollStep = 40;
		protected ScrollByPhysicalOffsetCommandBase(ISchedulerCommandTarget target)
			: base(target) {
				Step = new Size(FrameworkRenderElement.DpiScaleX * DefaultScrollStep, FrameworkRenderElement.DpiScaleX * DefaultScrollStep);
		}
		public Size Step { get; private set; }
		public int PhysicalOffset { get; set; }
		public int ScrolledOffset { get; protected set; }
		public override SchedulerMenuItemId MenuId { get { return SchedulerMenuItemId.Custom; } }
		public override void ForceExecute(DevExpress.Utils.Commands.ICommandUIState state) {
			ISchedulerOfficeScroller scroller = ObtainOfficeScroller();
			if (scroller == null)
				return;
			ScrolledOffset = ForceExecuteCore(scroller);
		}
		ISchedulerOfficeScroller ObtainOfficeScroller() {
			return Control.MouseHandler.OfficeScroller as ISchedulerOfficeScroller;
		}
		protected override void UpdateUIStateCore(DevExpress.Utils.Commands.ICommandUIState state) {
			state.Enabled = true;
			state.Visible = true;
		}
		protected abstract int ForceExecuteCore(ISchedulerOfficeScroller scroller);
	}
	public class ScrollVerticallyByPhysicalOffsetCommand : ScrollByPhysicalOffsetCommandBase {
		public ScrollVerticallyByPhysicalOffsetCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		protected override int ForceExecuteCore(ISchedulerOfficeScroller scroller) {
			InnerDayView dayView = Control.ActiveView as InnerDayView;
			if (dayView != null) {
				scroller.ScrollVerticalByPixel(-PhysicalOffset);
				return PhysicalOffset;
			}
			int verticalDelta = (int)(PhysicalOffset / Step.Height);
			if (verticalDelta != 0) 
				scroller.ScrollVertical(-verticalDelta);
			return (int)(verticalDelta * Step.Height);
		}
	}
	public class ScrollHorizontallyByPhysicalOffsetCommand : ScrollByPhysicalOffsetCommandBase {
		public ScrollHorizontallyByPhysicalOffsetCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		protected override int ForceExecuteCore(ISchedulerOfficeScroller scroller) {
			int horizontalDelta = (int)(PhysicalOffset / Step.Height);
			if (horizontalDelta != 0)
				scroller.ScrollHorizontal(-horizontalDelta);
			return (int)(horizontalDelta * Step.Height);
		}
	}
}
