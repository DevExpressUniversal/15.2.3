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

using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.PivotGrid.QueryMode;
namespace DevExpress.PivotGrid.OLAP {
	internal class OLAPQueryContextBase : QueryContextBase<OLAPCubeColumn>, IOLAPQueryContext {
		List<OLAPCubeColumn> columnRowCustomDefaultMemberColumns;
		public OLAPQueryContextBase(GroupInfo[] columns, GroupInfo[] rows, bool columnExpand, bool rowExpand, IOLAPHelpersOwner owner)
			: base(owner, columns, rows, columnExpand, rowExpand, owner.Areas) {
		}
		void IOLAPQueryContext.ValidateColumnsRows() {
			if((Columns == null) != (Rows == null))
				throw new ArgumentException("Invalid columns and rows parameters");
		}
		protected override void Initialize(GroupInfo[] columns, GroupInfo[] rows, bool columnExpand, bool rowExpand, QueryAreas<OLAPCubeColumn> context) {
			OLAPAreas areas = (OLAPAreas)context;
			this.columnRowCustomDefaultMemberColumns = GetColumnRowCustomDefaultMemberColumns(areas);
		}
		List<OLAPCubeColumn> GetColumnRowCustomDefaultMemberColumns(OLAPAreas Areas) {
			columnRowCustomDefaultMemberColumns = new List<OLAPCubeColumn>();
			AddColumnRowCustomDefaultMemberColumns(Areas.ColumnArea, ColumnArea, ColumnTuples);
			AddColumnRowCustomDefaultMemberColumns(Areas.RowArea, RowArea, RowTuples);
			return columnRowCustomDefaultMemberColumns;
		}
		void AddColumnRowCustomDefaultMemberColumns(List<OLAPCubeColumn> cols, List<OLAPCubeColumn> area, List<QueryTuple> tuples) {
			for(int i = 0; i < cols.Count; i++) {
				OLAPCubeColumn col1 = cols[i];
				if(!(area.Contains(col1) || col1.Filtered || !col1.HasCustomDefaultMember || col1.HasParent || tuples.Any(t => t.AllMembers.Any(m => m.Column == col1.Metadata))))
					columnRowCustomDefaultMemberColumns.Add(cols[i]);
			}
		}
		protected override IActionProvider<OLAPCubeColumn, CellSet<OLAPCubeColumn>> GetActionProvider(GroupInfo[] groups, bool isExpand) {
			if(isExpand)
				return new ExpandActionProvider<OLAPCubeColumn>();
			else {
				if(groups.Length == 0)
					return new FirstLevelActionProvider<OLAPCubeColumn>();
				else
					return new AllGroupsActionProvider();
			}
		}
		#region IOLAPQueryContext Members
		Action<OLAPCubeColumn, string[]> IOLAPQueryContext.QueryMembers { get { return ((IOLAPMetadata)Owner.Metadata).QueryMembers; } }
		List<OLAPCubeColumn> IOLAPQueryContext.ColumnRowCustomDefaultMemberColumns { get { return columnRowCustomDefaultMemberColumns; } }
		#endregion
	}
}
