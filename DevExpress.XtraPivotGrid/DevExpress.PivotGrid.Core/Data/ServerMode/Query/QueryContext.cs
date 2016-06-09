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

using System.Linq;
using DevExpress.PivotGrid.QueryMode;
using DevExpress.PivotGrid.QueryMode.Sorting;
namespace DevExpress.PivotGrid.ServerMode {
	class FullExpandQueryContext : QueryContext {
		public override bool IsFullExpand { get { return true; } }
		public FullExpandQueryContext(IServerModeHelpersOwner owner, QueryAreas<ServerModeColumn> areas)
			: base(owner, new GroupInfo[0], new GroupInfo[0], false, false, areas) {
		}
		protected override IActionProvider<ServerModeColumn, CellSet<ServerModeColumn>> GetActionProvider(GroupInfo[] groups, bool isExpand) {
			return new FullExpandActionProvider<ServerModeColumn>(Areas);
		}
		public override void PerformSorting() {
			CellTableSorter<ServerModeColumn>.FullSort(this, (Areas)Areas);
		}
	}
	class QueryContext : QueryContextBase<ServerModeColumn>, IServerQueryContext {
		public virtual bool IsFullExpand { get { return false; } }
		public QueryContext(IServerModeHelpersOwner owner, GroupInfo[] columns, GroupInfo[] rows, bool columnExpand, bool rowExpand, QueryAreas<ServerModeColumn> areas)
			: base(owner, columns, rows, columnExpand, rowExpand, areas) {
		}
		protected override void Initialize(GroupInfo[] columns, GroupInfo[] rows, bool columnExpand, bool rowExpand, QueryAreas<ServerModeColumn> context) { }
		protected override IActionProvider<ServerModeColumn, CellSet<ServerModeColumn>> GetActionProvider(GroupInfo[] groups, bool isExpand) {
			if(isExpand)
				return new ExpandActionProvider<ServerModeColumn>();
			else {
				if(groups.Length == 0)
					return new FirstLevelActionProvider<ServerModeColumn>();
				else
					return new AllGroupsActionProvider();
			}
		}
		public override void PerformSorting() {
			SortArea(true);
			SortArea(false);
		}
		void SortArea(bool isColumn) {
			GroupInfo[] groups = isColumn ? Columns : Rows;
			ServerModeColumn column = isColumn ? ColumnArea.FirstOrDefault() : RowArea.FirstOrDefault();
			bool expand = isColumn ? ColumnExpand : RowExpand;
			if(expand) {
				CellTableSorter<ServerModeColumn>.EnsureSortedByCalculatedMeasure(this, (Areas)Areas, groups, column, true);
			} else {
				if(groups != null && groups.Length != 0)
					CellTableSorter<ServerModeColumn>.EnsureSortedByCalculatedMeasure(this, (Areas)Areas, groups, column, true);
			}
		}
	}
}
