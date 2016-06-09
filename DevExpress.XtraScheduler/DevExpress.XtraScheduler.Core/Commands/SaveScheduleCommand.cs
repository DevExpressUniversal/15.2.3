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
namespace DevExpress.XtraScheduler.Commands {
	public class SaveScheduleCommand : SchedulerCommand {
		public SaveScheduleCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		public SaveScheduleCommand(InnerSchedulerControl control)
			: base(control) {
		}
		public override SchedulerMenuItemId MenuId { get { return SchedulerMenuItemId.SaveSchedule; } }
		public override Localization.SchedulerStringId DescriptionStringId { get { return Localization.SchedulerStringId.DescCmd_SaveSchedule; } }
		public override Localization.SchedulerStringId MenuCaptionStringId { get { return Localization.SchedulerStringId.MenuCmd_SaveSchedule; } }
		public override string ImageName { get { return SchedulerCommandImagesNames.SaveSchedule; } }
		public override void ForceExecute(Utils.Commands.ICommandUIState state) {
			AppointmentExporter Exporter = CreateExporter();
			if (Exporter == null)
				return;
			IFileOperationService fileService = Control.GetService<IFileOperationService>();
			if (fileService == null)
				return;
			using (Stream stream = fileService.OpenWrite("Calendar", "iCalendar files (*.ics)|*.ics", 1)) {
				if (stream != null) {
					Exporter.Export(stream);
					stream.Flush();
				}				
			}
		}
		public override bool CanExecute() {
			IFileOperationService fileService = Control.GetService<IFileOperationService>();
			AppointmentExporter Exporter = CreateExporter();
			return Exporter != null && fileService != null;
		}
		protected override void UpdateUIStateCore(Utils.Commands.ICommandUIState state) {
			IFileOperationService fileService = Control.GetService<IFileOperationService>();
			bool enabled = CanCreateExporter() && fileService != null;
			state.Visible = enabled;
			state.Enabled = enabled;
		}
		protected virtual AppointmentExporter CreateExporter() {
			AppointmentExporter Exporter = ExchangeHelper.CreateExporter(Control.Storage, ExchangeHelper.GetICalendarAssemblyName(), "DevExpress.XtraScheduler.iCalendar.iCalendarExporter");
			return Exporter;
		}
		protected virtual bool CanCreateExporter() {
			return ExchangeHelper.CanCreateExporter(ExchangeHelper.GetICalendarAssemblyName(), "DevExpress.XtraScheduler.iCalendar.iCalendarExporter");
		}
	}
}
