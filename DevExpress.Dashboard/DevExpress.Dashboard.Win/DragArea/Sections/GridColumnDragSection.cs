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

using System.Collections.Generic;
using DevExpress.DashboardCommon;
using DevExpress.XtraEditors;
using DevExpress.DashboardWin.DragDrop;
namespace DevExpress.DashboardWin.Native {
	public class GridColumnDragSection : HolderCollectionDragSection<GridColumnBase> {
		const string autoColumnId = "GridAutoColumn";
		protected override bool AllowNonNumericMeasures {
			get { return true; }
		}
		public GridColumnDragSection(DragArea area, string caption, string itemName, string itemNamePlural, GridColumnCollection columns)
			: base(area, caption, itemName, itemNamePlural, columns) {
		}
		protected override HolderCollectionDragGroup<GridColumnBase> CreateGroupInternal(GridColumnBase column) {
			GridColumnBase actualColumn = column ?? new GridDimensionColumn();
			bool autoMode = (column == null);
			string columnId = autoMode ? autoColumnId : actualColumn.ColumnId;
			return new GridColumnDragGroup(string.Format("Groups.{0}", columnId), actualColumn, autoMode);
		}
		public override XtraForm CreateOptionsForm(DashboardDesigner designer, DragGroup group) {
			GridDashboardItem gridDashboardItem = Area.DashboardItem as GridDashboardItem;
			GridColumnDragGroup columnGroup = group as GridColumnDragGroup;
			return (gridDashboardItem == null || columnGroup == null) ? null : new GridOptionsForm(columnGroup, designer, gridDashboardItem);
		}
		public override bool AcceptableDragObject(IDragObject dragObject) {
			return true;
		}
	}
}
