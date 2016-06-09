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

using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Bars.Native;
using DevExpress.Xpf.Scheduler.Commands;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Commands;
using DevExpress.XtraScheduler.Native;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
namespace DevExpress.Xpf.Scheduler.UI {
	public class SwitchTimeScalesCaptionBarSubItem : SubMenuBarSubItem {
		protected override void UpdateItemsCore() {
			TimelineView view = SchedulerControl.ActiveView as TimelineView;
			if (view == null)
				return;
			InnerTimelineView innerView = (InnerTimelineView)view.InnerView;
			TimeScaleCollection slots = innerView.Scales;
			if (slots == null)
				return;
			foreach (TimeScale timeScale in slots) {
				SetTimeScaleVisibleUICommand command = new SetTimeScaleVisibleUICommand(timeScale);
				BarCheckItem item = new BarCheckItem();
				item.Command = command;
				item.CommandParameter = SchedulerControl;
				item.IsPrivate = true;
				SchedulerControl.CommandManager.UpdateBarItemDefaultValues(item);
				SchedulerControl.CommandManager.UpdateBarItemCommandUIState(item, command.CreateCommand(SchedulerControl));
				ItemLinks.Add(item);
			}
		}
	}
}
