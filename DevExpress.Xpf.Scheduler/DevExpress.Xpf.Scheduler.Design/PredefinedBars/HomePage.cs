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
namespace DevExpress.Xpf.Scheduler.Design {
	public static partial class BarInfos {
		#region ViewNavigator
		public static BarInfo ViewNavigator { get { return viewNavigator; } }
		static readonly BarInfo viewNavigator = new BarInfo(
			String.Empty,
			"PageHome",
			"ViewNavigator",
			new BarInfoItems(
				new string[] { "NavigateViewBackward", "NavigateViewForward", "GotoToday", "ViewZoomIn", "ViewZoomOut" },
				new BarItemInfo[] { BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.Button }
				),
			String.Empty,
			String.Empty,
			"Caption_PageHome",
			"Caption_GroupViewNavigator"
			);
		#endregion
		#region ArrangeView
		public static BarInfo ArrangeView { get { return arrangeView; } }
		static readonly BarInfo arrangeView = new BarInfo(
			String.Empty,
			"PageHome",
			"ArrangeView",
			new BarInfoItems(
				new string[] { "SwitchToDayView", "SwitchToWorkWeekView", "SwitchToWeekView", "SwitchToFullWeekView", "SwitchToMonthView", "SwitchToTimelineView" },
				new BarItemInfo[] { BarItemInfos.Check, BarItemInfos.Check, BarItemInfos.Check, BarItemInfos.Check, BarItemInfos.Check, BarItemInfos.Check }
				),
			String.Empty,
			String.Empty,
			"Caption_PageHome",
			"Caption_GroupArrangeView"
			);
		#endregion
		#region GroupSelector
		public static BarInfo GroupSelector { get { return groupSelector; } }
		static readonly BarInfo groupSelector = new BarInfo(
			String.Empty,
			"PageHome",
			"GroupBy",
			new BarInfoItems(
				new String[] { "SwitchToGroupByNone", "SwitchToGroupByDate", "SwitchToGroupByResource" },
				new BarItemInfo[] { BarItemInfos.Check, BarItemInfos.Check, BarItemInfos.Check }
				),
			String.Empty,
			String.Empty,
			"Caption_PageHome",
			"Caption_GroupGroupBy"
			);
		#endregion
		#region Appointment
		public static BarInfo Appointment { get { return appointment; } }
		static BarInfo appointment = new BarInfo(
			String.Empty,
			"PageHome",
			"Appointment",
			new BarInfoItems(
					new String[] { "NewAppointment", "NewRecurringAppointment" },
					new BarItemInfo[] { BarItemInfos.Button, BarItemInfos.Button }
				),
			String.Empty,
			String.Empty,
			"Caption_PageHome",
			"Caption_GroupAppointment"
			);
		#endregion
	}
}
