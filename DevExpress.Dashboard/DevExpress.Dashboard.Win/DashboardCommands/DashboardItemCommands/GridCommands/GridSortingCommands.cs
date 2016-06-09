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

using DevExpress.DashboardWin.Native;
using DevExpress.Data;
namespace DevExpress.DashboardWin.Commands {
	public class GridSortAscendingCommand : DashboardItemViewerCommand {
		readonly GridDashboardColumn column;
		public GridSortAscendingCommand(DashboardViewer viewer, DashboardItemViewer itemViewer, GridDashboardColumn column)
			: base(viewer, itemViewer) {
				this.column = column;
		}
		public override void Execute() {
			column.SortOrder = ColumnSortOrder.Ascending;
		}
	}
	public class GridSortDescendingCommand : DashboardItemViewerCommand {
		readonly GridDashboardColumn column;
		public GridSortDescendingCommand(DashboardViewer viewer, DashboardItemViewer itemViewer, GridDashboardColumn column)
			: base(viewer, itemViewer) {
			this.column = column;
		}
		public override void Execute() {
			column.SortOrder = ColumnSortOrder.Descending;
		}
	}
	public class GridClearSortingCommand : DashboardItemViewerCommand {
		public GridClearSortingCommand(DashboardViewer viewer, DashboardItemViewer itemViewer)
			: base(viewer, itemViewer) {
		}
		public override void Execute() {
			((GridDashboardItemViewer)ItemViewer).GridView.ClearSorting();
		}
	}
}
