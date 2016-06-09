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

using System.Windows.Input;
using DevExpress.Office.Internal;
using DevExpress.Utils.Commands;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Bars.Native;
using DevExpress.Xpf.Ribbon;
using DevExpress.Xpf.Scheduler.Commands;
using DevExpress.XtraScheduler.Commands;
namespace DevExpress.Xpf.Scheduler.Internal {
	public class SchedulerControlBarCommandManager : ControlBarCommandManager<SchedulerControl, SchedulerCommand, SchedulerCommandId> {
		public SchedulerControlBarCommandManager(SchedulerControl scheduler) : base(scheduler) {
		}
		protected override object ControlAccessor { get { return Control.Accessor; } }
		protected override BarManager BarManager { get { return Control.BarManager; } }
		protected override RibbonControl Ribbon { get { return Control.Ribbon; } }
		protected override SchedulerCommandId EmptyCommandId { get { return SchedulerCommandId.None; } }
		protected override SchedulerCommandId GetCommandId(ICommand command) {
			SchedulerUICommand commandUi = command as SchedulerUICommand;
			if (commandUi == null)
				return SchedulerCommandId.None;
			return commandUi.CommandId;
		}
		protected override void SetFocus() {
			Control.SetFocus();
		}
		protected override bool IsControlProvider(object value) {
			return value is SchedulerControlProvider;
		}
		protected override BarItemCommandUIState CreateBarItemUIState(BarItem item) {
			return new BarItemCommandUIState(item);
		}
		protected internal override Command CreateCommand(ICommand barItemCommand) {
			SchedulerUICommand uiCommand = barItemCommand as SchedulerUICommand;
			if (uiCommand == null)
				return base.CreateCommand(barItemCommand); ;
			return uiCommand.CreateCommand(Control);
		}
		protected override void SetItemGlyphs(BarItem item, Command command) {
			ChangeAppointmentPropertyImageCommand imageCommand = command as ChangeAppointmentPropertyImageCommand;
			if (imageCommand != null) {
				BarItemDefaultProperties.SetGlyph(item, imageCommand.Image);
				BarItemDefaultProperties.SetLargeGlyph(item, imageCommand.LargeImage);
				return;
			}
			base.SetItemGlyphs(item, command);
		}
	}
}
