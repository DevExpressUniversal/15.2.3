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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Grid.DragDrop;
using DevExpress.Mvvm.UI.Interactivity;
namespace DevExpress.Xpf.Grid {
	[TargetType(typeof(GridControl))]
	public class GridDragDropManager : DataViewDragDropManager {
		#region inner classes
		public class GridDragSource : SupportDragDropBase {
			GridDragDropManager GridDragDropManager { get { return (GridDragDropManager)dragDropManager; } }
			protected override FrameworkElement Owner { get { return GridDragDropManager.DataControl; } }
			public GridDragSource(DragDropManagerBase dragDropManager)
				: base(dragDropManager) {
			}
			protected override FrameworkElement SourceElementCore {
				get { return GridDragDropManager.DataControl; }
			}
		}
		delegate void MoveRowsDelegate(DragDropManagerBase sourceManager, int targetRowHandle, DependencyObject hitElement);
		#endregion
		public GridDragDropManager() { }
		#region Events
		GridStartDragEventHandler StartDraggingEventHandler;
		GridDragOverEventHandler DragOverEventHandler;
		GridDropEventHandler DropEventHandler;
		GridDroppedEventHandler DroppedEventHandler;
		[Category("Events")]
		public event GridStartDragEventHandler StartDrag {
			add { StartDraggingEventHandler += value; }
			remove { StartDraggingEventHandler -= value; }
		}
		[Category("Events")]
		public event GridDragOverEventHandler DragOver {
			add { DragOverEventHandler += value; }
			remove { DragOverEventHandler -= value; }
		}
		[Category("Events")]
		public event GridDropEventHandler Drop {
			add { DropEventHandler += value; }
			remove { DropEventHandler -= value; }
		}
		[Category("Events")]
		public event GridDroppedEventHandler Dropped {
			add { DroppedEventHandler += value; }
			remove { DroppedEventHandler -= value; }
		}
		#endregion
		protected internal GridControl GridControl { get { return DataControl as GridControl; } }
		TableView TableView { get { return View as TableView; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public const string InvalidItemsSourceError = "The Grid's data source cannot be converted to an IList object, thus the GridDragDropManager cannot function properly.";
		protected override ISupportDragDrop CreateDragSource(DataViewDragDropManager dataViewDragDropManager) {
			return new GridDragSource(dataViewDragDropManager);
		}
		public override DataViewBase View {
			get {
				if(GridControl != null)
					return GridControl.View;
				return null;
			}
		}
		protected internal override IList CalcDraggingRows(IndependentMouseEventArgs e) {
			TableViewHitInfo hitInfo = MouseDownHitInfo as TableViewHitInfo;
			if(hitInfo == null || !hitInfo.InRow || hitInfo.HitTest == TableViewHitTest.RowIndicator) {
				return null;
			}
			if(GridControl.GroupCount == 0) {
				return new List<object>(TableView.DataControl.SelectedItems.Cast<object>());
			}
			List<int> collectedRowHandles = new List<int>();
			foreach(int rowHandle in TableView.DataControl.GetSelectedRowHandles()) {
				if(GridControl.IsGroupRowHandle(rowHandle)) {
					collectedRowHandles.AddRange(GetChildRows(rowHandle));
				}
				else {
					collectedRowHandles.Add(rowHandle);
				}
			}
			IEnumerable<int> rowHandles = collectedRowHandles.Distinct();
			List<object> selectedRows = new List<object>();
			foreach(var item in rowHandles) {
				selectedRows.Add(GridControl.GetRow(item));
			}
			return selectedRows;
		}
		protected override bool AllowMouseMoveSelection(MouseEventArgs args) {
			TableView view = TableView;
			if(view != null) {
				DependencyObject source = args.OriginalSource as DependencyObject;
				if(source != null) {
					TableViewHitInfo hitInfo = view.CalcHitInfo(source);
					return hitInfo.HitTest == TableViewHitTest.RowIndicator;
				}
			}
			return false;
		}
		protected virtual List<int> GetChildRows(int groupRowHandle) {
			List<int> handles = new List<int>();
			CollectGroupRowChildren(groupRowHandle, handles);
			return handles;
		}
		void CollectGroupRowChildren(int groupRowHandle, IList handles) {
			int childCount = GridControl.GetChildRowCount(groupRowHandle);
			for(int i = 0; i < childCount; i++) {
				int childRowHandle = GridControl.GetChildRowHandle(groupRowHandle, i);
				if(GridControl.IsGroupRowHandle(childRowHandle)) {
					CollectGroupRowChildren(childRowHandle, handles);
				}
				else {
					handles.Add(childRowHandle);
				}
			}
		}
		protected internal override void OnDrop(DragDropManagerBase sourceManager, UIElement source, Point pt) {
			if(DropEventIsLocked) return;
			GridDropEventArgs e = RaiseDropEvent(sourceManager, pt);
			if(!e.Handled) {
				ValidateItemsSource();
				PerformDropToView(sourceManager, GetHitInfo(HitElement) as TableViewHitInfo, pt, MoveSelectedRows, new MoveRowsDelegate((s, g, h) => MoveSelectedRowsToGroup(s, g, h, true)), AddRows, ReorderGroup);
			}
			RaiseDroppedEvent(sourceManager, e);
			base.OnDrop(sourceManager, source, pt);
		}
		protected override GridViewHitInfoBase GetHitInfo(DependencyObject element) {
			return TableView.CalcHitInfo(element);
		}
		protected override void PerformDropToViewCore(DragDropManagerBase sourceManager) {
			GridViewHitInfoBase hitInfo = GetHitInfo(HitElement);
			if(BanDrop(hitInfo.RowHandle, hitInfo, sourceManager, GetDropTargetTypeByHitElement(HitElement))) {
				ClearDragInfo(sourceManager);
				return;
			}
			PerformDropToView(sourceManager, hitInfo as TableViewHitInfo, LastPosition, SetReorderDropInfo, SetMoveToGroupRowDropInfo, SetAddRowsDropInfo, SetReorderGroupDropInfo);
		}
		void PerformDropToView(DragDropManagerBase sourceManager, TableViewHitInfo hitInfo, Point pt, MoveRowsDelegate reorderDelegate, MoveRowsDelegate groupDelegateExtractor, MoveRowsDelegate addRowsDelegate, MoveRowsDelegate reorderGroupDelegate) {
			int insertRowHandle = hitInfo.RowHandle;
			if(GridControl.IsGroupRowHandle(insertRowHandle)) {
				groupDelegateExtractor(sourceManager, insertRowHandle, HitElement);
				return;
			}
			if(IsSortedButNotGrouped() || hitInfo.HitTest == TableViewHitTest.DataArea) {
				if(sourceManager.DraggingRows.Count > 0 && GetDataAreaElement(HitElement) != null && !ReferenceEquals(sourceManager, this)) {
					addRowsDelegate(sourceManager, insertRowHandle, HitElement);
				}
				else {
					ClearDragInfo(sourceManager);
				}
				return;
			}
			if(insertRowHandle == GridControl.InvalidRowHandle || insertRowHandle == GridControl.AutoFilterRowHandle || insertRowHandle == GridControl.NewItemRowHandle) {
				ClearDragInfo(sourceManager);
				return;
			}
			if(GridControl.GroupCount > 0) {
				int groupRowHandle = GridControl.GetParentRowHandle(insertRowHandle);
				if(ShouldReorderGroup()) {
					reorderGroupDelegate(sourceManager, insertRowHandle, HitElement);
				}
				else
					groupDelegateExtractor(sourceManager, groupRowHandle, HitElement);
			} else {
				reorderDelegate(sourceManager, insertRowHandle, HitElement);
			}
		}
		bool ShouldReorderGroup() {
			return GridControl.SortInfo.Count <= GridControl.GroupCount;
		}
		void ReorderGroup(DragDropManagerBase sourceManager, int insertRowHandle, DependencyObject hitElement) {
			int groupRowHandle = GridControl.GetParentRowHandle(insertRowHandle);
			MoveSelectedRows(sourceManager, insertRowHandle, HitElement);
			if(!IsSameGroup(sourceManager, GetGroupInfos(groupRowHandle), HitElement))
				MoveSelectedRowsToGroup(sourceManager, groupRowHandle, HitElement, false);
		}
		void SetReorderGroupDropInfo(DragDropManagerBase sourceManager, int insertRowHandle, DependencyObject hitElement) {
			int groupRowHandle = GridControl.GetParentRowHandle(insertRowHandle);
			if(!IsSameGroup(sourceManager, GetGroupInfos(groupRowHandle), HitElement))
				SetMoveToGroupRowDropInfo(sourceManager, groupRowHandle, HitElement);
			SetReorderDropInfo(sourceManager, insertRowHandle, HitElement);
		}
		protected override DragDropDragOverEventArgs RaiseDragOverEvent(GridViewHitInfoBase hitInfo, DragDropManagerBase sourceManager, DropTargetType dropTargetType) {
			GridDragOverEventArgs e = new GridDragOverEventArgs(GridControl, this, sourceManager, hitInfo as TableViewHitInfo, sourceManager.DraggingRows, dropTargetType) {
				Handled = false,
			};
			if(DragOverEventHandler != null)
				DragOverEventHandler(this, e);
			return e;
		}
		protected override void AddRow(DragDropManagerBase sourceManager, object row, int insertRowHandle) {
			sourceManager.RemoveObject(row);
			ItemsSource.Add(sourceManager.GetObject(row));
		}
		void SetMoveToGroupRowDropInfo(DragDropManagerBase sourceManager, int insertRowHandle, DependencyObject hitElement) {
			GroupInfo[] groupInfo = GetGroupInfos(insertRowHandle);
			if(CanMoveSelectedRowsToGroup(sourceManager, groupInfo, hitElement)) {
				sourceManager.SetDropTargetType(DropTargetType.InsertRowsIntoGroup);
				sourceManager.ViewInfo.GroupInfo = groupInfo;
				sourceManager.ShowDropMarker(GetRowElement(hitElement), TableDragIndicatorPosition.None);
			}
			else {
				ClearDragInfo(sourceManager);
			}
		}
		bool IsSortedButNotGrouped() {
			return IsSorted() && !IsGrouped();
		}
		bool IsGrouped() {
			return GridControl.GroupCount > 0;
		}
		bool IsSorted() {
			return GridControl.SortInfo.Count > 0;
		}
		void MoveSelectedRows(DragDropManagerBase sourceManager, int insertRowHandle, DependencyObject hitElement) {
			object insertObject = GridControl.GetRow(insertRowHandle);
			if(insertObject == null || sourceManager.DraggingRows == null || sourceManager.DraggingRows.Contains(insertObject))
				return;
			DropTargetType dropTargetType = GetDropTargetTypeByHitElement(hitElement);
			if(dropTargetType == DropTargetType.None)
				return;
			int index = ItemsSource.IndexOf(insertObject) + (dropTargetType == DropTargetType.InsertRowsAfter ? 1 : 0);
			bool isDataTable = ItemsSource is DataView;
			GridControl.BeginDataUpdate();
			foreach(object obj in sourceManager.DraggingRows) {
				if(ReferenceEquals(ItemsSource, sourceManager.GetSource(obj)) &&
					index > ItemsSource.IndexOf(sourceManager.GetObject(obj))) {
					index--;
				}
				MoveRow(sourceManager, obj, index);
				index++;
			}
			GridControl.EndDataUpdate();
			if(RestoreSelection) {
				int startRowHandle = GridControl.GetRowHandleByListIndex(ItemsSource.IndexOf(sourceManager.DraggingRows[0]));
				int endRowHandle = GridControl.GetRowHandleByListIndex(ItemsSource.IndexOf(sourceManager.DraggingRows[sourceManager.DraggingRows.Count - 1]));
				DataControl.SelectRange(startRowHandle, endRowHandle);
			}
			else
				DataControl.UnselectAll();
			if(sourceManager.DraggingRows.Count > 0)
				DataControl.CurrentItem = sourceManager.DraggingRows[0];
		}
		void MoveRow(DragDropManagerBase sourceManager, object obj, int index) {
			object sourceObject = sourceManager.GetObject(obj);
			DataView dataView = ItemsSource as DataView;
			if(dataView != null) {
				DataRow oldRow = ((DataRowView)sourceObject).Row;
				DataRow clonedRow = DragDrop.Utils.CloneDataRow(dataView, sourceObject);
				DataRow rowToInsert = null;
				sourceManager.RemoveObject(obj);
				if(oldRow.RowState != DataRowState.Detached)
					oldRow.AcceptChanges();
				if(sourceManager.ItemsSource == ItemsSource) {
					oldRow.ItemArray = clonedRow.ItemArray;
					rowToInsert = oldRow;
				}
				else {
					rowToInsert = clonedRow;
				}
				dataView.Table.Rows.InsertAt(rowToInsert, index);
				rowToInsert.AcceptChanges();
			}
			else {
				sourceManager.RemoveObject(obj);
				ItemsSource.Insert(index, sourceObject);
			}
		}
		protected virtual GridDropEventArgs RaiseDropEvent(DragDropManagerBase sourceManager, Point pt) {
			UIElement hitElement = GetVisibleHitTestElement(pt);
			TableViewHitInfo hitInfo = GetHitInfo(hitElement) as TableViewHitInfo;
			DropTargetType dropTargetType = GridControl.GroupCount > 0 ? DropTargetType.InsertRowsIntoGroup : GetDropTargetTypeByHitElement(hitElement);
			return RaiseDropEvent(sourceManager, hitInfo, sourceManager.DraggingRows,
				hitInfo.HitTest == TableViewHitTest.DataArea ? DropTargetType.DataArea : dropTargetType);
		}
		protected virtual GridDropEventArgs RaiseDropEvent(DragDropManagerBase sourceManager, TableViewHitInfo hitInfo, IList rows, DropTargetType dropTargetType) {
			GridDropEventArgs e = new GridDropEventArgs(GridControl, this, sourceManager, dropTargetType, hitInfo, rows) {
				Handled = false,
			};
			if(DropEventHandler != null)
				DropEventHandler(this, e);
			return e;
		}
		protected virtual void RaiseDroppedEvent(DragDropManagerBase sourceManager, GridDropEventArgs dropEventArgs) {
			if(DroppedEventHandler != null) {
				GridDroppedEventArgs e = new GridDroppedEventArgs(GridControl, this, sourceManager, dropEventArgs.TargetRowHandle, dropEventArgs.DropTargetType, dropEventArgs.HitInfo, dropEventArgs.DraggedRows);
				DroppedEventHandler(this, e);
			}
		}
		void SetReorderDropInfo(DragDropManagerBase sourceManager, int insertRowHandle, DependencyObject hitElement) {
			FrameworkElement rowElement = GetRowElement(hitElement);
			TableDragIndicatorPosition dragIndicatorPosition = GetDragIndicatorPositionForRowElement(rowElement);
			if(dragIndicatorPosition != TableDragIndicatorPosition.None) {
				DropTargetType dropTargetType = dragIndicatorPosition == TableDragIndicatorPosition.Bottom ? DropTargetType.InsertRowsAfter : DropTargetType.InsertRowsBefore;
				sourceManager.SetDropTargetType(dropTargetType);
				sourceManager.ViewInfo.DropTargetRow = GridControl.GetRow(insertRowHandle);
				sourceManager.ShowDropMarker(rowElement, dragIndicatorPosition);
			}
			else {
				ClearDragInfo(sourceManager);
			}
		}
		void MoveSelectedRowsToGroup(DragDropManagerBase sourceManager, int groupRowHandle, DependencyObject hitElement, bool allowChangeSource) {
			GroupInfo[] groupInfo = GetGroupInfos(groupRowHandle);
			if(!CanMoveSelectedRowsToGroup(sourceManager, groupInfo, hitElement))
				return;
			foreach(object obj in sourceManager.DraggingRows) {
				foreach(GroupInfo item in groupInfo)
					DragDrop.Utils.SetPropertyValue(sourceManager.GetObject(obj), item.FieldName, item.Value);
				if(allowChangeSource && !ItemsSource.Contains(obj)) {
					sourceManager.RemoveObject(sourceManager.GetObject(obj));
					ItemsSource.Add(sourceManager.GetObject(obj));
				}
			}
		}
		protected override TableDragIndicatorPosition GetDragIndicatorPositionForRowElement(FrameworkElement rowElement) {
			if(rowElement == null)
				return TableDragIndicatorPosition.None;
			double point = LastPosition.Y - LayoutHelper.GetRelativeElementRect(rowElement, DataControl).Top;
			return point > rowElement.ActualHeight / 2 ?
							TableDragIndicatorPosition.Bottom :
							TableDragIndicatorPosition.Top;
		}
		bool CanMoveSelectedRowsToGroup(DragDropManagerBase sourceManager, GroupInfo[] groupInfos, DependencyObject hitElement) {
			if(GetRowElement(hitElement) == null)
				return false;
			else return !IsSameGroup(sourceManager, groupInfos, hitElement);
		}
		bool IsSameGroup(DragDropManagerBase sourceManager, GroupInfo[] groupInfos, DependencyObject hitElement) {
			foreach(object obj in sourceManager.DraggingRows) {
				if(GridControl.DataController.FindRowByRowValue(obj) == DataControlBase.InvalidRowHandle)
					return false;
				foreach(GroupInfo groupInfo in groupInfos) {
					if(!object.Equals(DragDrop.Utils.GetPropertyValue(obj, groupInfo.FieldName), groupInfo.Value))
						return false;
				}
			}
			return true;
		}
		GroupInfo[] GetGroupInfos(int rowHandle) {
			int rowLevel = GridControl.GetRowLevelByRowHandle(rowHandle);
			GroupInfo[] groupInfo = new GroupInfo[rowLevel + 1];
			int currentGroupRowHandle = rowHandle;
			for(int i = rowLevel; i >= 0; i--) {
				groupInfo[i] = new GroupInfo() {
					Value = GridControl.GetGroupRowValue(currentGroupRowHandle),
					FieldName = GridControl.SortInfo[i].FieldName
				};
				currentGroupRowHandle = GridControl.GetParentRowHandle(currentGroupRowHandle);
			}
			return groupInfo;
		}
		protected internal override FrameworkElement GetElementAcceptVisitor(DependencyObject hitElement, DataViewHitTestVisitorBase visitor) {
			GetHitInfo(hitElement).Accept(visitor);
			return (visitor as FindTableElementHitTestVisitorBase).StoredHitElement;
		}
		protected override DataViewHitTestVisitorBase CreateFindDataAreaElementHitTestVisitor(DataViewDragDropManager dataViewDragDropManager) {
			return new FindTableViewDataAreaElementHitTestVisitor(dataViewDragDropManager);
		}
		protected override DataViewHitTestVisitorBase CreateFindRowElementHitTestVisitor(DataViewDragDropManager dataViewDragDropManager) {
			return new FindTableViewRowElementHitTestVisitor(dataViewDragDropManager);
		}
		protected internal override StartDragEventArgs RaiseStartDragEvent(IndependentMouseEventArgs e) {
			if(StartDraggingEventHandler != null) {
				GridStartDragEventArgs startDragArgs = new GridStartDragEventArgs(GridControl, this, (MouseDownHitInfo ?? GetHitInfo(e)) as TableViewHitInfo) {
					CanDrag = true,
				};
				StartDraggingEventHandler(this, startDragArgs);
				return startDragArgs;
			}
			return null;
		}
		protected override int GetOverRowHandle(UIElement source, Point pt) {
			UIElement element = GetVisibleHitTestElement(pt);
			GridViewHitInfoBase hitInfo = GetHitInfo(element);
			return hitInfo.RowHandle;
		}
		protected override bool IsExpandable {
			get {
				if(HoverRowHandle != GridControl.InvalidRowHandle &&
					HoverRowHandle <= 0 &&
					HoverRowHandle != GridControl.NewItemRowHandle &&
					HoverRowHandle != GridControl.AutoFilterRowHandle)
					return !GridControl.IsGroupRowExpanded(HoverRowHandle);
				else
					return false;
			}
		}
		protected override void PerformAutoExpand() {
			GridControl.ExpandGroupRow(HoverRowHandle);
		}
		protected internal override bool CanStartDrag(MouseButtonEventArgs e) {
			if(View.IsEditing) return false;
			base.CanStartDrag(e);
			TableViewHitInfo hitInfo = MouseDownHitInfo as TableViewHitInfo;
			return hitInfo.InRow && hitInfo.HitTest != TableViewHitTest.RowIndicator;
		}
		internal override string GetItemsSourceErrorText() {
			return InvalidItemsSourceError;
		}
		protected override IDragElement CreateDragElement(Point offset, FrameworkElement owner) {
			var dragElement = new DataControlDragElement(this, offset, View);
			((ILogicalOwner)View).AddChild(dragElement.FloatingContainer);
			return dragElement;
		}
	}
}
