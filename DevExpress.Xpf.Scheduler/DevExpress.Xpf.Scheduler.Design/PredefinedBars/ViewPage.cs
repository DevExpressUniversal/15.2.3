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
	public static partial class BarInfos {
		#region ViewSelector
		public static BarInfo ViewSelector { get { return viewSelector; } }
		static readonly BarInfo viewSelector = new BarInfo(
			String.Empty,
			"PageView",
			"ViewSelector",
			new BarInfoItems(
				new string[] { "SwitchToDayView", "SwitchToWorkWeekView", "SwitchToWeekView", "SwitchToFullWeekView", "SwitchToMonthView", "SwitchToTimelineView" },
				new BarItemInfo[] { BarItemInfos.Check, BarItemInfos.Check, BarItemInfos.Check, BarItemInfos.Check, BarItemInfos.Check, BarItemInfos.Check }
				),
			String.Empty,
			String.Empty,
			"Caption_PageView",
			"Caption_GroupViewSelector"
			);
		#endregion
		#region TimeScale
		public static BarInfo TimeScale { get { return timeScale; } }
		static readonly BarInfo timeScale = new BarInfo(
			String.Empty,
			"PageView",
			"TimeScale",
			new BarInfoItems(
				new String[] { "SwitchTimeScalesUICommand", "SetTimeIntervalCount", "SwitchTimeScalesCaptionUICommand"},
				new BarItemInfo[] { new SchedulerSubItemInfo(typeof(TimeScaleBarSubItem)), BarItemInfos.Spin, new SchedulerSubItemInfo(typeof(SwitchTimeScalesCaptionBarSubItem))}),
			String.Empty,
			String.Empty,
			"Caption_PageView",
			"Caption_GroupTimeScale"
			);
		#endregion
		#region Layout
		public static BarInfo Layout { get { return layout; } }
		static readonly BarInfo layout = new BarInfo(
			String.Empty,
			"PageView",
			"Layout",
			new BarInfoItems(
				new String[] { "SwitchCompressWeekend", "SwitchShowWorkTimeOnly", "ChangeSnapToCellsUI" },
				new BarItemInfo[] { BarItemInfos.Check, BarItemInfos.Check, new SchedulerSubItemInfo(typeof(SnapToCellsSubItem)) }),
			String.Empty,
			String.Empty,
			"Caption_PageView",
			"Caption_GroupLayout"
			);
		#endregion
	}
}
