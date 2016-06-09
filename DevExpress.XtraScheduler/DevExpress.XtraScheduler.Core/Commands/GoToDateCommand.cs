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
	#region GotoDateCommand
	public class GotoDateCommand : SchedulerMenuItemSimpleCommand {
		#region Fields
		DateTime date;
		#endregion
		public GotoDateCommand(InnerSchedulerControl control)
			: this(control, control.Selection.FirstSelectedInterval.Start.Date) {
		}
		public GotoDateCommand(InnerSchedulerControl control, DateTime date)
			: base(control) {
			this.date = date.Date;
		}
		#region Properties
		public override SchedulerCommandId Id { get { return SchedulerCommandId.GotoDate; } }
		protected internal DateTime Date { get { return date; } }
		public override SchedulerMenuItemId MenuId { get { return SchedulerMenuItemId.GotoDate; } }
		public override SchedulerStringId MenuCaptionStringId { get { return SchedulerStringId.MenuCmd_GotoDate; } }
		public override SchedulerStringId DescriptionStringId { get { return SchedulerStringId.Caption_EmptyResource; } }
		public override string ImageName { get { return SchedulerCommandImagesNames.GoToDate; } }
		#endregion
		protected internal override void ExecuteCore() {
			InnerControl.Owner.ShowGotoDateForm(date);
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
		}
	}
	#endregion
}
