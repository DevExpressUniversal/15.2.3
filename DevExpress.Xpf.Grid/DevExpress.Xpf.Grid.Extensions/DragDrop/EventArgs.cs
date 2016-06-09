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
using System.Collections;
using System.Windows;
using DevExpress.Xpf.Grid.TreeList;
using DevExpress.Xpf.Editors;
using System.ComponentModel;
using System.Windows.Controls;
namespace DevExpress.Xpf.Grid.DragDrop {
	public abstract class DragDropEventArgs : EventArgs {
		public DragDropEventArgs(DragDropManagerBase sourceManager, IList dragRows) {
			DraggedRows = dragRows;
			SourceManager = sourceManager;
		}
		public IList DraggedRows { get; protected set; }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public DragDropManagerBase SourceManager { get; protected set; }
		public Control SourceControl { get { return SourceManager.AssociatedControl; } }
	}
	#region Drop
	abstract public class DropEventArgs : DragDropEventArgs {
		public DropEventArgs(DragDropManagerBase sourceManager, IList dragRows) : base(sourceManager, dragRows) { }
		public bool Handled { get; set; }
	}
	public class ListBoxDropEventArgs : DropEventArgs {
		public ListBoxDropEventArgs(ListBoxEdit listBoxEdit, ListBoxDragDropManager manager, DragDropManagerBase sourceManager, IList dragRows)
			: base(sourceManager, dragRows) {
			ListBoxEdit = listBoxEdit;
			Manager = manager;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public ListBoxDragDropManager Manager { get; protected set; }
		public ListBoxEdit ListBoxEdit { get; protected set; }
	}
	abstract public class DataControlDropEventArgs : DropEventArgs {
		public DataControlDropEventArgs(DragDropManagerBase sourceManager, DropTargetType dropTargetType, IList dragRows)
			: base(sourceManager, dragRows) {
			DropTargetType = dropTargetType;
		}
		public DropTargetType DropTargetType { get; protected set; }
	}
	public class TreeListDropEventArgs : DataControlDropEventArgs {
		public TreeListDropEventArgs(GridDataControlBase dataControl, TreeListDragDropManager manager, DragDropManagerBase sourceManager, TreeListNode targetNode, DropTargetType dropTargetType, TreeListViewHitInfo hitInfo, IList dragRows)
			: base(sourceManager, dropTargetType, dragRows) {
			DataControl = dataControl;
			Manager = manager;
			TargetNode = targetNode;
			HitInfo = hitInfo;
		}
		public TreeListNode TargetNode { get; protected set; }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public TreeListDragDropManager Manager { get; protected set; }
		public TreeListViewHitInfo HitInfo { get; protected set; }
		public GridDataControlBase DataControl { get; protected set; }
	}
	public class GridDropEventArgs : DataControlDropEventArgs {
		public GridDropEventArgs(GridControl gridControl, GridDragDropManager manager, DragDropManagerBase sourceManager, DropTargetType dropTargetType, TableViewHitInfo hitInfo, IList dragRows)
			: base(sourceManager, dropTargetType, dragRows) {
			GridControl = gridControl;
			Manager = manager;
			TargetRowHandle = hitInfo.RowHandle;
			HitInfo = hitInfo;
		}
		public object TargetRow { get { return GridControl.GetRow(TargetRowHandle); } }
		public int TargetRowHandle { get; protected set; }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public GridDragDropManager Manager { get; protected set; }
		public TableViewHitInfo HitInfo { get; protected set; }
		public GridControl GridControl { get; protected set; }
	}
	public delegate void TreeListDropEventHandler(object sender, TreeListDropEventArgs e);
	public delegate void GridDropEventHandler(object sender, GridDropEventArgs e);
	public delegate void ListBoxDropEventHandler(object sender, ListBoxDropEventArgs e);
	#endregion
	#region Dropped
	public class TreeListDroppedEventArgs : DragDropEventArgs {
		public TreeListDroppedEventArgs(GridDataControlBase dataControl, TreeListDragDropManager manager, DragDropManagerBase sourceManager, TreeListNode targetNode, DropTargetType dropTargetType, TreeListViewHitInfo hitInfo, IList dragRows)
			: base(sourceManager, dragRows) {
			DataControl = dataControl;
			Manager = manager;
			TargetNode = targetNode;
			HitInfo = hitInfo;
			DropTargetType = dropTargetType;
		}
		public TreeListNode TargetNode { get; protected set; }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public TreeListDragDropManager Manager { get; protected set; }
		public TreeListViewHitInfo HitInfo { get; protected set; }
		public GridDataControlBase DataControl { get; protected set; }
		public DropTargetType DropTargetType { get; protected set; }
	}
	public class GridDroppedEventArgs : DragDropEventArgs {
		public GridDroppedEventArgs(GridControl gridControl, GridDragDropManager manager, DragDropManagerBase sourceManager, int targetRowHandle, DropTargetType dropTargetType, TableViewHitInfo hitInfo, IList dragRows)
			: base(sourceManager, dragRows) {
			GridControl = gridControl;
			Manager = manager;
			TargetRowHandle = targetRowHandle;
			HitInfo = hitInfo;
			DropTargetType = dropTargetType;
		}
		public int TargetRowHandle { get; protected set; }
		public object TargetRow { get { return GridControl.GetRow(TargetRowHandle); } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public GridDragDropManager Manager { get; protected set; }
		public TableViewHitInfo HitInfo { get; protected set; }
		public GridControl GridControl { get; protected set; }
		public DropTargetType DropTargetType { get; protected set; }
	}
	public class ListBoxDroppedEventArgs : DragDropEventArgs {
		public ListBoxDroppedEventArgs(ListBoxEdit listBoxEdit, ListBoxDragDropManager manager, DragDropManagerBase sourceManager, IList dragRows)
			: base(sourceManager, dragRows) {
			ListBoxEdit = listBoxEdit;
			Manager = manager;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public ListBoxDragDropManager Manager { get; protected set; }
		public ListBoxEdit ListBoxEdit { get; protected set; }
	}
	public delegate void TreeListDroppedEventHandler(object sender, TreeListDroppedEventArgs e);
	public delegate void GridDroppedEventHandler(object sender, GridDroppedEventArgs e);
	public delegate void ListBoxDroppedEventHandler(object sender, ListBoxDroppedEventArgs e);
	#endregion
	#region DragOver
	public class DragDropDragOverEventArgs : DragDropEventArgs {
		public DragDropDragOverEventArgs(DragDropManagerBase sourceManager, IList dragRows)
			: base(sourceManager, dragRows) {
			DraggedRows = dragRows;
			AllowDrop = true;
			ShowDragInfo = true;
			ShowDropMarker = true;
		}
		public bool AllowDrop { get; set; }
		public bool Handled { get; set; }
		public bool ShowDragInfo { get; set; }
		public bool ShowDropMarker { get; set; }
	}
	public class GridDragOverEventArgs : DragDropDragOverEventArgs {
		public GridDragOverEventArgs(GridControl gridControl, GridDragDropManager manager, DragDropManagerBase sourceManager, TableViewHitInfo hitInfo, IList dragRows, DropTargetType dropTargetType)
			: base(sourceManager, dragRows) {
			GridControl = gridControl;
			Manager = manager;
			HitInfo = hitInfo;
			TargetRowHandle = HitInfo.RowHandle;
			DropTargetType = dropTargetType;
		}
		public int TargetRowHandle { get; protected set; }
		public object TargetRow { get { return GridControl.GetRow(TargetRowHandle); } }
		public GridControl GridControl { get; protected set; }
		public TableViewHitInfo HitInfo { get; protected set; }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public GridDragDropManager Manager { get; protected set; }
		public DropTargetType DropTargetType { get; protected set; }
	}
	public class TreeListDragOverEventArgs : DragDropDragOverEventArgs {
		public TreeListDragOverEventArgs(GridDataControlBase dataControl, TreeListNode targetNode, TreeListDragDropManager manager, DragDropManagerBase sourceManager, TreeListViewHitInfo hitInfo, IList dragRows, DropTargetType dropTargetType)
			: base(sourceManager, dragRows) {
			DataControl = dataControl;
			TargetNode = targetNode;
			HitInfo = hitInfo;
			Manager = manager;
			DropTargetType = dropTargetType;
		}
		public TreeListViewHitInfo HitInfo { get; protected set; }
		public TreeListNode TargetNode { get; protected set; }
		public GridDataControlBase DataControl { get; protected set; }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public TreeListDragDropManager Manager { get; protected set; }
		public DropTargetType DropTargetType { get; protected set; }
	}
	public class ListBoxDragOverEventArgs : DragDropDragOverEventArgs {
		public ListBoxDragOverEventArgs(ListBoxEdit listBox, ListBoxDragDropManager manager, DragDropManagerBase sourceManager, IList dragRows)
			: base(sourceManager, dragRows) {
			Manager = manager;
			ListBoxEdit = listBox;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public ListBoxDragDropManager Manager { get; protected set; }
		public ListBoxEdit ListBoxEdit { get; protected set; }
	}
	public delegate void TreeListDragOverEventHandler(object sender, TreeListDragOverEventArgs e);
	public delegate void GridDragOverEventHandler(object sender, GridDragOverEventArgs e);
	public delegate void ListBoxDragOverEventHandler(object sender, ListBoxDragOverEventArgs e);
	#endregion
	#region StartDragging
	public class StartDragEventArgs : EventArgs {
		public bool CanDrag { get; set; }
	}
	public class TreeListStartDragEventArgs : StartDragEventArgs {
		public TreeListStartDragEventArgs(GridDataControlBase dataControl, TreeListDragDropManager manager, TreeListViewHitInfo hitInfo, TreeListNode node) {
			DataControl = dataControl;
			Manager = manager;
			HitInfo = hitInfo;
			Node = node;
		}
		public TreeListViewHitInfo HitInfo { get; protected set; }
		public TreeListNode Node { get; protected set; }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public TreeListDragDropManager Manager { get; protected set; }
		public GridDataControlBase DataControl { get; protected set; }
	}
	public class GridStartDragEventArgs : StartDragEventArgs {
		public GridStartDragEventArgs(GridControl gridControl, GridDragDropManager manager, TableViewHitInfo hitInfo) {
			GridControl = gridControl;
			Manager = manager;
			HitInfo = hitInfo;
			RowHandle = HitInfo.RowHandle;
		}
		public TableViewHitInfo HitInfo { get; protected set; }
		public int RowHandle { get; protected set; }
		public object Row { get { return GridControl.GetRow(RowHandle); } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public GridDragDropManager Manager { get; protected set; }
		public GridControl GridControl { get; protected set; }
	}
	public class ListBoxStartDragEventArgs : StartDragEventArgs {
		public ListBoxStartDragEventArgs(ListBoxEdit listBox, ListBoxDragDropManager manager) {
			ListBoxEdit = listBox;
			Manager = manager;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public ListBoxDragDropManager Manager { get; protected set; }
		public ListBoxEdit ListBoxEdit { get; protected set; }
	}
	public delegate void TreeListStartDragEventHandler(object sender, TreeListStartDragEventArgs e);
	public delegate void GridStartDragEventHandler(object sender, GridStartDragEventArgs e);
	public delegate void ListBoxStartDragEventHandler(object sender, ListBoxStartDragEventArgs e);
	#endregion
	public class DragLeaveEventArgs : EventArgs {
		public DragLeaveEventArgs(DragDropManagerBase manager, DragDropManagerBase sourceManager) {
			Manager = manager;
			SourceManager = sourceManager;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public DragDropManagerBase Manager { get; protected set; }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public DragDropManagerBase SourceManager { get; protected set; }
		public Control SourceControl { get { return SourceManager.AssociatedControl; } }
	}
	public delegate void DragLeaveEventHandler(object sender, DragLeaveEventArgs e);
}
