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

using DevExpress.Xpf.Scheduler.Commands;
using DevExpress.XtraScheduler.Commands;
using DevExpress.XtraScheduler.Services.Implementation;
using System;
namespace DevExpress.Xpf.Scheduler.Services {
	public class XpfSchedulerCommandFactoryService : SchedulerCommandFactoryService {
		public XpfSchedulerCommandFactoryService(ISchedulerCommandTarget target)
			: base(target) {
		}
		protected internal override void PopulateConstructorTable(SchedulerCommandConstructorTable table) {
			base.PopulateConstructorTable(table);
			RemoveCommandConstructor(table, SchedulerCommandId.ViewZoomIn);
			RemoveCommandConstructor(table, SchedulerCommandId.ViewZoomOut);
			RemoveCommandConstructor(table, SchedulerCommandId.EditAppointmentUI);
			RemoveCommandConstructor(table, SchedulerCommandId.DeleteAppointmentsUI);
			AddCommandConstructor(table, SchedulerCommandId.DeleteAppointmentsUI, typeof(XpfDeleteAppointmentUICommand));
			AddCommandConstructor(table, SchedulerCommandId.EditAppointmentUI, typeof(XpfEditNormalAppointmentUICommand));
			AddCommandConstructor(table, SchedulerCommandId.ViewZoomIn, typeof(XpfViewZoomInCommand));
			AddCommandConstructor(table, SchedulerCommandId.ViewZoomOut, typeof(XpfViewZoomOutCommand));
			AddCommandConstructor(table, SchedulerCommandId.SetTimeIntervalCount, typeof(SetTimeIntervalCountCommand));
			AddCommandConstructor(table, SchedulerCommandId.SelectNextAppointment, typeof(SelectNextAppointmentCommand));
			AddCommandConstructor(table, SchedulerCommandId.MoveFocusNext, typeof(MoveFocusNextCommand));
			AddCommandConstructor(table, SchedulerCommandId.MoveFocusPrev, typeof(MoveFocusPrevCommand));
			AddCommandConstructor(table, SchedulerCommandId.SelectPrevAppointment, typeof(SelectPrevAppointmentCommand));
		}
		protected override void RegisterConstructorParameters() {
			base.RegisterConstructorParameters();
			RegisterConstructorParameters(new Type[] { typeof(SchedulerControl) });
		}
	}
}
