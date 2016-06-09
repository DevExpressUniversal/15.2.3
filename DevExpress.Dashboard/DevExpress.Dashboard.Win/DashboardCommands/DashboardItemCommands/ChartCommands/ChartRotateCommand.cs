#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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

using DevExpress.DashboardCommon;
using DevExpress.DashboardWin.Native;
using DevExpress.DashboardWin.Localization;
namespace DevExpress.DashboardWin.Commands {
	public class ChartRotateCommand : DashboardItemInteractionCommand<ChartDashboardItem> {
		public override DashboardCommandId Id { get { return DashboardCommandId.ChartRotate; } }
		public override DashboardWinStringId MenuCaptionStringId { get { return DashboardWinStringId.CommandChartRotateCaption; } }
		public override DashboardWinStringId DescriptionStringId { get { return DashboardWinStringId.CommandChartRotateDescription; } }
		public override string ImageName { get { return "ChartRotate"; } }
		public ChartRotateCommand(DashboardDesigner control)
			: base(control) {
		}
		protected override bool CheckDashboardItem(ChartDashboardItem item) {
			return item.Rotated;
		}
		protected override IHistoryItem CreateHistoryItem(ChartDashboardItem dashboardItem, bool enabled) {
			return new ChartRotateHistoryItem(dashboardItem);
		}
	}
	public class ScatterChartRotateCommand : DashboardItemInteractionCommand<ScatterChartDashboardItem> {
		public override DashboardCommandId Id { get { return DashboardCommandId.ScatterChartRotate; } }
		public override DashboardWinStringId MenuCaptionStringId { get { return DashboardWinStringId.CommandChartRotateCaption; } }
		public override DashboardWinStringId DescriptionStringId { get { return DashboardWinStringId.CommandChartRotateDescription; } }
		public override string ImageName { get { return "ChartRotate"; } }
		public ScatterChartRotateCommand(DashboardDesigner control)
			: base(control) {
		}
		protected override bool CheckDashboardItem(ScatterChartDashboardItem item) {
			return item.Rotated;
		}
		protected override IHistoryItem CreateHistoryItem(ScatterChartDashboardItem dashboardItem, bool enabled) {
			return new ChartRotateHistoryItem(dashboardItem);
		}
	}
}
