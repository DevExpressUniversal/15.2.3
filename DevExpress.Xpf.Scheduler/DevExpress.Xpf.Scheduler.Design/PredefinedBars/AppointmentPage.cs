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

extern alias Platform;
using System;
using Platform::DevExpress.Xpf.Core.Design;
using Platform::DevExpress.Xpf.Scheduler.UI;
namespace DevExpress.Xpf.Scheduler.Design {
	public partial class BarInfos {
		#region AppointmentActions
		public static BarInfo AppointmentActions { get { return appointmentActions; } }
		static readonly BarInfo appointmentActions = new BarInfo(
			"AppointmentTools",
			"Appointment",
			"Actions",
			new BarInfoItems(
				new string[] { "EditAppointmentSeriesGroup", "EditAppointment", "DeleteAppointmentSeriesGroup", "DeleteAppointment" },
				new BarItemInfo[] { new SchedulerSubItemInfo(typeof(EditAppointmentSeriesBarItem)), BarItemInfos.Button, new SchedulerSubItemInfo(typeof(DeleteAppointmentSeriesBarItem)), BarItemInfos.Button}
				),
			String.Empty,
			"Caption_PageCategoryAppointmentTools",
			"Caption_PageAppointment",
			"Caption_GroupAppointmentActions",
			"ToolsAppointmentCommandGroup"
			);
		#endregion
		#region AppointmentOptions
		public static BarInfo AppointmentOptions { get { return appointmentOptions; } }
		static readonly BarInfo appointmentOptions = new BarInfo(
			"AppointmentTools",
			"Appointment",
			"Options",
			new BarInfoItems(
					new string[] { "ChangeAppointmentStatus", "ChangeAppointmentLabel", "ToggleRecurrence" },
					new BarItemInfo[] { new ColorablePopupMenuSubItemInfo(typeof(ChangeStatusBarItem)), new ColorablePopupMenuSubItemInfo(typeof(ChangeLabelBarItem)), BarItemInfos.Check }
				),
			String.Empty,
			"Caption_PageCategoryAppointmentTools",
			"Caption_PageAppointment",
			"Caption_GroupAppointmentOptions",
			"ToolsAppointmentCommandGroup"
			);
		#endregion
	}
}
