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
using System.Text;
using System.Windows;
namespace DevExpress.Xpf.Grid.Native {
	public class HorizontalNavigationStrategyBase {
		public static readonly HorizontalNavigationStrategyBase NormalHorizontalNavigationStrategyBaseInstance = new HorizontalNavigationStrategyBase();
		protected HorizontalNavigationStrategyBase() {
		}
		public virtual void OnInvalidateHorizontalScrolling(DataViewBase view) { }
		public virtual void OnNavigationIndexChanged(DataViewBase view) { }
		public virtual void MoveNextNavigationIndex(DataViewBase view) {
			MoveNextNavigationIndex(view, false);
		}
		internal virtual void MoveNextNavigationIndex(DataViewBase view, bool isTabNavigation) {		
			DependencyObject obj = view.FindNearRightNavigationIndex(view.NavigationIndex, isTabNavigation);
			if (obj == null)
				return;
			view.NavigationIndex = ColumnBase.GetNavigationIndex(obj);
		}
		public virtual void MovePrevNavigationIndex(DataViewBase view) {
			MovePrevNavigationIndex(view, false);
		}
		internal virtual void MovePrevNavigationIndex(DataViewBase view, bool isTabNavigation) {
			DependencyObject obj = view.FindNearLeftNavigationIndex(view.NavigationIndex, isTabNavigation);
			if (obj == null)
				return;
			view.NavigationIndex = ColumnBase.GetNavigationIndex(obj);
		}
		public virtual void MoveLastNavigationIndex(DataViewBase view) {
			MoveLastNavigationIndex(view, false);
		}
		internal virtual void MoveLastNavigationIndex(DataViewBase view, bool isTabNavigation) {
			DependencyObject obj = view.FindNavigationIndex(view.NavigationIndex, int.MaxValue, false, isTabNavigation);
			if (obj == null)
				return;
			view.NavigationIndex = ColumnBase.GetNavigationIndex(obj);
		}
		public virtual void MoveFirstNavigationIndex(DataViewBase view) {
			MoveFirstNavigationIndex(view, false);
		}
		internal virtual void MoveFirstNavigationIndex(DataViewBase view, bool isTabNavigation) {
			DependencyObject obj = view.FindNavigationIndex(0, view.NavigationIndex, true, isTabNavigation);
			if (obj == null)
				return;
			view.NavigationIndex = ColumnBase.GetNavigationIndex(obj);
		}
		internal void MoveFirstNavigationIndexCore(DataViewBase view, bool isTabNavigation) {
			int navigationIndex = FindFirstNavigationIndex(view, isTabNavigation);
			if(navigationIndex != Constants.InvalidNavigationIndex) {
				view.NavigationIndex = navigationIndex;
			}
		}
		public virtual bool IsBeginNavigationIndex(DataViewBase view) {
			return IsBeginNavigationIndex(view, false);
		}
		internal virtual bool IsBeginNavigationIndex(DataViewBase view, bool isTabNavigation) {
			if (view.DataControl == null)
				return false;
			DependencyObject dobj = view.FindNearRightNavigationIndex(int.MinValue, isTabNavigation);
			if (dobj == null)
				return false;
			return view.NavigationIndex == ColumnBase.GetNavigationIndex(dobj);
		}
		public virtual bool IsEndNavigationIndex(DataViewBase view) {
			return IsEndNavigationIndex(view, false);
		}
		internal virtual bool IsEndNavigationIndex(DataViewBase view, bool isTabNavigation) {
			if (view.DataControl == null)
				return false;
			DependencyObject dobj = view.FindNearLeftNavigationIndex(int.MaxValue, isTabNavigation);
			if (dobj == null)
				return false;
			return view.NavigationIndex == ColumnBase.GetNavigationIndex(dobj);
		}
		public virtual bool OnBeforeChangePixelScrollOffset(DataViewBase view) {
			return true;
		}
		protected int FindNextNavigationIndex(DataViewBase view, bool isTabNavigation, int currentIndex) {
			for(int i = currentIndex + 1; i < view.VisibleColumnsCore.Count; i++) {
				if(view.IsColumnNavigatable(view.VisibleColumnsCore[i], isTabNavigation))
					return i;
			}
			return Constants.InvalidNavigationIndex;
		}
		protected int FindFirstNavigationIndex(DataViewBase view, bool isTabNavigation) {
			return FindNextNavigationIndex(view, isTabNavigation, -1);
		}
	}
	public class NormalHorizontalNavigationStrategy : HorizontalNavigationStrategyBase {
		public static readonly NormalHorizontalNavigationStrategy NormalHorizontalNavigationStrategyInstance = new NormalHorizontalNavigationStrategy();
		protected NormalHorizontalNavigationStrategy() { }
		public virtual void MakeCellVisible(TableViewBehavior behavior) {
			behavior.MakeCurrentCellVisible();
		}
		public virtual void UpdateFixedNoneCellData(ColumnsRowDataBase rowData, TableViewBehavior behavior) {
			rowData.UpdateFixedNoneCellData(false);
		}
		public virtual void UpdateViewportVisibleColumns(TableViewBehavior behavior) {
			behavior.TableView.ViewportVisibleColumns = null;
			behavior.ResetHorizontalVirtualizationOffset();
		}
	}
	public class VirtualizedHorizontalNavigationStrategy : NormalHorizontalNavigationStrategy {
		public static readonly VirtualizedHorizontalNavigationStrategy VirtualizedHorizontalNavigationStrategyInstance = new VirtualizedHorizontalNavigationStrategy();
		VirtualizedHorizontalNavigationStrategy() { }
		internal override void MoveNextNavigationIndex(DataViewBase view, bool isTabNavigation) {		
			if (view.NavigationIndex < 0)
				view.MoveFirstNavigationIndex();
			else if (view.NavigationIndex < view.VisibleColumnsCore.Count - 1) {
				int navigationIndex = FindNextNavigationIndex(view, isTabNavigation, view.NavigationIndex);
				if (navigationIndex == Constants.InvalidNavigationIndex) {
					int firstNavigationIndex = FindFirstNavigationIndex(view, isTabNavigation);
					if (firstNavigationIndex != Constants.InvalidNavigationIndex && view.CanMoveNextRow()) {
						view.MoveNextRow();
						view.NavigationIndex = firstNavigationIndex;
					}
				} else {
					view.NavigationIndex = navigationIndex;
				}
			}
		}
		internal override void MovePrevNavigationIndex(DataViewBase view, bool isTabNavigation) {			
			if (view.NavigationIndex < 0)
				view.MoveFirstNavigationIndex();
			else if (view.NavigationIndex > 0) {
				int navigationIndex = FindPrevNavigationIndex(view, isTabNavigation, view.NavigationIndex);
				if (navigationIndex == Constants.InvalidNavigationIndex) {
					int lastNavigationIndex = FindLastNavigationIndex(view, isTabNavigation);
					if (lastNavigationIndex != Constants.InvalidNavigationIndex && view.CanMovePrevRow()) {
						view.MovePrevRow();
						view.NavigationIndex = lastNavigationIndex;
					}
				} else {
					view.NavigationIndex = navigationIndex;
				}
			}
		}
		internal override void MoveLastNavigationIndex(DataViewBase view, bool isTabNavigation) {
			if (view.IsExpandableRowFocused())
				base.MoveLastNavigationIndex(view, isTabNavigation);
			else {
				int navigationIndex = FindLastNavigationIndex(view, isTabNavigation);
				if (navigationIndex != Constants.InvalidNavigationIndex) {
					view.NavigationIndex = navigationIndex;
				}
			}
		}
		internal override void MoveFirstNavigationIndex(DataViewBase view, bool isTabNavigation) {
			if(view.IsExpandableRowFocused())
				base.MoveFirstNavigationIndex(view, isTabNavigation);
			else {
				MoveFirstNavigationIndexCore(view, isTabNavigation);
			}
		}
		public override void MoveNextNavigationIndex(DataViewBase view) {
			MoveNextNavigationIndex(view, false);
		}
		public override void MovePrevNavigationIndex(DataViewBase view) {
			MovePrevNavigationIndex(view, false);
		}
		public override void MoveLastNavigationIndex(DataViewBase view) {
			MoveLastNavigationIndex(view, false);
		}
		public override void MoveFirstNavigationIndex(DataViewBase view) {
			MoveFirstNavigationIndex(view, false);
		}
		int FindPrevNavigationIndex(DataViewBase view, bool isTabNavigation, int currentIndex) {
			for (int i = currentIndex - 1; i > -1; i--) {
				if (view.IsColumnNavigatable(view.VisibleColumnsCore[i], isTabNavigation)) {
					return i;
				}
			}
			return Constants.InvalidNavigationIndex;
		}
		int FindLastNavigationIndex(DataViewBase view, bool isTabNavigation) {
			return FindPrevNavigationIndex(view, isTabNavigation, view.VisibleColumnsCore.Count);
		}
		public override void OnNavigationIndexChanged(DataViewBase view) {
			if(view.NavigationIndex != Constants.InvalidNavigationIndex)
				((ITableView)view).TableViewBehavior.MakeColumnVisible(view.DataControl.CurrentColumn);
		}
		public override void MakeCellVisible(TableViewBehavior view) { }
		public override void UpdateFixedNoneCellData(ColumnsRowDataBase rowData, TableViewBehavior behavior) {
			rowData.UpdateFixedNoneCellData(true);
		}
		public override void UpdateViewportVisibleColumns(TableViewBehavior behavior) {
			behavior.UpdateViewportVisibleColumnsCore();
		}
		public override void OnInvalidateHorizontalScrolling(DataViewBase view) {
			view.RowsStateDirty = true;
		}
		internal override bool IsBeginNavigationIndex(DataViewBase view, bool isTabNavigation) {
			return view.NavigationIndex == FindFirstNavigationIndex(view, isTabNavigation);
		}	   
		internal override bool IsEndNavigationIndex(DataViewBase view, bool isTabNavigation) {
			return view.NavigationIndex == FindLastNavigationIndex(view, isTabNavigation);
		}
		public override bool OnBeforeChangePixelScrollOffset(DataViewBase view) {
			return view.RequestUIUpdate();
		}
	}
}
