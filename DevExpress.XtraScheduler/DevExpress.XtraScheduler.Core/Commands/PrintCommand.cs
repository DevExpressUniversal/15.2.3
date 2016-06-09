﻿#region Copyright (c) 2000-2015 Developer Express Inc.
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
using DevExpress.XtraScheduler.Localization;
using DevExpress.XtraScheduler.Services;
using DevExpress.Utils.Commands;
namespace DevExpress.XtraScheduler.Commands {
	#region PrintBaseCommand (abstract class)
	public abstract class PrintBaseCommand : SchedulerMenuItemSimpleCommand {
		protected PrintBaseCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		protected PrintBaseCommand(InnerSchedulerControl control)
			: base(control) {
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			ISchedulerPrintService printService = InnerControl.GetService<ISchedulerPrintService>();
			state.Visible = printService != null;
			if (printService == null)
				return;
			state.Enabled = true;
		}
		protected internal override void ExecuteCore() {
			ISchedulerPrintService printService = InnerControl.GetService<ISchedulerPrintService>();
			if (printService == null)
				return;
			ExecutePrintAction(printService);
		}
		protected abstract void ExecutePrintAction(ISchedulerPrintService printService);
	}
	#endregion
	#region PrintCommand
	public class PrintCommand : PrintBaseCommand {
		public PrintCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		public PrintCommand(InnerSchedulerControl control)
			: base(control) {
		}
		#region Properties
		public override SchedulerMenuItemId MenuId {
			get { return SchedulerMenuItemId.Print; }
		}
		public override SchedulerStringId MenuCaptionStringId {
			get { return SchedulerStringId.MenuCmd_Print; }
		}
		public override SchedulerStringId DescriptionStringId {
			get { return SchedulerStringId.DescCmd_Print; }
		}
		public override string ImageName {
			get { return SchedulerCommandImagesNames.Print; }
		}
		#endregion
		protected override void ExecutePrintAction(ISchedulerPrintService printService) {
			printService.Print();
		}		
	}
	#endregion
}
