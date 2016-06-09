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
using DevExpress.Xpf.Grid.TreeList;
using DevExpress.XtraGrid;
using DevExpress.Data;
namespace DevExpress.Xpf.Grid {
	public class TreeListNodeComparer : Comparer<TreeListNode> {
		TreeListDataProvider provider;
		IList<GridSortInfo> sortInfo;
		public TreeListNodeComparer(TreeListDataProvider provider) {
			this.provider = provider;
		}
		protected TreeListDataProvider DataProvider { get { return provider; } }
		public void SetSortInfo(IList<GridSortInfo> sortInfo) {
			this.sortInfo = sortInfo;
		}
		public override int Compare(TreeListNode node1, TreeListNode node2) {
			if(node1 == node2) return 0;
			int result = 0;
			TreeListCustomColumnSortEventArgs e = null;
			ColumnSortOrder lastOrder = ColumnSortOrder.None;
			if(sortInfo != null)
				foreach(GridSortInfo info in sortInfo) {
					ColumnBase column = DataProvider.View.ColumnsCore[info.FieldName];
					if(column == null) continue;
					object value1 = GetNodeValue(node1, column);
					object value2 = GetNodeValue(node2, column);
					if(e == null)
						e = new TreeListCustomColumnSortEventArgs(column, node1, node2, value1, value2);
					else
						e.SetArgs(column, value1, value2);
					result = CompareInternal(node1, node2, value1, value2, column, e);
					ColumnSortOrder sortOrder = info.GetSortOrder();
					if(sortOrder == ColumnSortOrder.Descending)
						result = -result;
					lastOrder = sortOrder;
					if(result != 0) break;
				}
			if(result == 0) {
				result = Comparer<int>.Default.Compare(node1.Id, node2.Id);
				if(lastOrder == ColumnSortOrder.Descending)
					result = -result;
			}
			return result;
		}
		protected object GetNodeValue(TreeListNode node, ColumnBase column) {
			return DataProvider.View.GetNodeValue(node, column);
		}
		protected int CompareInternal(TreeListNode node1, TreeListNode node2, object value1, object value2, ColumnBase column, TreeListCustomColumnSortEventArgs e) {
			ColumnSortMode sortMode = column.SortMode;
			if(sortMode == ColumnSortMode.Custom) {
				DataProvider.CustomNodeSort(e);
				if(e.Handled)
					return e.Result;
			}
			if(value1 == DBNull.Value || value1 == null) {
				if(value2 == DBNull.Value || value2 == null)
					return 0;
				return -1;
			}
			if(value2 == DBNull.Value || value2 == null)
				return 1;
			if(sortMode == ColumnSortMode.DisplayText) {
				string text1, text2;
				text1 = DataProvider.View.GetNodeDisplayText(node1, column.FieldName, value1);
				text2 = DataProvider.View.GetNodeDisplayText(node2, column.FieldName, value2);
				return Comparer<string>.Default.Compare(text1, text2);
			}
			return Comparer<object>.Default.Compare(value1, value2);
		}
	}
}
