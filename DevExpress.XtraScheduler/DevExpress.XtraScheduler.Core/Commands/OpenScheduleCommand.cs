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
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.Exchange;
using DevExpress.XtraScheduler.Services.Internal;
using System.IO;
using DevExpress.XtraScheduler.Localization;
namespace DevExpress.XtraScheduler.Commands {
	public class OpenScheduleCommand : SchedulerCommand {
		public OpenScheduleCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		public OpenScheduleCommand(InnerSchedulerControl control)
			: base(control) {
		}
		public override SchedulerMenuItemId MenuId { get { return SchedulerMenuItemId.OpenSchedule; } }
		public override Localization.SchedulerStringId DescriptionStringId { get { return SchedulerStringId.DescCmd_OpenSchedule; } }
		public override Localization.SchedulerStringId MenuCaptionStringId { get { return SchedulerStringId.MenuCmd_OpenSchedule; } }
		public override string ImageName {			get { return SchedulerCommandImagesNames.OpenSchedule; } }
		public override void ForceExecute(Utils.Commands.ICommandUIState state) {
			AppointmentImporter importer = CreateImporter();
			if (importer == null)
				return;
			IFileOperationService fileService = Control.GetService<IFileOperationService>();
			if (fileService == null)
				return;
			using (Stream stream = fileService.OpenRead("Calendar", "iCalendar files (*.ics)|*.ics", 1)) {
				if (stream != null)
					importer.Import(stream);
			}
		}
		public override bool CanExecute() {
			IFileOperationService fileService = Control.GetService<IFileOperationService>();
			return fileService != null;
		}
		protected override void UpdateUIStateCore(Utils.Commands.ICommandUIState state) {
			IFileOperationService fileService = Control.GetService<IFileOperationService>();
			bool enabled = fileService != null;
			state.Visible = enabled;
			state.Enabled = enabled;
		}
		protected virtual AppointmentImporter CreateImporter() {
			AppointmentImporter importer = ExchangeHelper.CreateImporter(Control.Storage, ExchangeHelper.GetICalendarAssemblyName(), "DevExpress.XtraScheduler.iCalendar.iCalendarImporter");
			return importer;
		}
	}
}
