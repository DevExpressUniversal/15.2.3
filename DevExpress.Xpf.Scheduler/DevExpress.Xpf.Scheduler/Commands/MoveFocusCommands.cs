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
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Scheduler;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Commands;
using DevExpress.XtraScheduler.Localization;
using DevExpress.XtraScheduler.Native;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
namespace DevExpress.Xpf.Scheduler.Commands {
	#region MoveFocusNextPrevCommandBase
	public abstract class MoveFocusNextPrevCommandBase : SchedulerMenuItemWinSimpleCommand {
		protected MoveFocusNextPrevCommandBase(InnerSchedulerControl control)
			: base(control) {
		}
		protected MoveFocusNextPrevCommandBase(SchedulerControl control)
			: base(control) {
		}
		#region Properties
		public override SchedulerMenuItemId MenuId {
			get { return SchedulerMenuItemId.Custom; }
		}
		public override SchedulerStringId DescriptionStringId {
			get { return SchedulerStringId.MenuCmd_None; }
		}
		public override SchedulerStringId MenuCaptionStringId {
			get { return SchedulerStringId.MenuCmd_None; }
		}
		public abstract FocusNavigationDirection NavigationDirection { get; }
		#endregion
		protected internal override void ExecuteCore() {
			var request = new TraversalRequest(NavigationDirection);
			Keyboard.ClearFocus();
			System.Windows.DependencyObject focusScope = FocusManager.GetFocusScope(SchedulerControl);
			FrameworkElement focusedElement = FocusManager.GetFocusedElement(focusScope) as FrameworkElement;
			while (focusedElement != null && LayoutHelper.FindParentObject<SchedulerControl>(focusedElement) != null) {
				focusedElement.MoveFocus(request);
				focusedElement = FocusManager.GetFocusedElement(focusScope) as FrameworkElement;
			}
			if (focusedElement == SchedulerControl)
				focusedElement.MoveFocus(request);
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			state.Enabled = true;
			state.Visible = true;
			state.EditValue = null;
		}
	}
	#endregion
	#region MoveFocusNextCommand
	public class MoveFocusNextCommand : MoveFocusNextPrevCommandBase {
		public MoveFocusNextCommand(InnerSchedulerControl control)
			: base(control) {
		}
		public MoveFocusNextCommand(SchedulerControl control)
			: base(control) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.MoveFocusNext; } }
		public override FocusNavigationDirection NavigationDirection { get { return FocusNavigationDirection.Next; } }
	}
	#endregion
	#region MoveFocusPrevCommand
	public class MoveFocusPrevCommand : MoveFocusNextPrevCommandBase {
		public MoveFocusPrevCommand(InnerSchedulerControl control)
			: base(control) {
		}
		public MoveFocusPrevCommand(SchedulerControl control)
			: base(control) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.MoveFocusPrev; } }
		public override FocusNavigationDirection NavigationDirection { get { return FocusNavigationDirection.Previous; } }
	}
	#endregion
}
