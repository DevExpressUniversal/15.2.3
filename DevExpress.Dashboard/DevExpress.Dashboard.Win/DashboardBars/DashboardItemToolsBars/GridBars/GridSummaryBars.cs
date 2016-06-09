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

using System;
using DevExpress.DashboardCommon;
using DevExpress.DashboardWin.Localization;
using DevExpress.DashboardWin.Native;
using DevExpress.XtraBars;
namespace DevExpress.DashboardWin.Bars {
	public class GridAddColumnTotalsBarItem : BarSubItem, IDashboardViewerCommandBarItem {
		public GridAddColumnTotalsBarItem() {
			Caption = DashboardWinLocalizer.GetString(DashboardWinStringId.AddGridColumnTotalBarItemCaption);
		}
		public void ExecuteCommand(DashboardViewer viewer, DashboardItemViewer itemViewer) {
		}
	}
	public abstract class GridColumnTotalBarItemBase : BarButtonItem, IDashboardViewerCommandBarItem {
		readonly DashboardDesigner designer;
		readonly GridDashboardItem grid;
		readonly int columnIndex;
		readonly int totalIndex;
		protected GridColumnTotalBarItemBase(DashboardDesigner designer, GridDashboardItem grid, int columnIndex, int totalIndex) {
			this.designer = designer;
			this.grid = grid;
			this.columnIndex = columnIndex;
			this.totalIndex = totalIndex;
		}
		public void ExecuteCommand(DashboardViewer viewer, DashboardItemViewer itemViewer) {
			IHistoryItem historyItem = CreateHistoryItem(grid, columnIndex, totalIndex);
			historyItem.Redo(designer);
			designer.History.Add(historyItem);
		}
		protected abstract IHistoryItem CreateHistoryItem(GridDashboardItem grid, int columnIndex, int totalIndex);
	}
	public class CreateNewGridTotalBarItem : GridColumnTotalBarItemBase {
		readonly GridColumnTotalType totalType;
		public CreateNewGridTotalBarItem(DashboardDesigner designer, GridDashboardItem grid, int columnIndex, GridColumnTotalType totalType)
			: base(designer, grid, columnIndex, 0) {
			this.totalType = totalType;
			Caption = totalType.Localize();
			if(totalType != GridColumnTotalType.Auto)
				Glyph = ImageHelper.GetImage(string.Format("PopupMenu.GridTotal_{0}", totalType));
		}
		protected override IHistoryItem CreateHistoryItem(GridDashboardItem grid, int columnIndex, int totalIndex) {
			return new AddGridColumnTotalHistoryItem(grid, columnIndex, totalType);
		}
	}
	public class ClearGridColumnTotalsBarItem : GridColumnTotalBarItemBase {
		public ClearGridColumnTotalsBarItem(DashboardDesigner designer, GridDashboardItem grid, int columnIndex)
			: base(designer, grid, columnIndex, 0) {
			Caption = DashboardWinLocalizer.GetString(DashboardWinStringId.ClearGridColumnTotalsBarItemCaption);
			Glyph = ImageHelper.GetImage("PopupMenu.ClearTotals");
		}
		protected override IHistoryItem CreateHistoryItem(GridDashboardItem grid, int columnIndex, int totalIndex) {
			return new ClearGridColumnTotalsHistoryItem(grid, columnIndex);
		}
	}
	public class ChangeGridColumnTotalTypeBarItem : GridColumnTotalBarItemBase {
		readonly GridColumnTotalType totalType;
		public ChangeGridColumnTotalTypeBarItem(DashboardDesigner designer, GridDashboardItem grid, int columnIndex, int totalIndex, GridColumnTotalType totalType)
			: base(designer, grid, columnIndex, totalIndex) {
			this.totalType = totalType;
			Caption = totalType.Localize();
			if(totalType != GridColumnTotalType.Auto)
				Glyph = ImageHelper.GetImage(string.Format("PopupMenu.GridTotal_{0}", totalType));
		}
		protected override IHistoryItem CreateHistoryItem(GridDashboardItem grid, int columnIndex, int totalIndex) {
			return new ChangeGridColumnTotalTypeHistoryItem(grid, columnIndex, totalIndex, totalType);
		}
	}
	public class RemoveGridColumnTotalBarItem : GridColumnTotalBarItemBase {
		public RemoveGridColumnTotalBarItem(DashboardDesigner designer, GridDashboardItem grid, int columnIndex, int totalIndex)
			: base(designer, grid, columnIndex, totalIndex) {
			Caption = DashboardWinLocalizer.GetString(DashboardWinStringId.RemoveGridColumnTotalBarItemCaption);
			Glyph = ImageHelper.GetImage("PopupMenu.ClearTotals");
		}
		protected override IHistoryItem CreateHistoryItem(GridDashboardItem grid, int columnIndex, int totalIndex) {
			return new RemoveGridColumnTotalHistoryItem(grid, columnIndex, totalIndex);
		}
	}
}
