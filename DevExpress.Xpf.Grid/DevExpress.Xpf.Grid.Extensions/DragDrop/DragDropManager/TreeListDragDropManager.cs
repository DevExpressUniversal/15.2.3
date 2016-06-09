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

using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;
using System.ComponentModel;
using System.Windows.Input;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using System.Collections;
using DevExpress.Xpf.Grid.TreeList;
using DevExpress.Xpf.Grid.DragDrop;
using System.Windows.Data;
using System;
using System.Linq;
#if SL
using DevExpress.Xpf.Core.WPFCompatibility;
using DevExpress.Data.Browsing;
#else
using System.Data;
using DevExpress.Mvvm.UI.Interactivity;
#endif
namespace DevExpress.Xpf.Grid {
	[TargetType(typeof(GridControl))]
	[TargetType(typeof(TreeListControl))]
	public class TreeListDragDropManager : DataViewDragDropManager {
		#region inner classes
		public class TreeListDragSource : SupportDragDropBase {
			TreeListDragDropManager TreeListDragDropManager { get { return (TreeListDragDropManager)dragDropManager; } }
			protected override FrameworkElement Owner { get { return TreeListDragDropManager.DataControl; } }
			public TreeListDragSource(DragDropManagerBase dragDropManager)
				: base(dragDropManager) {
			}
			protected override FrameworkElement SourceElementCore {
				get { return TreeListDragDropManager.DataControl; }
			}
		}
		#endregion
		public TreeListDragDropManager() { }
		static readonly DependencyProperty ViewTreeDerivationModeProperty =
			DependencyProperty.Register("ViewTreeDerivationMode", typeof(TreeDerivationMode),
			typeof(TreeListDragDropManager), new PropertyMetadata(TreeDerivationMode.Selfreference, (d, e) => ((TreeListDragDropManager)d).OnTreeDerivationModeChanged()));
		protected virtual void OnTreeDerivationModeChanged() {
			UpdateDropStrategy();
		}
		[Browsable(false)]
		public TreeDerivationMode ViewTreeDerivationMode {
			get { return (TreeDerivationMode)GetValue(ViewTreeDerivationModeProperty); }
			set { SetValue(ViewTreeDerivationModeProperty, value); }
		}
		public override DataViewBase View {
			get {
				if(TreeListControl != null)
					return TreeListControl.View;
				if(GridControl != null)
					return GridControl.View;
				return null;
			}
		}
		protected internal override IList CalcDraggingRows(IndependentMouseEventArgs e) {
			TreeListViewHitInfo hitInfo = MouseDownHitInfo as TreeListViewHitInfo;
			if(hitInfo.InRow && hitInfo.HitTest != TreeListViewHitTest.RowIndicator)
				return GetSelectedRowsCopy();
			return null;
		}
		protected TreeListDropStrategy DropStrategy { get; set; }
		delegate void MoveRowsDelegate(DragDropManagerBase sourceManager, int targetRowHandle, DependencyObject hitElement);
		public TreeListView TreeListView { get { return View as TreeListView; } }
		TreeListControl TreeListControl { get { return DataControl as TreeListControl; } }
		GridControl GridControl { get { return DataControl as GridControl; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public const string InvalidItemsSourceError = "The TreeList's data source cannot be converted to an IList object, thus the TreeListDragDropManager cannot function properly.";
		protected override ISupportDragDrop CreateDragSource(DataViewDragDropManager dataViewDragDropManager) {
			return new TreeListDragSource(dataViewDragDropManager);
		}
		#region Events
		TreeListDragOverEventHandler DragOverEventHandler;
		TreeListDropEventHandler DropEventHandler;
		TreeListStartDragEventHandler StartDraggingEventHandler;
		TreeListDroppedEventHandler DroppedEventHandler;
		[Category("Events")]
		public event TreeListDragOverEventHandler DragOver {
			add { DragOverEventHandler += value; }
			remove { DragOverEventHandler -= value; }
		}
		[Category("Events")]
		public event TreeListDropEventHandler Drop {
			add { DropEventHandler += value; }
			remove { DropEventHandler -= value; }
		}
		[Category("Events")]
		public event TreeListDroppedEventHandler Dropped {
			add { DroppedEventHandler += value; }
			remove { DroppedEventHandler -= value; }
		}
		[Category("Events")]
		public event TreeListStartDragEventHandler StartDrag {
			add { StartDraggingEventHandler += value; }
			remove { StartDraggingEventHandler -= value; }
		}
		#endregion
		protected override void OnAttached() {
			base.OnAttached();
			BindProperties();
			UpdateDropStrategy();
		}
		protected override void OnDetaching() {
#if !SL
			if(TreeListView != null)
				BindingOperations.ClearBinding(TreeListView, ViewTreeDerivationModeProperty);
#endif
			base.OnDetaching();
		}
		protected override bool AllowMouseMoveSelection(MouseEventArgs args) {
			TreeListView view = TreeListView;
			if(view != null) {
				DependencyObject source = args.OriginalSource as DependencyObject;
				if(source != null) {
					TreeListViewHitInfo hitInfo = view.CalcHitInfo(source);
					return hitInfo.HitTest == TreeListViewHitTest.RowIndicator;
				}
			}
			return false;
		}
		protected virtual void BindProperties() {
			Binding b = new Binding("TreeDerivationMode");
			b.Source = TreeListView;
			BindingOperations.SetBinding(this, ViewTreeDerivationModeProperty, b);
		}
		protected override void ItemsSourceChanged(object sender, ItemsSourceChangedEventArgs e) {
			base.ItemsSourceChanged(sender, e);
			UpdateDropStrategy();
		}
		protected virtual void UpdateDropStrategy() {
			DropStrategy = CreateDropStrategy();
		}
		protected virtual TreeListDropStrategy CreateDropStrategy() {
			TreeListView view = this.TreeListView;
			if(view == null) return null;
			if(DataControl.ItemsSource == null)
				return new UnboundDropStrategy(view);
			if(TreeDerivationMode.Selfreference == view.TreeDerivationMode)
				return new SelfReferenceDropStrategy(view);
			return new EmptyDropStrategy(view);
		}
		protected virtual List<object> GetSelectedRowsCopy() {
			List<object> objects = new List<object>();
			foreach(int rowHandle in TreeListView.DataControl.GetSelectedRowHandles()) 
				objects.Add(TreeListView.GetNodeByRowHandle(rowHandle));
			return objects;
		}
		protected internal override void OnDrop(DragDropManagerBase sourceManager, UIElement source, Point pt) {
			if(DropEventIsLocked) return;
			TreeListDropEventArgs e = RaiseDropEvent(sourceManager, pt);
			if(!e.Handled) {
				ValidateItemsSource();
				PerformDropToView(sourceManager, GetHitInfo(HitElement) as TreeListViewHitInfo, pt, MoveSelectedRows, null, AddRows);
			}
			RaiseDroppedEvent(sourceManager, e);
			base.OnDrop(sourceManager, source, pt);
		}
		protected override GridViewHitInfoBase GetHitInfo(DependencyObject element) {
			return TreeListView.CalcHitInfo(element);
		}
		protected override void AddRow(DragDropManagerBase sourceManager, object row, int insertRowHandle) {
			if(DropStrategy == null)
				UpdateDropStrategy();
			DropStrategy.AddRow(this, sourceManager, row, insertRowHandle);
		}
		protected override void PerformDropToViewCore(DragDropManagerBase sourceManager) {
			TreeListViewHitInfo hitInfo = GetHitInfo(HitElement) as TreeListViewHitInfo;
			if(BanDrop(hitInfo.RowHandle, hitInfo, sourceManager, GetDropTargetTypeByHitElement(HitElement))) {
				ClearDragInfo(sourceManager);
				return;
			}
			PerformDropToView(sourceManager, hitInfo, LastPosition, SetReorderDropInfo, null, SetAddRowsDropInfo);
		}
		void PerformDropToView(DragDropManagerBase sourceManager,TreeListViewHitInfo hitInfo, Point pt, MoveRowsDelegate setreorderDelegate, MoveRowsDelegate moveToGroupDelegate, MoveRowsDelegate addRowsDelegate) {
			int insertRowHandle = hitInfo.RowHandle;
			if(hitInfo.HitTest == TreeListViewHitTest.DataArea) {
				if(sourceManager.DraggingRows.Count > 0 && GetDataAreaElement(HitElement) != null)
					addRowsDelegate(sourceManager, insertRowHandle, HitElement);
				else
					ClearDragInfo(sourceManager);
				return;
			}
			if(insertRowHandle == GridControl.InvalidRowHandle || insertRowHandle == GridControl.AutoFilterRowHandle || insertRowHandle == GridControl.NewItemRowHandle) {
				ClearDragInfo(sourceManager);
				return;
			}
			setreorderDelegate(sourceManager, insertRowHandle, HitElement);
		}
		protected override DragDropDragOverEventArgs RaiseDragOverEvent(GridViewHitInfoBase hitInfo, DragDropManagerBase sourceManager, DropTargetType dropTargetType) {
			TreeListDragOverEventArgs e = new TreeListDragOverEventArgs(DataControl as GridDataControlBase, TreeListView.GetNodeByRowHandle(hitInfo.RowHandle), this, sourceManager, hitInfo as TreeListViewHitInfo, sourceManager.DraggingRows, dropTargetType) {
				Handled = false,
			};
			if(DragOverEventHandler != null)
				DragOverEventHandler(this, e);
			return e;
		}
		protected override bool InternalBanDrop(int insertRowHandle, DragDropManagerBase sourceManager) {
			TreeListNode insertNode = TreeListView.GetNodeByRowHandle(insertRowHandle);
			if(ReferenceEquals(this, sourceManager))
				foreach(TreeListNode node in sourceManager.DraggingRows) {
					if(ReferenceEquals(insertNode, node) || (insertNode != null && insertNode.IsDescendantOf(node)))
						return true;
				}
			return base.InternalBanDrop(insertRowHandle, sourceManager);
		}
		void MoveSelectedRows(DragDropManagerBase sourceManager, int insertRowHandle, DependencyObject hitElement) {
			TreeListNode insertNode = TreeListView.GetNodeByRowHandle(insertRowHandle);
			object insertObject = insertNode.Content;
			if(insertObject == null || sourceManager.DraggingRows == null || sourceManager.DraggingRows.Contains(insertObject))
				return;
			DropTargetType dropTargetType = GetDropTargetTypeByHitElement(hitElement);
			if(dropTargetType != DropTargetType.None) {
				if(DropStrategy == null)
					UpdateDropStrategy();
				DropStrategy.MoveRows(this, sourceManager, insertNode, dropTargetType);
			}
		}
		protected virtual TreeListDropEventArgs RaiseDropEvent(DragDropManagerBase sourceManager, Point pt) {
			UIElement hitElement = GetVisibleHitTestElement(pt);
			TreeListViewHitInfo hitInfo = GetHitInfo(hitElement) as TreeListViewHitInfo;
			TreeListNode insertNode = TreeListView.GetNodeByRowHandle(hitInfo.RowHandle);
			DropTargetType dropTargetType = GetDropTargetTypeByHitElement(hitElement);
			return RaiseDropEvent(sourceManager, hitInfo, insertNode, sourceManager.DraggingRows,
				hitInfo.HitTest == TreeListViewHitTest.DataArea ? DropTargetType.DataArea : dropTargetType);
		}
		protected virtual TreeListDropEventArgs RaiseDropEvent(DragDropManagerBase sourceManager, TreeListViewHitInfo hitInfo, TreeListNode insertNode, IList rows, DropTargetType dropTargetType) {
			TreeListDropEventArgs e = new TreeListDropEventArgs(DataControl as GridDataControlBase, this, sourceManager, insertNode, dropTargetType, hitInfo, rows) {
				Handled = false,
			};
			if(DropEventHandler != null)
				DropEventHandler(this, e);
			return e;
		}
		protected virtual void RaiseDroppedEvent(DragDropManagerBase sourceManager, TreeListDropEventArgs dropEventArgs) {
			if(DroppedEventHandler != null) {
				TreeListDroppedEventArgs e = new TreeListDroppedEventArgs(DataControl as GridDataControlBase, this, sourceManager, dropEventArgs.TargetNode, dropEventArgs.DropTargetType, dropEventArgs.HitInfo, dropEventArgs.DraggedRows);
				DroppedEventHandler(this, e);
			}
		}
		public void InsertObject(IList ItemsSource, TreeListNode node, DropTargetType dropTargetType, object obj, int index) {
			switch(dropTargetType) {
				case DropTargetType.InsertRowsBefore:
				case DropTargetType.InsertRowsAfter:
					ItemsSource.Insert(index, obj);
					break;
				case DropTargetType.DataArea:
				case DropTargetType.InsertRowsIntoNode:
					GetItemsSource(node).Add(obj);
					break;
				default:
					break;
			}
		}
#if !SL
		public void InsertObject(DataRowCollection ItemsSource, TreeListNode node, DropTargetType dropTargetType, DataRow obj, int index) {
			switch(dropTargetType) {
				case DropTargetType.InsertRowsBefore:
				case DropTargetType.InsertRowsAfter:
					ItemsSource.InsertAt(obj, index);
					break;
				case DropTargetType.DataArea:
				case DropTargetType.InsertRowsIntoNode:
					if(TreeListView.TreeDerivationMode == TreeDerivationMode.Selfreference)
						ItemsSource.Add(obj);
					else
						(GetItemsSource(node) as DataView).Table.Rows.Add(obj);
					break;
				default:
					break;
			}
		}
#endif
		object GetObjectByRowHandle(int insertRowHandle) {
			IGridDataRow row = View.GetRowElementByRowHandle(insertRowHandle) as IGridDataRow;
			if(row != null) {
				return row.RowData != null ? row.RowData.Row : null;
			}
			return null;
		}
		void SetReorderDropInfo(DragDropManagerBase sourceManager, int insertRowHandle, DependencyObject hitElement) {
			FrameworkElement row = GetRowElement(hitElement);
			FrameworkElement rowElement = GetRowContentElement(row);
			TableDragIndicatorPosition dragIndicatorPosition = GetDragIndicatorPositionForRowElement(rowElement);
			if(dragIndicatorPosition != TableDragIndicatorPosition.None) {
				DropTargetType dropTargetType = GetDropTargetType(dragIndicatorPosition);
				sourceManager.SetDropTargetType(dropTargetType);
				sourceManager.ViewInfo.DropTargetRow = GetObjectByRowHandle(insertRowHandle);
				sourceManager.ShowDropMarker(rowElement, dragIndicatorPosition, GetRowContentIndent(row));
			} else {
				ClearDragInfo(sourceManager);
			}
		}
		protected override FrameworkElement GetRowContentElementByHitElement(DependencyObject hitElement) {
			return GetRowContentElement(GetRowElement(hitElement));
		}
		FrameworkElement GetRowContentElement(FrameworkElement row) {
			if(row is IFocusedRowBorderObject)
				return ((IFocusedRowBorderObject)row).RowDataContent;
			return row;
		}
		double GetRowContentIndent(FrameworkElement row) {
			if(row is IFocusedRowBorderObject)
				return ((IFocusedRowBorderObject)row).LeftIndent;
			return 0;
		}
		protected override TableDragIndicatorPosition GetDragIndicatorPositionForRowElement(FrameworkElement rowElement) {
			if(rowElement == null)
				return TableDragIndicatorPosition.None;
#if SL
			double point = LastPosition.Y - LayoutHelper.GetRelativeElementRect(rowElement, View).Top;
#else
			double point = LastPosition.Y - LayoutHelper.GetRelativeElementRect(rowElement, DataControl).Top;
#endif
			if(point < rowElement.ActualHeight / 4.0f)
				return TableDragIndicatorPosition.Top;
			else if(point > rowElement.ActualHeight * .75f)
				return TableDragIndicatorPosition.Bottom;
			return TableDragIndicatorPosition.InRow;
		}
		protected internal override FrameworkElement GetElementAcceptVisitor(DependencyObject hitElement, DataViewHitTestVisitorBase visitor) {
			GetHitInfo(hitElement).Accept(visitor);
			return ((FindTreeListViewElementHitTestVisitorBase)visitor).StoredHitElement;
		}
		protected override DataViewHitTestVisitorBase CreateFindDataAreaElementHitTestVisitor(DataViewDragDropManager dataViewDragDropManager) {
			return new FindTreeListViewDataAreaElementHitTestVisitor(dataViewDragDropManager);
		}
		protected override DataViewHitTestVisitorBase CreateFindRowElementHitTestVisitor(DataViewDragDropManager dataViewDragDropManager) {
			return new FindTreeListViewRowElementHitTestVisitor(dataViewDragDropManager);
		}
		public override object GetObject(object obj) {
			TreeListNode node = obj as TreeListNode;
			if(node!=null)
				return node.Content;
			return obj;
		}
		public override IList GetSource(object row) {
			if(TreeListView.TreeDerivationMode == TreeDerivationMode.Selfreference)
				return ItemsSource;
			TreeListNode node = row as TreeListNode;
			if(node == null) return null;
			return GetParentItemsSource(node);
		}
		public override void RemoveObject(object obj) {
			if(obj == null) return;
			if(ItemsSource != null)
				base.RemoveObject(obj);
			else
				TreeListView.DeleteNode(obj as TreeListNode, false);
		}
		IList GetParentItemsSource(TreeListNode node) {
			return (node.ParentNode == null) ? ItemsSource : GetItemsSource(node.ParentNode);
		}
		IList GetItemsSource(TreeListNode node) {
			if(TreeListView.TreeDerivationMode == TreeDerivationMode.Selfreference || node == null)
				return ItemsSource;
			else if(TreeListView.TreeDerivationMode == TreeDerivationMode.ChildNodesSelector
				|| TreeListView.TreeDerivationMode == TreeDerivationMode.HierarchicalDataTemplate)
				return node.ItemsSource;
			return null;
		}
		protected internal override StartDragEventArgs RaiseStartDragEvent(IndependentMouseEventArgs e) {
			if(StartDraggingEventHandler != null) {
				TreeListViewHitInfo hitInfo = (MouseDownHitInfo ?? GetHitInfo(e)) as TreeListViewHitInfo;
				TreeListStartDragEventArgs startDragArgs = new TreeListStartDragEventArgs(DataControl as GridDataControlBase, this, hitInfo , TreeListView.GetNodeByRowHandle(hitInfo.RowHandle)) {
					CanDrag = true,
				};
				StartDraggingEventHandler(this, startDragArgs);
				return startDragArgs;
			}
			return null;
		}
		protected override int GetOverRowHandle(UIElement source, Point pt) {
			UIElement element = GetVisibleHitTestElement(pt);
			return GetHitInfo(element).RowHandle;
		}
		protected override bool IsExpandable {
			get {
				TreeListNode node = TreeListView.GetNodeByRowHandle(HoverRowHandle);
				if(node != null && !node.IsExpanded && 
					(node.IsExpandButtonVisible == DevExpress.Utils.DefaultBoolean.True  || (node.IsExpandButtonVisible == DevExpress.Utils.DefaultBoolean.Default && node.HasVisibleChildren)) && 
					GetDragIndicatorPositionForRowElement(TreeListView.GetRowElementByRowHandle(HoverRowHandle)) == TableDragIndicatorPosition.InRow)
					return true;
				else
					return false;
			}
		}
		protected override void PerformAutoExpand() {
			TreeListView.ExpandNode(HoverRowHandle);
		}
		protected internal override bool CanStartDrag(MouseButtonEventArgs e) {
			if(View.IsEditing) return false;
			base.CanStartDrag(e);
			TreeListViewHitInfo hitInfo = MouseDownHitInfo as TreeListViewHitInfo;
			return hitInfo.InRow;
		}
		internal override string GetItemsSourceErrorText() {
			return InvalidItemsSourceError;
		}
	}
}
