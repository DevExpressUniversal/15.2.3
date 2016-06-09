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
	public class GridAutoFitToContentsColumnWidthModeCommand : DashboardItemInteractionCommand<GridDashboardItem> {
		public override DashboardCommandId Id { get { return DashboardCommandId.GridAutoFitToContentsColumnWidthMode; } }
		public override DashboardWinStringId MenuCaptionStringId { get { return DashboardWinStringId.CommandGridAutoFitToContentsColumnWidthModeCaption; } }
		public override DashboardWinStringId DescriptionStringId { get { return DashboardWinStringId.CommandGridAutoFitToContentsColumnWidthModeDescription; } }
		public override string ImageName { get { return "GridAutoFitToContentsColumnWidthMode"; } }
		public GridAutoFitToContentsColumnWidthModeCommand(DashboardDesigner control)
			: base(control) {
		}
		protected override bool CheckDashboardItem(GridDashboardItem item) {
			return item.GridOptions.ColumnWidthMode == GridColumnWidthMode.AutoFitToContents;
		}
		protected override IHistoryItem CreateHistoryItem(GridDashboardItem dashboardItem, bool enabled) {
			GridDashboardItemViewer itemViewer = (GridDashboardItemViewer)FindDashboardItemViewer(dashboardItem.ComponentName);
			return new GridColumnWidthModeHistoryItem(dashboardItem, itemViewer.GridView.ColumnsWidthOptionsInfo, GridColumnWidthMode.AutoFitToContents);
		}
	}
	public class GridAutoFitToGridColumnWidthModeCommand : DashboardItemInteractionCommand<GridDashboardItem> {
		public override DashboardCommandId Id { get { return DashboardCommandId.GridAutoFitToGridColumnWidthMode; } }
		public override DashboardWinStringId MenuCaptionStringId { get { return DashboardWinStringId.CommandGridAutoFitToGridColumnWidthModeCaption; } }
		public override DashboardWinStringId DescriptionStringId { get { return DashboardWinStringId.CommandGridAutoFitToGridColumnWidthModeDescription; } }
		public override string ImageName { get { return "GridAutoFitToGridColumnWidthMode"; } }
		public GridAutoFitToGridColumnWidthModeCommand(DashboardDesigner control)
			: base(control) {
		}
		protected override bool CheckDashboardItem(GridDashboardItem item) {
			return item.GridOptions.ColumnWidthMode == GridColumnWidthMode.AutoFitToGrid;
		}
		protected override IHistoryItem CreateHistoryItem(GridDashboardItem dashboardItem, bool enabled) {
			GridDashboardItemViewer itemViewer = (GridDashboardItemViewer)FindDashboardItemViewer(dashboardItem.ComponentName);
			return new GridColumnWidthModeHistoryItem(dashboardItem, itemViewer.GridView.ColumnsWidthOptionsInfo, GridColumnWidthMode.AutoFitToGrid);
		}
	}
	public class GridManualGridColumnWidthModeCommand : DashboardItemInteractionCommand<GridDashboardItem> {
		public override DashboardCommandId Id { get { return DashboardCommandId.GridManualGridColumnWidthMode; } }
		public override DashboardWinStringId MenuCaptionStringId { get { return DashboardWinStringId.CommandGridManualGridColumnWidthModeCaption; } }
		public override DashboardWinStringId DescriptionStringId { get { return DashboardWinStringId.CommandGridManualGridColumnWidthModeDescription; } }
		public override string ImageName { get { return "GridManualGridColumnWidthMode"; } }
		public GridManualGridColumnWidthModeCommand(DashboardDesigner control)
			: base(control) {
		}
		protected override bool CheckDashboardItem(GridDashboardItem item) {
			return item.GridOptions.ColumnWidthMode == GridColumnWidthMode.Manual;
		}
		protected override IHistoryItem CreateHistoryItem(GridDashboardItem dashboardItem, bool enabled) {
			GridDashboardItemViewer itemViewer = (GridDashboardItemViewer)FindDashboardItemViewer(dashboardItem.ComponentName);
			return new GridColumnWidthModeHistoryItem(dashboardItem, itemViewer.GridView.ColumnsWidthOptionsInfo, GridColumnWidthMode.Manual);
		}
	}
}
